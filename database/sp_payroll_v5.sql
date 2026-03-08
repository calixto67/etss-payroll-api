CREATE OR ALTER PROCEDURE dbo.sp_Payroll
    @ActionType   VARCHAR(50),
    @Id           INT = NULL,
    @Page         INT = 1,
    @PageSize     INT = 10,
    @EmployeeId   INT = NULL,
    @PeriodId     INT = NULL,
    @EmployeeIds  NVARCHAR(MAX) = NULL,
    @InitiatedBy  NVARCHAR(256) = NULL,
    @ApprovedBy   NVARCHAR(256) = NULL,
    @ReleasedBy   NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

    -- Status IDs from PayrollStatuses table
    DECLARE @StatusDraft       INT = (SELECT Id FROM PayrollStatuses WHERE Name = 'Draft');
    DECLARE @StatusForApproval INT = (SELECT Id FROM PayrollStatuses WHERE Name = 'ForApproval');
    DECLARE @StatusApproved    INT = (SELECT Id FROM PayrollStatuses WHERE Name = 'Approved');
    DECLARE @StatusReleased    INT = (SELECT Id FROM PayrollStatuses WHERE Name = 'Released');

    -- -----------------------------------------------------------------------
    -- GET_PAGED
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_PAGED'
    BEGIN
        BEGIN TRY
            DECLARE @Offset INT = (@Page - 1) * @PageSize;
            DECLARE @TotalCount INT;

            SELECT @TotalCount = COUNT(*)
            FROM PayrollRecords pr
            WHERE pr.IsDeleted = 0
              AND (@EmployeeId IS NULL OR pr.EmployeeId = @EmployeeId)
              AND (@PeriodId IS NULL OR pr.PayrollPeriodId = @PeriodId);

            SELECT
                pr.Id, pr.EmployeeId, pr.PayrollPeriodId,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                e.EmployeeCode, pp.PeriodCode, pp.Name AS PeriodName,
                pr.BasicPay, pr.OvertimePay, pr.HolidayPay, pr.Allowances, pr.GrossPay,
                pr.SssDeduction, pr.PhilHealthDeduction, pr.PagIbigDeduction, pr.TaxWithheld,
                pr.OtherDeductions, pr.TotalDeductions, pr.NetPay,
                ps.Name AS StatusName, pr.Status,
                pr.ProcessedAt, pr.ProcessedBy, pr.Remarks, pr.CreatedAt
            FROM PayrollRecords pr
            INNER JOIN Employees e ON e.Id = pr.EmployeeId
            INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
            LEFT JOIN PayrollStatuses ps ON ps.Id = pr.Status
            WHERE pr.IsDeleted = 0
              AND (@EmployeeId IS NULL OR pr.EmployeeId = @EmployeeId)
              AND (@PeriodId IS NULL OR pr.PayrollPeriodId = @PeriodId)
            ORDER BY pr.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT @TotalCount AS TotalCount;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_BY_ID
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_BY_ID'
    BEGIN
        BEGIN TRY
            SELECT
                pr.Id, pr.EmployeeId, pr.PayrollPeriodId,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                e.EmployeeCode, pp.PeriodCode, pp.Name AS PeriodName,
                pr.BasicPay, pr.OvertimePay, pr.HolidayPay, pr.Allowances, pr.GrossPay,
                pr.SssDeduction, pr.PhilHealthDeduction, pr.PagIbigDeduction, pr.TaxWithheld,
                pr.OtherDeductions, pr.TotalDeductions, pr.NetPay,
                ps.Name AS StatusName, pr.Status,
                pr.ProcessedAt, pr.ProcessedBy, pr.Remarks, pr.CreatedAt
            FROM PayrollRecords pr
            INNER JOIN Employees e ON e.Id = pr.EmployeeId
            INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
            LEFT JOIN PayrollStatuses ps ON ps.Id = pr.Status
            WHERE pr.Id = @Id AND pr.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_DETAILS  (returns itemized allowances/deductions for a payroll record)
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_DETAILS'
    BEGIN
        BEGIN TRY
            SELECT Id, PayrollRecordId, ItemType, ItemName, Amount
            FROM PayrollRecordDetails
            WHERE PayrollRecordId = @Id
            ORDER BY ItemType, ItemName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- RUN  (salary frequency proration + per-period contribution flags)
    -- -----------------------------------------------------------------------
    IF @ActionType = 'RUN'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF NOT EXISTS (SELECT 1 FROM PayrollPeriods WHERE Id = @PeriodId AND IsDeleted = 0)
            BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END

            CREATE TABLE #TargetEmployees (EmployeeId INT);

            IF @EmployeeIds IS NULL
            BEGIN
                INSERT INTO #TargetEmployees (EmployeeId)
                SELECT Id FROM Employees WHERE Status = 1 AND IsDeleted = 0;
            END
            ELSE
            BEGIN
                INSERT INTO #TargetEmployees (EmployeeId)
                SELECT CAST(value AS INT)
                FROM STRING_SPLIT(@EmployeeIds, ',')
                WHERE LTRIM(RTRIM(value)) <> ''
                  AND CAST(value AS INT) IN (SELECT Id FROM Employees WHERE Status = 1 AND IsDeleted = 0);
            END

            -- Delete existing detail rows for records being reprocessed
            DELETE d FROM PayrollRecordDetails d
            INNER JOIN PayrollRecords pr ON pr.Id = d.PayrollRecordId
            WHERE pr.PayrollPeriodId = @PeriodId
              AND pr.EmployeeId IN (SELECT EmployeeId FROM #TargetEmployees)
              AND pr.IsDeleted = 0;

            -- Delete existing records for target employees in this period
            DELETE FROM PayrollRecords
            WHERE PayrollPeriodId = @PeriodId
              AND EmployeeId IN (SELECT EmployeeId FROM #TargetEmployees)
              AND IsDeleted = 0;

            -- Period info
            DECLARE @PeriodStartDate DATE;
            DECLARE @PeriodType INT;
            DECLARE @PPDeductSss BIT;
            DECLARE @PPDeductPhilHealth BIT;
            DECLARE @PPDeductPagIbig BIT;

            SELECT @PeriodStartDate = StartDate,
                   @PeriodType = PeriodType,
                   @PPDeductSss = DeductSss,
                   @PPDeductPhilHealth = DeductPhilHealth,
                   @PPDeductPagIbig = DeductPagIbig
            FROM PayrollPeriods WHERE Id = @PeriodId;

            -- Determine which half of the month this period covers
            DECLARE @PeriodHalf VARCHAR(10);
            IF DAY(@PeriodStartDate) <= 15
                SET @PeriodHalf = '1st_half';
            ELSE
                SET @PeriodHalf = '2nd_half';

            -- Periods per month for this PeriodType
            DECLARE @PeriodPeriodsPerMonth DECIMAL(10,4);
            SET @PeriodPeriodsPerMonth = CASE @PeriodType
                WHEN 1 THEN 2.0      -- Semi-Monthly
                WHEN 2 THEN 1.0      -- Monthly
                WHEN 3 THEN 4.3333   -- Weekly
                ELSE 2.0
            END;

            -- Contribution multipliers from per-period flags
            -- If the flag is ON (1), deduct the full monthly amount for that period.
            -- For split timing (Semi-Monthly with both halves ON), each half gets full amount
            -- but the employee contribution fields should already be set to half-amounts.
            -- For weekly, each flagged period gets the full monthly contribution amount.
            DECLARE @SssMultiplier     DECIMAL(5,4) = CASE WHEN @PPDeductSss = 1 THEN 1.0 ELSE 0.0 END;
            DECLARE @PhilMultiplier    DECIMAL(5,4) = CASE WHEN @PPDeductPhilHealth = 1 THEN 1.0 ELSE 0.0 END;
            DECLARE @PagIbigMultiplier DECIMAL(5,4) = CASE WHEN @PPDeductPagIbig = 1 THEN 1.0 ELSE 0.0 END;

            -- Allowance / Deduction items (for detail rows)
            DECLARE @PeriodHalfDisplay VARCHAR(10);
            SET @PeriodHalfDisplay = CASE @PeriodHalf
                WHEN '1st_half' THEN '1st Half'
                WHEN '2nd_half' THEN '2nd Half'
            END;

            CREATE TABLE #AllowanceItems (EmployeeId INT, ItemName NVARCHAR(200), Amount DECIMAL(18,2));
            INSERT INTO #AllowanceItems (EmployeeId, ItemName, Amount)
            SELECT ea.EmployeeId, at.Name, ea.Amount
            FROM EmployeeAllowances ea
            INNER JOIN AllowanceTypes at ON at.Id = ea.AllowanceTypeId
            INNER JOIN #TargetEmployees te ON te.EmployeeId = ea.EmployeeId
            WHERE ea.IsActive = 1 AND ea.IsDeleted = 0
              AND (ea.Frequency = @PeriodHalfDisplay OR ea.Frequency = 'Both' OR ea.Frequency = 'Monthly' OR ea.Frequency IS NULL);

            CREATE TABLE #DeductionItems (EmployeeId INT, ItemName NVARCHAR(200), Amount DECIMAL(18,2));
            INSERT INTO #DeductionItems (EmployeeId, ItemName, Amount)
            SELECT ed.EmployeeId, dt.Name, ed.Amount
            FROM EmployeeDeductions ed
            INNER JOIN DeductionTypes dt ON dt.Id = ed.DeductionTypeId
            INNER JOIN #TargetEmployees te ON te.EmployeeId = ed.EmployeeId
            WHERE ed.IsActive = 1 AND ed.IsDeleted = 0
              AND (ed.Frequency = @PeriodHalfDisplay OR ed.Frequency = 'Both' OR ed.Frequency = 'Monthly' OR ed.Frequency IS NULL);

            CREATE TABLE #EmpAllowances (EmployeeId INT, TotalAllowances DECIMAL(18,2));
            INSERT INTO #EmpAllowances (EmployeeId, TotalAllowances)
            SELECT EmployeeId, SUM(Amount) FROM #AllowanceItems GROUP BY EmployeeId;

            CREATE TABLE #EmpDeductions (EmployeeId INT, TotalOtherDeductions DECIMAL(18,2));
            INSERT INTO #EmpDeductions (EmployeeId, TotalOtherDeductions)
            SELECT EmployeeId, SUM(Amount) FROM #DeductionItems GROUP BY EmployeeId;

            -- Compute payroll
            INSERT INTO PayrollRecords (
                EmployeeId, PayrollPeriodId, BasicPay, OvertimePay, HolidayPay, Allowances,
                GrossPay, SssDeduction, PhilHealthDeduction, PagIbigDeduction, TaxWithheld,
                OtherDeductions, TotalDeductions, NetPay, Status, Remarks,
                CreatedAt, CreatedBy
            )
            SELECT
                calc.EmployeeId,
                @PeriodId,
                calc.PeriodBasicPay,
                0,  -- OvertimePay
                0,  -- HolidayPay
                calc.TotalAllowances,
                calc.PeriodBasicPay + calc.TotalAllowances,  -- GrossPay
                calc.PeriodSss,
                calc.PeriodPhilHealth,
                calc.PeriodPagIbig,
                -- Tax: monthly tax prorated to period
                ROUND(ISNULL(tb.BaseTax + (calc.MonthlyTaxableIncome - tb.ExcessOver) * tb.TaxRate, 0)
                      / @PeriodPeriodsPerMonth, 2),
                calc.TotalOtherDeductions,
                -- TotalDeductions
                calc.PeriodSss + calc.PeriodPhilHealth + calc.PeriodPagIbig
                    + ROUND(ISNULL(tb.BaseTax + (calc.MonthlyTaxableIncome - tb.ExcessOver) * tb.TaxRate, 0)
                            / @PeriodPeriodsPerMonth, 2)
                    + calc.TotalOtherDeductions,
                -- NetPay
                (calc.PeriodBasicPay + calc.TotalAllowances) - (
                    calc.PeriodSss + calc.PeriodPhilHealth + calc.PeriodPagIbig
                    + ROUND(ISNULL(tb.BaseTax + (calc.MonthlyTaxableIncome - tb.ExcessOver) * tb.TaxRate, 0)
                            / @PeriodPeriodsPerMonth, 2)
                    + calc.TotalOtherDeductions
                ),
                @StatusForApproval,
                NULL,
                GETDATE(),
                @InitiatedBy
            FROM (
                SELECT
                    e.Id AS EmployeeId,

                    -- Period basic pay: normalize salary to monthly, then divide by periods/month
                    ROUND(e.BasicSalary * CASE e.SalaryFrequency
                        WHEN 0 THEN 1.0 WHEN 1 THEN 2.0 WHEN 2 THEN 2.1667 WHEN 3 THEN 4.3333 WHEN 4 THEN 21.75 ELSE 1.0
                    END / @PeriodPeriodsPerMonth, 2) AS PeriodBasicPay,

                    -- Statutory: monthly amount * multiplier (1 or 0 based on period flags)
                    ROUND(e.SssContribution * @SssMultiplier, 2) AS PeriodSss,
                    ROUND(e.PhilHealthContribution * @PhilMultiplier, 2) AS PeriodPhilHealth,
                    ROUND(e.PagIbigContribution * @PagIbigMultiplier, 2) AS PeriodPagIbig,

                    ISNULL(a.TotalAllowances, 0) AS TotalAllowances,
                    ISNULL(d.TotalOtherDeductions, 0) AS TotalOtherDeductions,

                    -- Monthly taxable income for tax bracket lookup
                    e.BasicSalary * CASE e.SalaryFrequency
                        WHEN 0 THEN 1.0 WHEN 1 THEN 2.0 WHEN 2 THEN 2.1667 WHEN 3 THEN 4.3333 WHEN 4 THEN 21.75 ELSE 1.0
                    END - e.SssContribution - e.PhilHealthContribution - e.PagIbigContribution AS MonthlyTaxableIncome

                FROM Employees e
                INNER JOIN #TargetEmployees te ON te.EmployeeId = e.Id
                LEFT JOIN #EmpAllowances a ON a.EmployeeId = e.Id
                LEFT JOIN #EmpDeductions d ON d.EmployeeId = e.Id
            ) calc
            LEFT JOIN TaxBrackets tb ON tb.IsActive = 1
                AND calc.MonthlyTaxableIncome >= tb.MinAmount
                AND (tb.MaxAmount IS NULL OR calc.MonthlyTaxableIncome <= tb.MaxAmount);

            -- Insert allowance detail rows
            INSERT INTO PayrollRecordDetails (PayrollRecordId, ItemType, ItemName, Amount)
            SELECT pr.Id, 'Allowance', ai.ItemName, ai.Amount
            FROM #AllowanceItems ai
            INNER JOIN PayrollRecords pr ON pr.EmployeeId = ai.EmployeeId
                AND pr.PayrollPeriodId = @PeriodId AND pr.IsDeleted = 0 AND pr.CreatedBy = @InitiatedBy;

            -- Insert deduction detail rows
            INSERT INTO PayrollRecordDetails (PayrollRecordId, ItemType, ItemName, Amount)
            SELECT pr.Id, 'Deduction', di.ItemName, di.Amount
            FROM #DeductionItems di
            INNER JOIN PayrollRecords pr ON pr.EmployeeId = di.EmployeeId
                AND pr.PayrollPeriodId = @PeriodId AND pr.IsDeleted = 0 AND pr.CreatedBy = @InitiatedBy;

            -- Return results
            SELECT
                pr.Id, pr.EmployeeId, pr.PayrollPeriodId,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                e.EmployeeCode, pp.PeriodCode, pp.Name AS PeriodName,
                pr.BasicPay, pr.OvertimePay, pr.HolidayPay, pr.Allowances, pr.GrossPay,
                pr.SssDeduction, pr.PhilHealthDeduction, pr.PagIbigDeduction, pr.TaxWithheld,
                pr.OtherDeductions, pr.TotalDeductions, pr.NetPay,
                ps.Name AS StatusName, pr.Status,
                pr.ProcessedAt, pr.ProcessedBy, pr.Remarks, pr.CreatedAt
            FROM PayrollRecords pr
            INNER JOIN Employees e ON e.Id = pr.EmployeeId
            INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
            LEFT JOIN PayrollStatuses ps ON ps.Id = pr.Status
            WHERE pr.PayrollPeriodId = @PeriodId
              AND pr.IsDeleted = 0
              AND pr.CreatedBy = @InitiatedBy
              AND pr.EmployeeId IN (SELECT EmployeeId FROM #TargetEmployees)
            ORDER BY e.LastName, e.FirstName;

            DROP TABLE #TargetEmployees;
            DROP TABLE #AllowanceItems;
            DROP TABLE #DeductionItems;
            DROP TABLE #EmpAllowances;
            DROP TABLE #EmpDeductions;
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#TargetEmployees') IS NOT NULL DROP TABLE #TargetEmployees;
            IF OBJECT_ID('tempdb..#AllowanceItems') IS NOT NULL DROP TABLE #AllowanceItems;
            IF OBJECT_ID('tempdb..#DeductionItems') IS NOT NULL DROP TABLE #DeductionItems;
            IF OBJECT_ID('tempdb..#EmpAllowances') IS NOT NULL DROP TABLE #EmpAllowances;
            IF OBJECT_ID('tempdb..#EmpDeductions') IS NOT NULL DROP TABLE #EmpDeductions;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- APPROVE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'APPROVE'
    BEGIN
        BEGIN TRY
            DECLARE @ApprCurStatus INT;
            SELECT @ApprCurStatus = Status FROM PayrollRecords WHERE Id = @Id AND IsDeleted = 0;
            IF @ApprCurStatus IS NULL
            BEGIN RAISERROR('Payroll record not found.', 16, 1); RETURN; END
            IF @ApprCurStatus <> @StatusForApproval
            BEGIN RAISERROR('Payroll record is not in ForApproval status. Current status: %d', 16, 1, @ApprCurStatus); RETURN; END

            UPDATE PayrollRecords
            SET Status = @StatusApproved, UpdatedBy = @ApprovedBy, UpdatedAt = GETDATE()
            WHERE Id = @Id;

            SELECT
                pr.Id, pr.EmployeeId, pr.PayrollPeriodId,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                e.EmployeeCode, pp.PeriodCode, pp.Name AS PeriodName,
                pr.BasicPay, pr.OvertimePay, pr.HolidayPay, pr.Allowances, pr.GrossPay,
                pr.SssDeduction, pr.PhilHealthDeduction, pr.PagIbigDeduction, pr.TaxWithheld,
                pr.OtherDeductions, pr.TotalDeductions, pr.NetPay,
                ps.Name AS StatusName, pr.Status,
                pr.ProcessedAt, pr.ProcessedBy, pr.Remarks, pr.CreatedAt
            FROM PayrollRecords pr
            INNER JOIN Employees e ON e.Id = pr.EmployeeId
            INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
            LEFT JOIN PayrollStatuses ps ON ps.Id = pr.Status
            WHERE pr.Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- RELEASE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'RELEASE'
    BEGIN
        BEGIN TRY
            DECLARE @RelCurStatus INT;
            SELECT @RelCurStatus = Status FROM PayrollRecords WHERE Id = @Id AND IsDeleted = 0;
            IF @RelCurStatus IS NULL
            BEGIN RAISERROR('Payroll record not found.', 16, 1); RETURN; END
            IF @RelCurStatus <> @StatusApproved
            BEGIN RAISERROR('Payroll record is not in Approved status. Current status: %d', 16, 1, @RelCurStatus); RETURN; END

            UPDATE PayrollRecords
            SET Status = @StatusReleased,
                ProcessedAt = GETDATE(),
                ProcessedBy = @ReleasedBy,
                UpdatedBy = @ReleasedBy,
                UpdatedAt = GETDATE()
            WHERE Id = @Id;

            SELECT
                pr.Id, pr.EmployeeId, pr.PayrollPeriodId,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                e.EmployeeCode, pp.PeriodCode, pp.Name AS PeriodName,
                pr.BasicPay, pr.OvertimePay, pr.HolidayPay, pr.Allowances, pr.GrossPay,
                pr.SssDeduction, pr.PhilHealthDeduction, pr.PagIbigDeduction, pr.TaxWithheld,
                pr.OtherDeductions, pr.TotalDeductions, pr.NetPay,
                ps.Name AS StatusName, pr.Status,
                pr.ProcessedAt, pr.ProcessedBy, pr.Remarks, pr.CreatedAt
            FROM PayrollRecords pr
            INNER JOIN Employees e ON e.Id = pr.EmployeeId
            INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
            LEFT JOIN PayrollStatuses ps ON ps.Id = pr.Status
            WHERE pr.Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_BY_PERIOD
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_BY_PERIOD'
    BEGIN
        BEGIN TRY
            SELECT
                pr.Id, pr.EmployeeId, pr.PayrollPeriodId,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                e.EmployeeCode, pp.PeriodCode, pp.Name AS PeriodName,
                pr.BasicPay, pr.OvertimePay, pr.HolidayPay, pr.Allowances, pr.GrossPay,
                pr.SssDeduction, pr.PhilHealthDeduction, pr.PagIbigDeduction, pr.TaxWithheld,
                pr.OtherDeductions, pr.TotalDeductions, pr.NetPay,
                ps.Name AS StatusName, pr.Status,
                pr.ProcessedAt, pr.ProcessedBy, pr.Remarks, pr.CreatedAt
            FROM PayrollRecords pr
            INNER JOIN Employees e ON e.Id = pr.EmployeeId
            INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
            LEFT JOIN PayrollStatuses ps ON ps.Id = pr.Status
            WHERE pr.PayrollPeriodId = @PeriodId AND pr.IsDeleted = 0
            ORDER BY e.LastName, e.FirstName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    END TRY
    BEGIN CATCH
        INSERT INTO ErrorLogs (ModuleName, ProcedureName, ActionType,
            ErrorNumber, ErrorMessage, ErrorSeverity, ErrorState, ErrorLine, ErrorProcedure,
            ParameterValues, UserContext, CreatedDate)
        VALUES ('Payroll', 'sp_Payroll', @ActionType,
            ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_SEVERITY(), ERROR_STATE(),
            ERROR_LINE(), ERROR_PROCEDURE(),
            'PeriodId=' + ISNULL(CAST(@PeriodId AS VARCHAR), 'NULL') + ',EmployeeId=' + ISNULL(CAST(@EmployeeId AS VARCHAR), 'NULL'),
            ISNULL(@InitiatedBy, ISNULL(@ApprovedBy, @ReleasedBy)), GETDATE());
        THROW;
    END CATCH
END
