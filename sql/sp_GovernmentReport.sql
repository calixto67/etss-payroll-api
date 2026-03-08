-- ============================================================
-- sp_GovernmentReport
-- Single SP for all government report queries
-- ActionTypes: GET_SSS, GET_PHILHEALTH, GET_PAGIBIG, GET_BIR
-- Filters: PayrollPeriodId, DepartmentId, BranchId, EmployeeId
-- ============================================================
CREATE OR ALTER PROCEDURE sp_GovernmentReport
    @ActionType       VARCHAR(50),
    @PayrollPeriodId  INT          = NULL,
    @DepartmentId     INT          = NULL,
    @BranchId         INT          = NULL,
    @EmployeeId       INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- ── SSS Contribution Report ─────────────────────────────────
    IF @ActionType = 'GET_SSS'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            e.SssNumber,
            d.DepartmentName,
            pr.BasicPay,
            pr.SssDeduction   AS EmployeeShare,
            pr.SssDeduction   AS EmployerShare,
            pr.SssDeduction * 2 AS TotalContribution,
            pp.Name           AS PeriodName,
            pp.StartDate      AS PeriodStart,
            pp.EndDate        AS PeriodEnd,
            pr.Status
        FROM PayrollRecords pr
        INNER JOIN Employees e   ON e.Id = pr.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
        LEFT  JOIN Departments d ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        WHERE pr.IsDeleted = 0
          AND pr.PayrollPeriodId = @PayrollPeriodId
          AND pr.Status IN (3, 4)
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY d.DepartmentName, e.LastName, e.FirstName;
    END

    -- ── PhilHealth Premium Report ───────────────────────────────
    ELSE IF @ActionType = 'GET_PHILHEALTH'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            e.PhilHealthNumber,
            d.DepartmentName,
            pr.BasicPay,
            pr.PhilHealthDeduction AS EmployeeShare,
            pr.PhilHealthDeduction AS EmployerShare,
            pr.PhilHealthDeduction * 2 AS TotalPremium,
            pp.Name           AS PeriodName,
            pp.StartDate      AS PeriodStart,
            pp.EndDate        AS PeriodEnd,
            pr.Status
        FROM PayrollRecords pr
        INNER JOIN Employees e   ON e.Id = pr.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
        LEFT  JOIN Departments d ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        WHERE pr.IsDeleted = 0
          AND pr.PayrollPeriodId = @PayrollPeriodId
          AND pr.Status IN (3, 4)
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY d.DepartmentName, e.LastName, e.FirstName;
    END

    -- ── Pag-IBIG Contribution Report ────────────────────────────
    ELSE IF @ActionType = 'GET_PAGIBIG'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            e.PagIbigNumber,
            d.DepartmentName,
            pr.BasicPay,
            pr.PagIbigDeduction AS EmployeeShare,
            pr.PagIbigDeduction AS EmployerShare,
            pr.PagIbigDeduction * 2 AS TotalContribution,
            pp.Name           AS PeriodName,
            pp.StartDate      AS PeriodStart,
            pp.EndDate        AS PeriodEnd,
            pr.Status
        FROM PayrollRecords pr
        INNER JOIN Employees e   ON e.Id = pr.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
        LEFT  JOIN Departments d ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        WHERE pr.IsDeleted = 0
          AND pr.PayrollPeriodId = @PayrollPeriodId
          AND pr.Status IN (3, 4)
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY d.DepartmentName, e.LastName, e.FirstName;
    END

    -- ── BIR Withholding Tax Report ──────────────────────────────
    ELSE IF @ActionType = 'GET_BIR'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            e.TaxIdentificationNumber AS Tin,
            d.DepartmentName,
            pr.BasicPay,
            pr.OvertimePay,
            pr.HolidayPay,
            pr.Allowances,
            pr.GrossPay,
            (pr.SssDeduction + pr.PhilHealthDeduction + pr.PagIbigDeduction)
                              AS TotalMandatoryDeductions,
            (pr.GrossPay - pr.SssDeduction - pr.PhilHealthDeduction - pr.PagIbigDeduction)
                              AS TaxableIncome,
            pr.TaxWithheld,
            pr.NetPay,
            pp.Name           AS PeriodName,
            pp.StartDate      AS PeriodStart,
            pp.EndDate        AS PeriodEnd,
            pr.Status
        FROM PayrollRecords pr
        INNER JOIN Employees e   ON e.Id = pr.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
        LEFT  JOIN Departments d ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        WHERE pr.IsDeleted = 0
          AND pr.PayrollPeriodId = @PayrollPeriodId
          AND pr.Status IN (3, 4)
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY d.DepartmentName, e.LastName, e.FirstName;
    END
END
GO
