-- ============================================================================
-- ETSS Payroll System - Consolidated Stored Procedures
-- Database: ETSSPayrollDb (SQL Server)
-- Generated: 2026-03-08
-- Convention: ONE stored procedure per module/entity with @ActionType routing
-- ============================================================================
-- ENUM VALUES:
--   EmploymentStatus: Active=1, Inactive=2, OnLeave=3, Terminated=4, Retired=5, Suspended=6
--   PayrollStatus: Draft=1, ForApproval=2, Approved=3, Released=4, Cancelled=5
--   PeriodStatus: Draft=1, Open=2, Computed=3, Approved=4, Locked=5, Exported=6
--   AttendanceStatus: Complete=1, Review=2, Absent=3
--   LeaveApplicationStatus: Pending=1, Approved=2, Rejected=3, Cancelled=4
--   LeaveYearEndStatus: Completed=1, RolledBack=2
--   HolidayType: Public=1, Company=2, Regional=3, Special=4
--   LeaveCarryForwardPolicy: None=1, Limited=2, Full=3
-- ============================================================================

-- ============================================================================
-- DROP ALL OLD INDIVIDUAL STORED PROCEDURES (95 total)
-- ============================================================================
IF OBJECT_ID('dbo.sp_Payroll_GetPaged', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payroll_GetPaged;
IF OBJECT_ID('dbo.sp_Payroll_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payroll_GetById;
IF OBJECT_ID('dbo.sp_Payroll_Run', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payroll_Run;
IF OBJECT_ID('dbo.sp_Payroll_Approve', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payroll_Approve;
IF OBJECT_ID('dbo.sp_Payroll_Release', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payroll_Release;
IF OBJECT_ID('dbo.sp_Payroll_GetByPeriod', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payroll_GetByPeriod;
IF OBJECT_ID('dbo.sp_Attendance_GetByPeriod', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_GetByPeriod;
IF OBJECT_ID('dbo.sp_Attendance_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_GetById;
IF OBJECT_ID('dbo.sp_Attendance_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_Create;
IF OBJECT_ID('dbo.sp_Attendance_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_Update;
IF OBJECT_ID('dbo.sp_Attendance_Resolve', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_Resolve;
IF OBJECT_ID('dbo.sp_Attendance_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_Delete;
IF OBJECT_ID('dbo.sp_Attendance_GetDetails', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_GetDetails;
IF OBJECT_ID('dbo.sp_Attendance_UpdateDetails', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_UpdateDetails;
IF OBJECT_ID('dbo.sp_Attendance_Import', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_Import;
IF OBJECT_ID('dbo.sp_Attendance_ImportRawPunches', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_ImportRawPunches;
IF OBJECT_ID('dbo.sp_Attendance_CheckSchedules', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance_CheckSchedules;
IF OBJECT_ID('dbo.sp_Leave_GetApplications', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_GetApplications;
IF OBJECT_ID('dbo.sp_Leave_CreateApplication', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_CreateApplication;
IF OBJECT_ID('dbo.sp_Leave_UpdateApplicationStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_UpdateApplicationStatus;
IF OBJECT_ID('dbo.sp_Leave_GetBalances', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_GetBalances;
IF OBJECT_ID('dbo.sp_Leave_CreateBalance', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_CreateBalance;
IF OBJECT_ID('dbo.sp_Leave_UpdateBalance', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_UpdateBalance;
IF OBJECT_ID('dbo.sp_Leave_DeleteBalance', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_DeleteBalance;
IF OBJECT_ID('dbo.sp_Leave_EnrollAllEmployees', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_EnrollAllEmployees;
IF OBJECT_ID('dbo.sp_Leave_RunYearEnd', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_RunYearEnd;
IF OBJECT_ID('dbo.sp_Leave_RollbackYearEnd', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_RollbackYearEnd;
IF OBJECT_ID('dbo.sp_Leave_GetLastBatch', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_GetLastBatch;
IF OBJECT_ID('dbo.sp_Leave_GetHolidays', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_GetHolidays;
IF OBJECT_ID('dbo.sp_Leave_CreateHoliday', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_CreateHoliday;
IF OBJECT_ID('dbo.sp_Leave_DeleteHoliday', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave_DeleteHoliday;
IF OBJECT_ID('dbo.sp_Employee_GetPaged', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_GetPaged;
IF OBJECT_ID('dbo.sp_Employee_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_GetById;
IF OBJECT_ID('dbo.sp_Employee_GetDetail', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_GetDetail;
IF OBJECT_ID('dbo.sp_Employee_GetByCode', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_GetByCode;
IF OBJECT_ID('dbo.sp_Employee_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_Create;
IF OBJECT_ID('dbo.sp_Employee_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_Update;
IF OBJECT_ID('dbo.sp_Employee_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_Delete;
IF OBJECT_ID('dbo.sp_Employee_ChangeStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_ChangeStatus;
IF OBJECT_ID('dbo.sp_Employee_GetStatusHistory', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_GetStatusHistory;
IF OBJECT_ID('dbo.sp_Employee_GetEmergencyContacts', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_GetEmergencyContacts;
IF OBJECT_ID('dbo.sp_Employee_CreateEmergencyContact', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_CreateEmergencyContact;
IF OBJECT_ID('dbo.sp_Employee_UpdateEmergencyContact', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_UpdateEmergencyContact;
IF OBJECT_ID('dbo.sp_Employee_DeleteEmergencyContact', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_DeleteEmergencyContact;
IF OBJECT_ID('dbo.sp_Employee_CreateDocument', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_CreateDocument;
IF OBJECT_ID('dbo.sp_Employee_UpdateDocument', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_UpdateDocument;
IF OBJECT_ID('dbo.sp_Employee_DeleteDocument', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee_DeleteDocument;
IF OBJECT_ID('dbo.sp_PayPeriod_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_PayPeriod_GetAll;
IF OBJECT_ID('dbo.sp_PayPeriod_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_PayPeriod_GetById;
IF OBJECT_ID('dbo.sp_PayPeriod_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_PayPeriod_Create;
IF OBJECT_ID('dbo.sp_PayPeriod_UpdateStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_PayPeriod_UpdateStatus;
IF OBJECT_ID('dbo.sp_PayPeriod_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_PayPeriod_Delete;
IF OBJECT_ID('dbo.sp_WorkSchedule_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_GetAll;
IF OBJECT_ID('dbo.sp_WorkSchedule_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_GetById;
IF OBJECT_ID('dbo.sp_WorkSchedule_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_Create;
IF OBJECT_ID('dbo.sp_WorkSchedule_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_Update;
IF OBJECT_ID('dbo.sp_WorkSchedule_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_Delete;
IF OBJECT_ID('dbo.sp_WorkSchedule_AssignEmployees', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_AssignEmployees;
IF OBJECT_ID('dbo.sp_WorkSchedule_GetEmployees', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_GetEmployees;
IF OBJECT_ID('dbo.sp_WorkSchedule_UnassignEmployee', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule_UnassignEmployee;
IF OBJECT_ID('dbo.sp_ScheduleRule_Get', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ScheduleRule_Get;
IF OBJECT_ID('dbo.sp_ScheduleRule_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ScheduleRule_Update;
IF OBJECT_ID('dbo.sp_Role_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_GetAll;
IF OBJECT_ID('dbo.sp_Role_GetWithPermissions', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_GetWithPermissions;
IF OBJECT_ID('dbo.sp_Role_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_Create;
IF OBJECT_ID('dbo.sp_Role_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_Update;
IF OBJECT_ID('dbo.sp_Role_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_Delete;
IF OBJECT_ID('dbo.sp_Role_UpdatePermissions', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_UpdatePermissions;
IF OBJECT_ID('dbo.sp_Role_GetAllUsers', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_GetAllUsers;
IF OBJECT_ID('dbo.sp_Role_GetUsersForRole', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_GetUsersForRole;
IF OBJECT_ID('dbo.sp_Role_AssignUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_AssignUser;
IF OBJECT_ID('dbo.sp_Role_RemoveUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_RemoveUser;
IF OBJECT_ID('dbo.sp_Role_GetUserPermissions', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role_GetUserPermissions;
IF OBJECT_ID('dbo.sp_User_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_Create;
IF OBJECT_ID('dbo.sp_User_ResetPassword', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_ResetPassword;
IF OBJECT_ID('dbo.sp_Auth_GetUserForLogin', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Auth_GetUserForLogin;
IF OBJECT_ID('dbo.sp_CompanySettings_Get', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CompanySettings_Get;
IF OBJECT_ID('dbo.sp_CompanySettings_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CompanySettings_Update;
IF OBJECT_ID('dbo.sp_CompanySettings_UpdateDeductions', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CompanySettings_UpdateDeductions;
IF OBJECT_ID('dbo.sp_GlobalConfig_GetByName', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GlobalConfig_GetByName;
IF OBJECT_ID('dbo.sp_GlobalConfig_Upsert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GlobalConfig_Upsert;
IF OBJECT_ID('dbo.sp_LeaveType_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LeaveType_GetAll;
IF OBJECT_ID('dbo.sp_LeaveType_GetAllActive', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LeaveType_GetAllActive;
IF OBJECT_ID('dbo.sp_LeaveType_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LeaveType_GetById;
IF OBJECT_ID('dbo.sp_LeaveType_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LeaveType_Create;
IF OBJECT_ID('dbo.sp_LeaveType_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LeaveType_Update;
IF OBJECT_ID('dbo.sp_LeaveType_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LeaveType_Delete;
IF OBJECT_ID('dbo.sp_AllowanceType_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllowanceType_GetAll;
IF OBJECT_ID('dbo.sp_AllowanceType_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllowanceType_GetById;
IF OBJECT_ID('dbo.sp_AllowanceType_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllowanceType_Create;
IF OBJECT_ID('dbo.sp_AllowanceType_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllowanceType_Update;
IF OBJECT_ID('dbo.sp_AllowanceType_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllowanceType_Delete;
IF OBJECT_ID('dbo.sp_Lookup_GetDepartments', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Lookup_GetDepartments;
IF OBJECT_ID('dbo.sp_Lookup_GetPositions', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Lookup_GetPositions;
IF OBJECT_ID('dbo.sp_Lookup_GetBranches', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Lookup_GetBranches;
GO

-- Drop new consolidated SPs if they exist (for re-runnability)
IF OBJECT_ID('dbo.sp_Payroll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payroll;
IF OBJECT_ID('dbo.sp_Attendance', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Attendance;
IF OBJECT_ID('dbo.sp_Leave', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Leave;
IF OBJECT_ID('dbo.sp_Employee', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Employee;
IF OBJECT_ID('dbo.sp_PayPeriod', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_PayPeriod;
IF OBJECT_ID('dbo.sp_WorkSchedule', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_WorkSchedule;
IF OBJECT_ID('dbo.sp_ScheduleRule', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ScheduleRule;
IF OBJECT_ID('dbo.sp_Role', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Role;
IF OBJECT_ID('dbo.sp_User', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User;
IF OBJECT_ID('dbo.sp_CompanySettings', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CompanySettings;
IF OBJECT_ID('dbo.sp_GlobalConfig', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GlobalConfig;
IF OBJECT_ID('dbo.sp_LeaveType', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LeaveType;
IF OBJECT_ID('dbo.sp_AllowanceType', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AllowanceType;
IF OBJECT_ID('dbo.sp_Lookup', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Lookup;
GO

-- ============================================================================
-- Lookup Tables (PayrollStatuses, TaxBrackets)
-- ============================================================================
IF OBJECT_ID('dbo.PayrollStatuses', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PayrollStatuses (
        Id          INT          NOT NULL PRIMARY KEY,
        Name        VARCHAR(50)  NOT NULL,
        Description VARCHAR(200) NULL
    );
    INSERT INTO dbo.PayrollStatuses (Id, Name, Description) VALUES
        (1, 'Draft',       'Initial draft state'),
        (2, 'ForApproval', 'Submitted for approval'),
        (3, 'Approved',    'Approved by manager'),
        (4, 'Released',    'Payment released'),
        (5, 'Cancelled',   'Cancelled');
END
GO

IF OBJECT_ID('dbo.TaxBrackets', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaxBrackets (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        BracketName     VARCHAR(100)  NOT NULL,
        MinAmount       DECIMAL(18,2) NOT NULL,
        MaxAmount       DECIMAL(18,2) NULL,       -- NULL = no upper limit
        BaseTax         DECIMAL(18,2) NOT NULL,
        TaxRate         DECIMAL(8,4)  NOT NULL,    -- e.g. 0.15 = 15%
        ExcessOver      DECIMAL(18,2) NOT NULL,
        EffectiveDate   DATE          NOT NULL DEFAULT '2023-01-01',
        IsActive        BIT           NOT NULL DEFAULT 1
    );
    INSERT INTO dbo.TaxBrackets (BracketName, MinAmount, MaxAmount, BaseTax, TaxRate, ExcessOver) VALUES
        ('0%',  0,         20833,  0,          0.00, 0),
        ('15%', 20833.01,  33332,  0,          0.15, 20833),
        ('20%', 33332.01,  66666,  1875.00,    0.20, 33333),
        ('25%', 66666.01,  166666, 8541.80,    0.25, 66667),
        ('30%', 166666.01, 666666, 33541.80,   0.30, 166667),
        ('35%', 666666.01, NULL,   183541.80,  0.35, 666667);
END
GO

-- ============================================================================
-- 1. sp_Payroll
-- ============================================================================
CREATE PROCEDURE dbo.sp_Payroll
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
    -- RUN  (uses TaxBrackets table instead of hardcoded CASE)
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

            DELETE te FROM #TargetEmployees te
            WHERE EXISTS (
                SELECT 1 FROM PayrollRecords pr
                WHERE pr.EmployeeId = te.EmployeeId
                  AND pr.PayrollPeriodId = @PeriodId
                  AND pr.IsDeleted = 0
            );

            -- Compute payroll using TaxBrackets table
            INSERT INTO PayrollRecords (
                EmployeeId, PayrollPeriodId, BasicPay, OvertimePay, HolidayPay, Allowances,
                GrossPay, SssDeduction, PhilHealthDeduction, PagIbigDeduction, TaxWithheld,
                OtherDeductions, TotalDeductions, NetPay, Status, Remarks,
                CreatedAt, CreatedBy
            )
            SELECT
                calc.EmployeeId,
                @PeriodId,
                calc.BasicSalary,
                0,  -- OvertimePay
                0,  -- HolidayPay
                0,  -- Allowances
                calc.BasicSalary,  -- GrossPay
                calc.SssContribution,
                calc.PhilHealthContribution,
                calc.PagIbigContribution,
                ISNULL(tb.BaseTax + (calc.TaxableIncome - tb.ExcessOver) * tb.TaxRate, 0), -- TaxWithheld
                0,  -- OtherDeductions
                -- TotalDeductions
                calc.SssContribution + calc.PhilHealthContribution + calc.PagIbigContribution
                    + ISNULL(tb.BaseTax + (calc.TaxableIncome - tb.ExcessOver) * tb.TaxRate, 0),
                -- NetPay
                calc.BasicSalary - (
                    calc.SssContribution + calc.PhilHealthContribution + calc.PagIbigContribution
                    + ISNULL(tb.BaseTax + (calc.TaxableIncome - tb.ExcessOver) * tb.TaxRate, 0)
                ),
                @StatusForApproval,  -- Status from PayrollStatuses
                NULL,
                GETDATE(),
                @InitiatedBy
            FROM (
                SELECT
                    e.Id AS EmployeeId,
                    e.BasicSalary,
                    e.SssContribution,
                    e.PhilHealthContribution,
                    e.PagIbigContribution,
                    e.BasicSalary - e.SssContribution - e.PhilHealthContribution - e.PagIbigContribution AS TaxableIncome
                FROM Employees e
                INNER JOIN #TargetEmployees te ON te.EmployeeId = e.Id
            ) calc
            LEFT JOIN TaxBrackets tb ON tb.IsActive = 1
                AND calc.TaxableIncome >= tb.MinAmount
                AND (tb.MaxAmount IS NULL OR calc.TaxableIncome <= tb.MaxAmount);

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
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#TargetEmployees') IS NOT NULL DROP TABLE #TargetEmployees;
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
            SET Status = @StatusReleased, ProcessedAt = GETDATE(), ProcessedBy = @ReleasedBy,
                UpdatedBy = @ReleasedBy, UpdatedAt = GETDATE()
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

    RAISERROR('Invalid @ActionType for sp_Payroll: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 2. sp_Attendance
-- ============================================================================
CREATE PROCEDURE dbo.sp_Attendance
    @ActionType     VARCHAR(50),
    @Id             INT = NULL,
    @PeriodId       INT = NULL,
    @EmployeeId     INT = NULL,
    @EmployeeCode   NVARCHAR(50) = NULL,
    @Search         NVARCHAR(200) = NULL,
    @DaysWorked     DECIMAL(18,2) = NULL,
    @TotalDays      DECIMAL(18,2) = NULL,
    @LateHours      DECIMAL(18,2) = NULL,
    @UndertimeHours DECIMAL(18,2) = NULL,
    @OtHours        DECIMAL(18,2) = NULL,
    @NightDiffHours DECIMAL(18,2) = NULL,
    @Status         NVARCHAR(50) = NULL,
    @Issue          NVARCHAR(500) = NULL,
    @Resolution     NVARCHAR(50) = NULL,
    @Notes          NVARCHAR(1000) = NULL,
    @RowsJson       NVARCHAR(MAX) = NULL,
    @PunchesJson    NVARCHAR(MAX) = NULL,
    @DetailsJson    NVARCHAR(MAX) = NULL,
    @EmployeeCodes  NVARCHAR(MAX) = NULL,
    @CreatedBy      NVARCHAR(256) = NULL,
    @UpdatedBy      NVARCHAR(256) = NULL,
    @DeletedBy      NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- -----------------------------------------------------------------------
    -- GET_BY_PERIOD
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_BY_PERIOD'
    BEGIN
        BEGIN TRY
            SELECT
                a.Id, a.PayrollPeriodId, a.EmployeeId, e.EmployeeCode,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                a.DaysWorked, a.TotalDays, a.LateHours, a.UndertimeHours,
                a.OtHours, a.NightDiffHours, a.Status, a.Issue, a.ResolutionNotes
            FROM Attendances a
            INNER JOIN Employees e ON e.Id = a.EmployeeId
            WHERE a.PayrollPeriodId = @PeriodId
              AND a.IsDeleted = 0
              AND (@Search IS NULL
                   OR e.FirstName LIKE '%' + @Search + '%'
                   OR e.LastName LIKE '%' + @Search + '%'
                   OR e.EmployeeCode LIKE '%' + @Search + '%'
                   OR (e.LastName + ', ' + e.FirstName) LIKE '%' + @Search + '%')
            ORDER BY e.LastName, e.FirstName;
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
                a.Id, a.PayrollPeriodId, a.EmployeeId, e.EmployeeCode,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                a.DaysWorked, a.TotalDays, a.LateHours, a.UndertimeHours,
                a.OtHours, a.NightDiffHours, a.Status, a.Issue, a.ResolutionNotes
            FROM Attendances a
            INNER JOIN Employees e ON e.Id = a.EmployeeId
            WHERE a.Id = @Id AND a.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            IF @EmployeeId IS NULL AND @EmployeeCode IS NOT NULL
                SELECT @EmployeeId = Id FROM Employees WHERE EmployeeCode = @EmployeeCode AND IsDeleted = 0;

            IF @EmployeeId IS NULL
            BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END

            IF NOT EXISTS (SELECT 1 FROM PayrollPeriods WHERE Id = @PeriodId AND IsDeleted = 0)
            BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM Attendances WHERE EmployeeId = @EmployeeId AND PayrollPeriodId = @PeriodId AND IsDeleted = 0)
            BEGIN RAISERROR('Attendance already exists for this employee and period.', 16, 1); RETURN; END

            DECLARE @CrStatusInt INT = CASE WHEN @Status IN ('Complete','1') THEN 1 WHEN @Status IN ('Review','2') THEN 2 WHEN @Status IN ('Absent','3') THEN 3 ELSE CAST(@Status AS INT) END;

            INSERT INTO Attendances (PayrollPeriodId, EmployeeId, DaysWorked, TotalDays, LateHours, UndertimeHours, OtHours, NightDiffHours, Status, Issue, CreatedAt, CreatedBy)
            VALUES (@PeriodId, @EmployeeId, @DaysWorked, @TotalDays, @LateHours, @UndertimeHours, @OtHours, @NightDiffHours, @CrStatusInt, @Issue, GETDATE(), @CreatedBy);

            DECLARE @CrNewId INT = SCOPE_IDENTITY();

            SELECT a.Id, a.PayrollPeriodId, a.EmployeeId, e.EmployeeCode,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                a.DaysWorked, a.TotalDays, a.LateHours, a.UndertimeHours,
                a.OtHours, a.NightDiffHours, a.Status, a.Issue, a.ResolutionNotes
            FROM Attendances a INNER JOIN Employees e ON e.Id = a.EmployeeId
            WHERE a.Id = @CrNewId;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Attendances WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Attendance record not found.', 16, 1); RETURN; END

            DECLARE @UpStatusInt INT = CASE WHEN @Status IN ('Complete','1') THEN 1 WHEN @Status IN ('Review','2') THEN 2 WHEN @Status IN ('Absent','3') THEN 3 ELSE CAST(@Status AS INT) END;

            UPDATE Attendances SET DaysWorked=@DaysWorked, TotalDays=@TotalDays, LateHours=@LateHours, UndertimeHours=@UndertimeHours,
                OtHours=@OtHours, NightDiffHours=@NightDiffHours, Status=@UpStatusInt, Issue=@Issue, UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @Id;

            SELECT a.Id, a.PayrollPeriodId, a.EmployeeId, e.EmployeeCode,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                a.DaysWorked, a.TotalDays, a.LateHours, a.UndertimeHours,
                a.OtHours, a.NightDiffHours, a.Status, a.Issue, a.ResolutionNotes
            FROM Attendances a INNER JOIN Employees e ON e.Id = a.EmployeeId WHERE a.Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- RESOLVE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'RESOLVE'
    BEGIN
        BEGIN TRY
            DECLARE @ResCurSt INT;
            SELECT @ResCurSt = Status FROM Attendances WHERE Id = @Id AND IsDeleted = 0;
            IF @ResCurSt IS NULL BEGIN RAISERROR('Attendance record not found.', 16, 1); RETURN; END
            IF @ResCurSt <> 2 BEGIN RAISERROR('Attendance record is not in Review status.', 16, 1); RETURN; END

            IF @Resolution = 'adjust'
                UPDATE Attendances SET DaysWorked=ISNULL(@DaysWorked,DaysWorked), LateHours=ISNULL(@LateHours,LateHours),
                    UndertimeHours=ISNULL(@UndertimeHours,UndertimeHours), OtHours=ISNULL(@OtHours,OtHours),
                    NightDiffHours=ISNULL(@NightDiffHours,NightDiffHours), Status=1, Issue=NULL, ResolutionNotes=@Notes,
                    UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE() WHERE Id = @Id;
            ELSE
                UPDATE Attendances SET Status=1, Issue=NULL, ResolutionNotes=@Notes,
                    UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE() WHERE Id = @Id;

            SELECT a.Id, a.PayrollPeriodId, a.EmployeeId, e.EmployeeCode,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                a.DaysWorked, a.TotalDays, a.LateHours, a.UndertimeHours,
                a.OtHours, a.NightDiffHours, a.Status, a.Issue, a.ResolutionNotes
            FROM Attendances a INNER JOIN Employees e ON e.Id = a.EmployeeId WHERE a.Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Attendances WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Attendance record not found.', 16, 1); RETURN; END

            BEGIN TRANSACTION;
            UPDATE AttendanceDetails SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE AttendanceId=@Id AND IsDeleted=0;
            UPDATE Attendances SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_DETAILS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_DETAILS'
    BEGIN
        BEGIN TRY
            SELECT d.Id, d.AttendanceId, d.Date, d.TimeIn, d.TimeOut,
                d.LateHours, d.UndertimeHours, d.OtHours, d.NightDiffHours, d.Status, d.Remarks
            FROM AttendanceDetails d
            WHERE d.AttendanceId = @Id AND d.IsDeleted = 0
            ORDER BY d.Date;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE_DETAILS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_DETAILS'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Attendances WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Attendance record not found.', 16, 1); RETURN; END

            BEGIN TRANSACTION;

            MERGE AttendanceDetails AS tgt
            USING (
                SELECT
                    ISNULL(CAST(JSON_VALUE(j.value, '$.Id') AS INT), 0) AS DetailId,
                    CAST(JSON_VALUE(j.value, '$.Date') AS DATE) AS [Date],
                    CAST(JSON_VALUE(j.value, '$.TimeIn') AS TIME) AS TimeIn,
                    CAST(JSON_VALUE(j.value, '$.TimeOut') AS TIME) AS TimeOut,
                    ISNULL(CAST(JSON_VALUE(j.value, '$.LateHours') AS DECIMAL(18,2)), 0) AS LateHours,
                    ISNULL(CAST(JSON_VALUE(j.value, '$.UndertimeHours') AS DECIMAL(18,2)), 0) AS UndertimeHours,
                    ISNULL(CAST(JSON_VALUE(j.value, '$.OtHours') AS DECIMAL(18,2)), 0) AS OtHours,
                    ISNULL(CAST(JSON_VALUE(j.value, '$.NightDiffHours') AS DECIMAL(18,2)), 0) AS NightDiffHours,
                    ISNULL(JSON_VALUE(j.value, '$.Status'), 'Present') AS [Status],
                    JSON_VALUE(j.value, '$.Remarks') AS Remarks
                FROM OPENJSON(@DetailsJson) j
            ) AS src ON tgt.Id = src.DetailId AND src.DetailId > 0 AND tgt.AttendanceId = @Id
            WHEN MATCHED THEN UPDATE SET
                tgt.[Date]=src.[Date], tgt.TimeIn=src.TimeIn, tgt.TimeOut=src.TimeOut,
                tgt.LateHours=src.LateHours, tgt.UndertimeHours=src.UndertimeHours,
                tgt.OtHours=src.OtHours, tgt.NightDiffHours=src.NightDiffHours,
                tgt.[Status]=src.[Status], tgt.Remarks=src.Remarks,
                tgt.UpdatedBy=@UpdatedBy, tgt.UpdatedAt=GETDATE()
            WHEN NOT MATCHED THEN INSERT (AttendanceId,[Date],TimeIn,TimeOut,LateHours,UndertimeHours,OtHours,NightDiffHours,[Status],Remarks,CreatedAt,CreatedBy)
                VALUES (@Id,src.[Date],src.TimeIn,src.TimeOut,src.LateHours,src.UndertimeHours,src.OtHours,src.NightDiffHours,src.[Status],src.Remarks,GETDATE(),@UpdatedBy);

            UPDATE Attendances SET
                LateHours      = ISNULL((SELECT SUM(d.LateHours) FROM AttendanceDetails d WHERE d.AttendanceId=@Id AND d.IsDeleted=0), 0),
                UndertimeHours = ISNULL((SELECT SUM(d.UndertimeHours) FROM AttendanceDetails d WHERE d.AttendanceId=@Id AND d.IsDeleted=0), 0),
                OtHours        = ISNULL((SELECT SUM(d.OtHours) FROM AttendanceDetails d WHERE d.AttendanceId=@Id AND d.IsDeleted=0), 0),
                NightDiffHours = ISNULL((SELECT SUM(d.NightDiffHours) FROM AttendanceDetails d WHERE d.AttendanceId=@Id AND d.IsDeleted=0), 0),
                DaysWorked     = ISNULL((SELECT COUNT(*) FROM AttendanceDetails d WHERE d.AttendanceId=@Id AND d.IsDeleted=0 AND d.Status<>'Absent'), 0),
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @Id;

            COMMIT TRANSACTION;

            SELECT a.Id, a.PayrollPeriodId, a.EmployeeId, e.EmployeeCode,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                a.DaysWorked, a.TotalDays, a.LateHours, a.UndertimeHours,
                a.OtHours, a.NightDiffHours, a.Status, a.Issue, a.ResolutionNotes
            FROM Attendances a INNER JOIN Employees e ON e.Id = a.EmployeeId WHERE a.Id = @Id;

            SELECT d.Id, d.AttendanceId, d.Date, d.TimeIn, d.TimeOut,
                d.LateHours, d.UndertimeHours, d.OtHours, d.NightDiffHours, d.Status, d.Remarks
            FROM AttendanceDetails d WHERE d.AttendanceId=@Id AND d.IsDeleted=0 ORDER BY d.Date;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- IMPORT
    -- -----------------------------------------------------------------------
    IF @ActionType = 'IMPORT'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;
            DECLARE @ImpCreatedCount INT = 0;

            SELECT
                JSON_VALUE(j.value, '$.EmployeeCode') AS EmployeeCode,
                ISNULL(CAST(JSON_VALUE(j.value, '$.DaysWorked') AS DECIMAL(18,2)), 0) AS DaysWorked,
                ISNULL(CAST(JSON_VALUE(j.value, '$.TotalDays') AS DECIMAL(18,2)), 0) AS TotalDays,
                ISNULL(CAST(JSON_VALUE(j.value, '$.LateHours') AS DECIMAL(18,2)), 0) AS LateHours,
                ISNULL(CAST(JSON_VALUE(j.value, '$.UndertimeHours') AS DECIMAL(18,2)), 0) AS UndertimeHours,
                ISNULL(CAST(JSON_VALUE(j.value, '$.OtHours') AS DECIMAL(18,2)), 0) AS OtHours,
                ISNULL(CAST(JSON_VALUE(j.value, '$.NightDiffHours') AS DECIMAL(18,2)), 0) AS NightDiffHours,
                JSON_QUERY(j.value, '$.Details') AS DetailsJson,
                CAST(j.[key] AS INT) AS RowIdx
            INTO #ImportRows
            FROM OPENJSON(@RowsJson) j;

            DECLARE @ImpIdx INT = 0, @ImpMaxIdx INT = (SELECT ISNULL(MAX(RowIdx),-1) FROM #ImportRows);
            WHILE @ImpIdx <= @ImpMaxIdx
            BEGIN
                DECLARE @ImpEC NVARCHAR(50), @ImpDW DECIMAL(18,2), @ImpTD DECIMAL(18,2), @ImpLH DECIMAL(18,2), @ImpUH DECIMAL(18,2), @ImpOH DECIMAL(18,2), @ImpNH DECIMAL(18,2), @ImpDJ NVARCHAR(MAX);
                SELECT @ImpEC=EmployeeCode, @ImpDW=DaysWorked, @ImpTD=TotalDays, @ImpLH=LateHours, @ImpUH=UndertimeHours, @ImpOH=OtHours, @ImpNH=NightDiffHours, @ImpDJ=DetailsJson FROM #ImportRows WHERE RowIdx=@ImpIdx;

                DECLARE @ImpEId INT = NULL;
                SELECT @ImpEId = Id FROM Employees WHERE EmployeeCode = @ImpEC AND IsDeleted = 0;

                IF @ImpEId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Attendances WHERE EmployeeId=@ImpEId AND PayrollPeriodId=@PeriodId AND IsDeleted=0)
                BEGIN
                    DECLARE @ImpAS INT = CASE WHEN @ImpDW=0 THEN 3 WHEN @ImpDW<@ImpTD THEN 2 ELSE 1 END;
                    INSERT INTO Attendances (PayrollPeriodId,EmployeeId,DaysWorked,TotalDays,LateHours,UndertimeHours,OtHours,NightDiffHours,Status,CreatedAt,CreatedBy)
                    VALUES (@PeriodId,@ImpEId,@ImpDW,@ImpTD,@ImpLH,@ImpUH,@ImpOH,@ImpNH,@ImpAS,GETDATE(),@CreatedBy);
                    DECLARE @ImpAId INT = SCOPE_IDENTITY();

                    IF @ImpDJ IS NOT NULL AND LEN(@ImpDJ) > 2
                        INSERT INTO AttendanceDetails (AttendanceId,[Date],TimeIn,TimeOut,LateHours,UndertimeHours,OtHours,NightDiffHours,[Status],Remarks,CreatedAt,CreatedBy)
                        SELECT @ImpAId, CAST(JSON_VALUE(d.value,'$.Date') AS DATE), CAST(JSON_VALUE(d.value,'$.TimeIn') AS TIME), CAST(JSON_VALUE(d.value,'$.TimeOut') AS TIME),
                            ISNULL(CAST(JSON_VALUE(d.value,'$.LateHours') AS DECIMAL(18,2)),0), ISNULL(CAST(JSON_VALUE(d.value,'$.UndertimeHours') AS DECIMAL(18,2)),0),
                            ISNULL(CAST(JSON_VALUE(d.value,'$.OtHours') AS DECIMAL(18,2)),0), ISNULL(CAST(JSON_VALUE(d.value,'$.NightDiffHours') AS DECIMAL(18,2)),0),
                            ISNULL(JSON_VALUE(d.value,'$.Status'),'Present'), JSON_VALUE(d.value,'$.Remarks'), GETDATE(), @CreatedBy
                        FROM OPENJSON(@ImpDJ) d;

                    SET @ImpCreatedCount = @ImpCreatedCount + 1;
                END
                SET @ImpIdx = @ImpIdx + 1;
            END

            DROP TABLE #ImportRows;
            COMMIT TRANSACTION;
            SELECT @ImpCreatedCount AS CreatedCount;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#ImportRows') IS NOT NULL DROP TABLE #ImportRows;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- IMPORT_RAW_PUNCHES
    -- -----------------------------------------------------------------------
    IF @ActionType = 'IMPORT_RAW_PUNCHES'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @RpCreatedCount INT = 0;
            DECLARE @RpPeriodStart DATE, @RpPeriodEnd DATE;

            SELECT @RpPeriodStart = StartDate, @RpPeriodEnd = EndDate
            FROM PayrollPeriods WHERE Id = @PeriodId AND IsDeleted = 0;

            IF @RpPeriodStart IS NULL
            BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END

            DECLARE @RpGracePeriod INT = 15, @RpOTStartAfter INT = 0, @RpOTMinimum INT = 30;
            DECLARE @RpNDStart TIME = '22:00:00', @RpNDEnd TIME = '06:00:00';
            DECLARE @RpDefaultShiftStart TIME = '08:00:00', @RpDefaultShiftEnd TIME = '17:00:00';

            SELECT TOP 1
                @RpGracePeriod = GracePeriodMinutes,
                @RpOTStartAfter = OTStartAfterMinutes,
                @RpOTMinimum = OTMinimumMinutes,
                @RpNDStart = NightDiffStartTime,
                @RpNDEnd = NightDiffEndTime
            FROM ScheduleRules WHERE IsDeleted = 0;

            SELECT
                JSON_VALUE(p.value, '$.EmployeeCode') AS EmployeeCode,
                CAST(JSON_VALUE(p.value, '$.Timestamp') AS DATETIME) AS PunchTime,
                CAST(JSON_VALUE(p.value, '$.PunchType') AS INT) AS PunchType
            INTO #RpPunches
            FROM OPENJSON(@PunchesJson) p;

            SELECT DISTINCT EmployeeCode INTO #RpPunchEmployees FROM #RpPunches;

            ;WITH DateRange AS (
                SELECT @RpPeriodStart AS WorkDate
                UNION ALL
                SELECT DATEADD(DAY, 1, WorkDate) FROM DateRange WHERE WorkDate < @RpPeriodEnd
            )
            SELECT WorkDate, DATEPART(WEEKDAY, WorkDate) - 1 AS DayOfWeek
            INTO #RpWorkDays
            FROM DateRange
            OPTION (MAXRECURSION 366);

            DECLARE @RpEmpCode NVARCHAR(50), @RpEmpId INT, @RpSchedId INT;

            DECLARE rp_emp_cursor CURSOR LOCAL FAST_FORWARD FOR
                SELECT EmployeeCode FROM #RpPunchEmployees;

            OPEN rp_emp_cursor;
            FETCH NEXT FROM rp_emp_cursor INTO @RpEmpCode;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SET @RpEmpId = NULL;
                SELECT @RpEmpId = Id FROM Employees WHERE EmployeeCode = @RpEmpCode AND IsDeleted = 0 AND Status = 1;

                IF @RpEmpId IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM Attendances WHERE EmployeeId = @RpEmpId AND PayrollPeriodId = @PeriodId AND IsDeleted = 0)
                BEGIN
                    SET @RpSchedId = NULL;
                    SELECT TOP 1 @RpSchedId = WorkScheduleId
                    FROM EmployeeSchedules
                    WHERE EmployeeId = @RpEmpId AND EndDate IS NULL AND IsDeleted = 0
                    ORDER BY EffectiveDate DESC;

                    IF @RpSchedId IS NULL
                        SELECT TOP 1 @RpSchedId = Id FROM WorkSchedules WHERE IsDefault = 1 AND IsDeleted = 0;

                    DECLARE @RpTotalLate DECIMAL(18,2) = 0, @RpTotalUT DECIMAL(18,2) = 0;
                    DECLARE @RpTotalOT DECIMAL(18,2) = 0, @RpTotalND DECIMAL(18,2) = 0;
                    DECLARE @RpDaysPresent INT = 0, @RpTotalDaysCount INT = 0;

                    CREATE TABLE #RpEmpDetails (
                        [Date] DATE, TimeIn TIME, TimeOut TIME,
                        LateHours DECIMAL(18,2), UndertimeHours DECIMAL(18,2),
                        OtHours DECIMAL(18,2), NightDiffHours DECIMAL(18,2),
                        Status NVARCHAR(50)
                    );

                    DECLARE @RpWD DATE, @RpDOW INT;
                    DECLARE rp_day_cursor CURSOR LOCAL FAST_FORWARD FOR
                        SELECT WorkDate, DayOfWeek FROM #RpWorkDays;

                    OPEN rp_day_cursor;
                    FETCH NEXT FROM rp_day_cursor INTO @RpWD, @RpDOW;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        SET @RpTotalDaysCount = @RpTotalDaysCount + 1;

                        DECLARE @RpShiftStart TIME = @RpDefaultShiftStart, @RpShiftEnd TIME = @RpDefaultShiftEnd, @RpIsRestDay BIT = 0;

                        IF @RpSchedId IS NOT NULL
                        BEGIN
                            SELECT @RpShiftStart = ISNULL(ShiftStart, @RpDefaultShiftStart),
                                   @RpShiftEnd = ISNULL(ShiftEnd, @RpDefaultShiftEnd),
                                   @RpIsRestDay = ISNULL(IsRestDay, 0)
                            FROM WorkScheduleDays
                            WHERE WorkScheduleId = @RpSchedId AND DayOfWeek = @RpDOW AND IsDeleted = 0;
                        END

                        IF @RpIsRestDay = 1 OR @RpDOW = 0 OR @RpDOW = 6
                        BEGIN
                            INSERT INTO #RpEmpDetails ([Date], TimeIn, TimeOut, LateHours, UndertimeHours, OtHours, NightDiffHours, Status)
                            VALUES (@RpWD, NULL, NULL, 0, 0, 0, 0, 'Rest Day');
                        END
                        ELSE
                        BEGIN
                            DECLARE @RpTimeIn TIME = NULL, @RpTimeOut TIME = NULL;

                            SELECT @RpTimeIn = MIN(CAST(PunchTime AS TIME))
                            FROM #RpPunches WHERE EmployeeCode = @RpEmpCode AND CAST(PunchTime AS DATE) = @RpWD AND PunchType = 0;

                            SELECT @RpTimeOut = MAX(CAST(PunchTime AS TIME))
                            FROM #RpPunches WHERE EmployeeCode = @RpEmpCode AND CAST(PunchTime AS DATE) = @RpWD AND PunchType = 1;

                            IF @RpTimeIn IS NULL AND @RpTimeOut IS NULL
                            BEGIN
                                INSERT INTO #RpEmpDetails VALUES (@RpWD, NULL, NULL, 0, 0, 0, 0, 'Absent');
                            END
                            ELSE
                            BEGIN
                                DECLARE @RpLateMins DECIMAL(18,2) = 0, @RpUTMins DECIMAL(18,2) = 0;
                                DECLARE @RpOTMins DECIMAL(18,2) = 0, @RpNDMins DECIMAL(18,2) = 0;

                                IF @RpTimeIn IS NOT NULL AND @RpTimeIn > DATEADD(MINUTE, @RpGracePeriod, @RpShiftStart)
                                    SET @RpLateMins = DATEDIFF(MINUTE, @RpShiftStart, @RpTimeIn);

                                IF @RpTimeOut IS NOT NULL AND @RpTimeOut < @RpShiftEnd
                                    SET @RpUTMins = DATEDIFF(MINUTE, @RpTimeOut, @RpShiftEnd);

                                IF @RpTimeOut IS NOT NULL AND @RpTimeOut > DATEADD(MINUTE, @RpOTStartAfter, @RpShiftEnd)
                                BEGIN
                                    DECLARE @RpOTCandidate DECIMAL(18,2) = DATEDIFF(MINUTE, @RpShiftEnd, @RpTimeOut);
                                    IF @RpOTCandidate >= @RpOTMinimum
                                        SET @RpOTMins = @RpOTCandidate;
                                END

                                IF @RpTimeOut IS NOT NULL AND @RpTimeOut > @RpNDStart
                                    SET @RpNDMins = DATEDIFF(MINUTE, @RpNDStart, @RpTimeOut);

                                DECLARE @RpLateH DECIMAL(18,2) = ROUND(@RpLateMins / 60.0, 2);
                                DECLARE @RpUTH DECIMAL(18,2) = ROUND(@RpUTMins / 60.0, 2);
                                DECLARE @RpOTH DECIMAL(18,2) = ROUND(@RpOTMins / 60.0, 2);
                                DECLARE @RpNDH DECIMAL(18,2) = ROUND(@RpNDMins / 60.0, 2);

                                INSERT INTO #RpEmpDetails VALUES (@RpWD, @RpTimeIn, @RpTimeOut, @RpLateH, @RpUTH, @RpOTH, @RpNDH, 'Present');
                                SET @RpDaysPresent = @RpDaysPresent + 1;
                                SET @RpTotalLate = @RpTotalLate + @RpLateH;
                                SET @RpTotalUT = @RpTotalUT + @RpUTH;
                                SET @RpTotalOT = @RpTotalOT + @RpOTH;
                                SET @RpTotalND = @RpTotalND + @RpNDH;
                            END
                        END

                        FETCH NEXT FROM rp_day_cursor INTO @RpWD, @RpDOW;
                    END

                    CLOSE rp_day_cursor;
                    DEALLOCATE rp_day_cursor;

                    DECLARE @RpAttStatus INT = CASE
                        WHEN @RpDaysPresent = 0 THEN 3
                        WHEN @RpDaysPresent < @RpTotalDaysCount THEN 2
                        ELSE 1
                    END;

                    DECLARE @RpIssueText NVARCHAR(500) = NULL;
                    IF @RpAttStatus = 2
                        SET @RpIssueText = 'Days worked (' + CAST(@RpDaysPresent AS NVARCHAR) + ') is less than total days (' + CAST(@RpTotalDaysCount AS NVARCHAR) + ')';

                    INSERT INTO Attendances (PayrollPeriodId, EmployeeId, DaysWorked, TotalDays, LateHours, UndertimeHours, OtHours, NightDiffHours, Status, Issue, CreatedAt, CreatedBy)
                    VALUES (@PeriodId, @RpEmpId, @RpDaysPresent, @RpTotalDaysCount, @RpTotalLate, @RpTotalUT, @RpTotalOT, @RpTotalND, @RpAttStatus, @RpIssueText, GETDATE(), @CreatedBy);

                    DECLARE @RpAttId INT = SCOPE_IDENTITY();

                    INSERT INTO AttendanceDetails (AttendanceId, [Date], TimeIn, TimeOut, LateHours, UndertimeHours, OtHours, NightDiffHours, Status, CreatedAt, CreatedBy)
                    SELECT @RpAttId, [Date], TimeIn, TimeOut, LateHours, UndertimeHours, OtHours, NightDiffHours, Status, GETDATE(), @CreatedBy
                    FROM #RpEmpDetails ORDER BY [Date];

                    SET @RpCreatedCount = @RpCreatedCount + 1;

                    DROP TABLE #RpEmpDetails;
                END

                FETCH NEXT FROM rp_emp_cursor INTO @RpEmpCode;
            END

            CLOSE rp_emp_cursor;
            DEALLOCATE rp_emp_cursor;

            DROP TABLE #RpPunches;
            DROP TABLE #RpPunchEmployees;
            DROP TABLE #RpWorkDays;

            COMMIT TRANSACTION;
            SELECT @RpCreatedCount AS CreatedCount;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#RpPunches') IS NOT NULL DROP TABLE #RpPunches;
            IF OBJECT_ID('tempdb..#RpPunchEmployees') IS NOT NULL DROP TABLE #RpPunchEmployees;
            IF OBJECT_ID('tempdb..#RpWorkDays') IS NOT NULL DROP TABLE #RpWorkDays;
            IF OBJECT_ID('tempdb..#RpEmpDetails') IS NOT NULL DROP TABLE #RpEmpDetails;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CHECK_SCHEDULES
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CHECK_SCHEDULES'
    BEGIN
        BEGIN TRY
            SELECT
                e.EmployeeCode,
                e.LastName + ', ' + e.FirstName AS EmployeeName,
                CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM EmployeeSchedules es
                    WHERE es.EmployeeId = e.Id AND es.EndDate IS NULL AND es.IsDeleted = 0
                ) THEN 1 ELSE 0 END AS BIT) AS HasSchedule
            FROM Employees e
            INNER JOIN STRING_SPLIT(@EmployeeCodes, ',') s ON LTRIM(RTRIM(s.value)) = e.EmployeeCode
            WHERE e.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_Attendance: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 3. sp_Leave
-- ============================================================================
CREATE PROCEDURE dbo.sp_Leave
    @ActionType    VARCHAR(50),
    @Id            INT = NULL,
    @Search        NVARCHAR(200) = NULL,
    @Status        INT = NULL,
    @LeaveType     NVARCHAR(100) = NULL,
    @Employee      NVARCHAR(256) = NULL,
    @StartDate     DATE = NULL,
    @EndDate       DATE = NULL,
    @Reason        NVARCHAR(1000) = NULL,
    @Approver      NVARCHAR(256) = NULL,
    @Remarks       NVARCHAR(1000) = NULL,
    @EmployeeCode  NVARCHAR(50) = NULL,
    @EmployeeName  NVARCHAR(256) = NULL,
    @Entitlement   INT = NULL,
    @Used          INT = NULL,
    @Pending       INT = NULL,
    @CarryOver     INT = NULL,
    @Year          INT = NULL,
    @Name          NVARCHAR(256) = NULL,
    @Date          DATE = NULL,
    @Type          INT = NULL,
    @Region        NVARCHAR(100) = NULL,
    @IsRecurring   BIT = NULL,
    @CreatedBy     NVARCHAR(256) = NULL,
    @UpdatedBy     NVARCHAR(256) = NULL,
    @DeletedBy     NVARCHAR(256) = NULL,
    @ProcessedBy   NVARCHAR(256) = NULL,
    @RolledBackBy  NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- -----------------------------------------------------------------------
    -- GET_APPLICATIONS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_APPLICATIONS'
    BEGIN
        BEGIN TRY
            SELECT Id, ReferenceNumber, EmployeeName, LeaveType, StartDate, EndDate,
                DeductibleDays, Reason, Status, SubmittedOn, ApproverName, ApproverRemarks,
                CreatedAt, CreatedBy
            FROM LeaveApplications
            WHERE IsDeleted = 0
              AND (@Search IS NULL OR EmployeeName LIKE '%' + @Search + '%' OR ReferenceNumber LIKE '%' + @Search + '%')
              AND (@Status IS NULL OR Status = @Status)
              AND (@LeaveType IS NULL OR LeaveType = @LeaveType)
            ORDER BY SubmittedOn DESC;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE_APPLICATION
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE_APPLICATION'
    BEGIN
        BEGIN TRY
            IF @EndDate < @StartDate
            BEGIN RAISERROR('End date must be on or after start date.', 16, 1); RETURN; END

            DECLARE @CaYear INT = YEAR(GETDATE());
            DECLARE @CaCount INT = (SELECT COUNT(*) FROM LeaveApplications WHERE YEAR(CreatedAt) = @CaYear) + 1;
            DECLARE @CaRefNo NVARCHAR(20) = 'LVA-' + CAST(@CaYear AS NVARCHAR) + '-' + RIGHT('00000' + CAST(@CaCount AS NVARCHAR), 5);

            DECLARE @CaDays INT = DATEDIFF(DAY, @StartDate, @EndDate) + 1;

            INSERT INTO LeaveApplications (ReferenceNumber, EmployeeName, LeaveType, StartDate, EndDate,
                DeductibleDays, Reason, Status, SubmittedOn, ApproverName, CreatedAt, CreatedBy)
            VALUES (@CaRefNo, @Employee, @LeaveType, @StartDate, @EndDate,
                @CaDays, @Reason, 1, GETDATE(), @Approver, GETDATE(), @CreatedBy);

            SELECT * FROM LeaveApplications WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE_APPLICATION_STATUS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_APPLICATION_STATUS'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM LeaveApplications WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Leave application not found.', 16, 1); RETURN; END

            UPDATE LeaveApplications
            SET Status = @Status, ApproverRemarks = @Remarks, UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE()
            WHERE Id = @Id;

            SELECT * FROM LeaveApplications WHERE Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_BALANCES
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_BALANCES'
    BEGIN
        BEGIN TRY
            SELECT Id, EmployeeCode, EmployeeName, LeaveType, Entitlement, Used, Pending, CarryOver,
                (Entitlement + CarryOver - Used - Pending) AS Remaining,
                CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
            FROM LeaveBalances
            WHERE IsDeleted = 0
              AND (@Search IS NULL OR EmployeeName LIKE '%' + @Search + '%' OR EmployeeCode LIKE '%' + @Search + '%')
              AND (@LeaveType IS NULL OR LeaveType = @LeaveType)
            ORDER BY EmployeeName, LeaveType;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE_BALANCE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE_BALANCE'
    BEGIN
        BEGIN TRY
            IF EXISTS (SELECT 1 FROM LeaveBalances WHERE EmployeeCode = @EmployeeCode AND LeaveType = @LeaveType AND IsDeleted = 0)
            BEGIN RAISERROR('Balance already exists for this employee and leave type.', 16, 1); RETURN; END

            INSERT INTO LeaveBalances (EmployeeCode, EmployeeName, LeaveType, Entitlement, Used, Pending, CarryOver, CreatedAt, CreatedBy)
            VALUES (@EmployeeCode, @EmployeeName, @LeaveType, @Entitlement, 0, 0, ISNULL(@CarryOver, 0), GETDATE(), @CreatedBy);

            SELECT *, (Entitlement + CarryOver - Used - Pending) AS Remaining FROM LeaveBalances WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE_BALANCE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_BALANCE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM LeaveBalances WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Leave balance not found.', 16, 1); RETURN; END

            UPDATE LeaveBalances SET Entitlement=@Entitlement, Used=@Used, Pending=@Pending, CarryOver=@CarryOver,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @Id;

            SELECT *, (Entitlement + CarryOver - Used - Pending) AS Remaining FROM LeaveBalances WHERE Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE_BALANCE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE_BALANCE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM LeaveBalances WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Leave balance not found.', 16, 1); RETURN; END

            UPDATE LeaveBalances SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- ENROLL_ALL
    -- -----------------------------------------------------------------------
    IF @ActionType = 'ENROLL_ALL'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            INSERT INTO LeaveBalances (EmployeeCode, EmployeeName, LeaveType, Entitlement, Used, Pending, CarryOver, CreatedAt, CreatedBy)
            SELECT e.EmployeeCode, e.LastName + ', ' + e.FirstName, @LeaveType, @Entitlement, 0, 0, 0, GETDATE(), @CreatedBy
            FROM Employees e
            WHERE e.Status = 1 AND e.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM LeaveBalances lb WHERE lb.EmployeeCode = e.EmployeeCode AND lb.LeaveType = @LeaveType AND lb.IsDeleted = 0);

            DECLARE @EnrollCount INT = @@ROWCOUNT;
            COMMIT TRANSACTION;
            SELECT @EnrollCount AS CreatedCount;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- RUN_YEAR_END
    -- -----------------------------------------------------------------------
    IF @ActionType = 'RUN_YEAR_END'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF EXISTS (SELECT 1 FROM LeaveYearEndBatches WHERE Year >= @Year AND Status = 1 AND IsDeleted = 0)
            BEGIN RAISERROR('A year-end batch already exists for this year or later.', 16, 1); RETURN; END

            DECLARE @YeSnapshot NVARCHAR(MAX);
            SET @YeSnapshot = (
                SELECT Id, EmployeeCode, EmployeeName, LeaveType, Entitlement, Used, Pending, CarryOver,
                    IsDeleted, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM LeaveBalances WHERE IsDeleted = 0
                FOR JSON PATH
            );

            DECLARE @YeEmployeesProcessed INT = 0, @YeBalancesCreated INT = 0;
            DECLARE @YeBalancesExpired INT = 0, @YeCarryForwardsApplied INT = 0;

            INSERT INTO LeaveBalances (EmployeeCode, EmployeeName, LeaveType, Entitlement, Used, Pending, CarryOver, CreatedAt, CreatedBy)
            SELECT
                lb.EmployeeCode,
                lb.EmployeeName,
                lb.LeaveType,
                ISNULL(lt.DefaultDaysPerYear, lb.Entitlement),
                0,
                0,
                CASE
                    WHEN lt.CarryForwardPolicy = 3 THEN
                        CASE WHEN (lb.Entitlement + lb.CarryOver - lb.Used - lb.Pending) > 0
                             THEN (lb.Entitlement + lb.CarryOver - lb.Used - lb.Pending) ELSE 0 END
                    WHEN lt.CarryForwardPolicy = 2 THEN
                        CASE WHEN (lb.Entitlement + lb.CarryOver - lb.Used - lb.Pending) > ISNULL(lt.CarryForwardMaxDays, 0)
                             THEN CAST(ISNULL(lt.CarryForwardMaxDays, 0) AS INT)
                             WHEN (lb.Entitlement + lb.CarryOver - lb.Used - lb.Pending) > 0
                             THEN (lb.Entitlement + lb.CarryOver - lb.Used - lb.Pending)
                             ELSE 0 END
                    ELSE 0
                END,
                GETDATE(),
                @ProcessedBy
            FROM LeaveBalances lb
            LEFT JOIN LeaveTypes lt ON lt.Name = lb.LeaveType AND lt.IsActive = 1 AND lt.IsDeleted = 0
            WHERE lb.IsDeleted = 0;

            SET @YeBalancesCreated = @@ROWCOUNT;

            SELECT @YeCarryForwardsApplied = COUNT(*)
            FROM LeaveBalances lb
            LEFT JOIN LeaveTypes lt ON lt.Name = lb.LeaveType AND lt.IsActive = 1 AND lt.IsDeleted = 0
            WHERE lb.IsDeleted = 0 AND lb.CreatedBy = @ProcessedBy
              AND lb.CarryOver > 0;

            SELECT @YeBalancesExpired = COUNT(*)
            FROM LeaveBalances lb
            LEFT JOIN LeaveTypes lt ON lt.Name = lb.LeaveType AND lt.IsActive = 1 AND lt.IsDeleted = 0
            WHERE lb.IsDeleted = 0 AND lb.CreatedBy <> @ProcessedBy
              AND ISNULL(lt.CarryForwardPolicy, 1) = 1
              AND (lb.Entitlement + lb.CarryOver - lb.Used - lb.Pending) > 0;

            SELECT @YeEmployeesProcessed = COUNT(DISTINCT EmployeeCode)
            FROM LeaveBalances WHERE IsDeleted = 0 AND CreatedBy = @ProcessedBy;

            UPDATE LeaveBalances SET IsDeleted = 1, DeletedAt = GETDATE(), DeletedBy = @ProcessedBy
            WHERE IsDeleted = 0 AND CreatedBy <> @ProcessedBy;

            INSERT INTO LeaveYearEndBatches (Year, ProcessedAt, ProcessedBy, EmployeesProcessed, BalancesCreated,
                BalancesExpired, CarryForwardsApplied, Status, SnapshotJson, CreatedAt, CreatedBy)
            VALUES (@Year, GETDATE(), @ProcessedBy, @YeEmployeesProcessed, @YeBalancesCreated,
                @YeBalancesExpired, @YeCarryForwardsApplied, 1, @YeSnapshot, GETDATE(), @ProcessedBy);

            COMMIT TRANSACTION;

            SELECT @Year AS Year, @YeEmployeesProcessed AS EmployeesProcessed,
                @YeBalancesCreated AS BalancesCreated, @YeBalancesExpired AS BalancesExpired,
                @YeCarryForwardsApplied AS CarryForwardsApplied,
                'Year-end processing completed successfully.' AS Message;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- ROLLBACK_YEAR_END
    -- -----------------------------------------------------------------------
    IF @ActionType = 'ROLLBACK_YEAR_END'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @RbBatchId INT, @RbSnapshot NVARCHAR(MAX), @RbBatchCreatedBy NVARCHAR(256);

            SELECT TOP 1 @RbBatchId = Id, @RbSnapshot = SnapshotJson, @RbBatchCreatedBy = ProcessedBy
            FROM LeaveYearEndBatches
            WHERE Status = 1 AND IsDeleted = 0
            ORDER BY ProcessedAt DESC;

            IF @RbBatchId IS NULL
            BEGIN RAISERROR('No completed year-end batch found to rollback.', 16, 1); RETURN; END

            DELETE FROM LeaveBalances WHERE CreatedBy = @RbBatchCreatedBy AND IsDeleted = 0;

            SET IDENTITY_INSERT LeaveBalances ON;

            INSERT INTO LeaveBalances (Id, EmployeeCode, EmployeeName, LeaveType, Entitlement, Used, Pending, CarryOver,
                IsDeleted, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
            SELECT
                CAST(JSON_VALUE(j.value, '$.Id') AS INT),
                JSON_VALUE(j.value, '$.EmployeeCode'),
                JSON_VALUE(j.value, '$.EmployeeName'),
                JSON_VALUE(j.value, '$.LeaveType'),
                CAST(JSON_VALUE(j.value, '$.Entitlement') AS INT),
                CAST(JSON_VALUE(j.value, '$.Used') AS INT),
                CAST(JSON_VALUE(j.value, '$.Pending') AS INT),
                CAST(JSON_VALUE(j.value, '$.CarryOver') AS INT),
                0,
                CAST(JSON_VALUE(j.value, '$.CreatedAt') AS DATETIME),
                JSON_VALUE(j.value, '$.CreatedBy'),
                JSON_VALUE(j.value, '$.UpdatedAt'),
                JSON_VALUE(j.value, '$.UpdatedBy')
            FROM OPENJSON(@RbSnapshot) j;

            SET IDENTITY_INSERT LeaveBalances OFF;

            UPDATE LeaveYearEndBatches SET Status = 2, RolledBackAt = GETDATE(), RolledBackBy = @RolledBackBy,
                UpdatedBy = @RolledBackBy, UpdatedAt = GETDATE()
            WHERE Id = @RbBatchId;

            COMMIT TRANSACTION;

            SELECT 'Year-end batch rolled back successfully.' AS Message;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_LAST_BATCH
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_LAST_BATCH'
    BEGIN
        BEGIN TRY
            SELECT TOP 1 * FROM LeaveYearEndBatches WHERE Status = 1 AND IsDeleted = 0 ORDER BY ProcessedAt DESC;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_HOLIDAYS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_HOLIDAYS'
    BEGIN
        BEGIN TRY
            SELECT Id, Name, Date, Type, Region, IsRecurring, CreatedAt, CreatedBy
            FROM Holidays
            WHERE IsDeleted = 0
              AND (@Search IS NULL OR Name LIKE '%' + @Search + '%')
              AND (@Type IS NULL OR Type = @Type)
            ORDER BY Date;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE_HOLIDAY
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE_HOLIDAY'
    BEGIN
        BEGIN TRY
            IF @Type NOT IN (1,2,3,4)
            BEGIN RAISERROR('Invalid holiday type.', 16, 1); RETURN; END

            INSERT INTO Holidays (Name, Date, Type, Region, IsRecurring, CreatedAt, CreatedBy)
            VALUES (@Name, @Date, @Type, @Region, @IsRecurring, GETDATE(), @CreatedBy);

            SELECT * FROM Holidays WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE_HOLIDAY
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE_HOLIDAY'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Holidays WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Holiday not found.', 16, 1); RETURN; END

            UPDATE Holidays SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_Leave: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 4. sp_Employee
-- ============================================================================
CREATE PROCEDURE dbo.sp_Employee
    @ActionType              VARCHAR(50),
    @Id                      INT = NULL,
    @Page                    INT = 1,
    @PageSize                INT = 10,
    @Search                  NVARCHAR(200) = NULL,
    @DepartmentId            INT = NULL,
    @StatusFilter            INT = NULL,
    @EmployeeCode            NVARCHAR(50) = NULL,
    @FirstName               NVARCHAR(100) = NULL,
    @MiddleName              NVARCHAR(100) = NULL,
    @LastName                NVARCHAR(100) = NULL,
    @Suffix                  NVARCHAR(20) = NULL,
    @DateOfBirth             DATETIME = NULL,
    @Gender                  INT = 0,
    @MaritalStatus           INT = 0,
    @TaxIdentificationNumber NVARCHAR(50) = NULL,
    @SssNumber               NVARCHAR(50) = NULL,
    @PhilHealthNumber        NVARCHAR(50) = NULL,
    @PagIbigNumber           NVARCHAR(50) = NULL,
    @Email                   NVARCHAR(256) = NULL,
    @PersonalEmail           NVARCHAR(256) = NULL,
    @MobileNumber            NVARCHAR(50) = NULL,
    @AlternatePhone          NVARCHAR(50) = NULL,
    @PresentAddress          NVARCHAR(500) = NULL,
    @PresentCity             NVARCHAR(100) = NULL,
    @PresentProvince         NVARCHAR(100) = NULL,
    @PresentZipCode          NVARCHAR(20) = NULL,
    @SameAsPresentAddress    BIT = 0,
    @PermanentAddress        NVARCHAR(500) = NULL,
    @PermanentCity           NVARCHAR(100) = NULL,
    @PermanentProvince       NVARCHAR(100) = NULL,
    @PermanentZipCode        NVARCHAR(20) = NULL,
    @DepartmentIdParam       INT = NULL,
    @PositionId              INT = NULL,
    @ManagerId               INT = NULL,
    @BranchId                INT = NULL,
    @EmploymentType          INT = 1,
    @HireDate                DATETIME = NULL,
    @ProbationEndDate        DATETIME = NULL,
    @RegularizationDate      DATETIME = NULL,
    @BasicSalary             DECIMAL(18,2) = 0,
    @SalaryFrequency         INT = 0,
    @SalaryEffectiveDate     DATETIME = NULL,
    @BankAccountNumber       NVARCHAR(50) = NULL,
    @BankName                NVARCHAR(100) = NULL,
    @ProfilePhotoPath        NVARCHAR(500) = NULL,
    @BiometricId             NVARCHAR(50) = NULL,
    @NewStatus               INT = NULL,
    @Remarks                 NVARCHAR(500) = NULL,
    @LastWorkingDate         DATETIME = NULL,
    @ContactId               INT = NULL,
    @ContactName             NVARCHAR(256) = NULL,
    @Relationship            NVARCHAR(100) = NULL,
    @Phone                   NVARCHAR(50) = NULL,
    @ContactEmail            NVARCHAR(256) = NULL,
    @Address                 NVARCHAR(500) = NULL,
    @IsPrimary               BIT = 0,
    @DocumentId              INT = NULL,
    @DocumentType            INT = NULL,
    @DocumentName            NVARCHAR(256) = NULL,
    @FilePath                NVARCHAR(500) = NULL,
    @DocumentNotes           NVARCHAR(500) = NULL,
    @ExpiryDate              DATETIME = NULL,
    @CreatedBy               NVARCHAR(256) = NULL,
    @UpdatedBy               NVARCHAR(256) = NULL,
    @DeletedBy               NVARCHAR(256) = NULL,
    @ChangedBy               NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- -----------------------------------------------------------------------
    -- GET_PAGED
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_PAGED'
    BEGIN
        BEGIN TRY
            DECLARE @GpOffset INT = (@Page - 1) * @PageSize;
            DECLARE @GpTotalCount INT;

            SELECT @GpTotalCount = COUNT(*)
            FROM Employees e
            WHERE e.IsDeleted = 0
              AND (@Search IS NULL
                   OR e.FirstName LIKE '%' + @Search + '%'
                   OR e.LastName LIKE '%' + @Search + '%'
                   OR e.Email LIKE '%' + @Search + '%'
                   OR e.EmployeeCode LIKE '%' + @Search + '%'
                   OR e.MobileNumber LIKE '%' + @Search + '%')
              AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
              AND (@StatusFilter IS NULL OR e.Status = @StatusFilter);

            SELECT
                e.Id, e.EmployeeCode, e.FirstName, e.MiddleName, e.LastName, e.Suffix,
                e.Email, e.MobileNumber, e.Status,
                e.DepartmentId, d.DepartmentName,
                e.PositionId, p.PositionTitle AS PositionName,
                e.ManagerId, ISNULL(m.LastName + ', ' + m.FirstName, NULL) AS ManagerName,
                e.BranchId, b.BranchName,
                e.HireDate, e.BasicSalary, e.EmploymentType, e.ProfilePhotoPath,
                e.LastName + ', ' + e.FirstName AS FullName
            FROM Employees e
            LEFT JOIN Departments d ON d.Id = e.DepartmentId
            LEFT JOIN Positions p ON p.Id = e.PositionId
            LEFT JOIN Employees m ON m.Id = e.ManagerId
            LEFT JOIN Branches b ON b.Id = e.BranchId
            WHERE e.IsDeleted = 0
              AND (@Search IS NULL
                   OR e.FirstName LIKE '%' + @Search + '%'
                   OR e.LastName LIKE '%' + @Search + '%'
                   OR e.Email LIKE '%' + @Search + '%'
                   OR e.EmployeeCode LIKE '%' + @Search + '%'
                   OR e.MobileNumber LIKE '%' + @Search + '%')
              AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
              AND (@StatusFilter IS NULL OR e.Status = @StatusFilter)
            ORDER BY e.LastName, e.FirstName
            OFFSET @GpOffset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT @GpTotalCount AS TotalCount;
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
                e.Id, e.EmployeeCode, e.FirstName, e.MiddleName, e.LastName, e.Suffix,
                e.Email, e.MobileNumber, e.Status,
                e.DepartmentId, d.DepartmentName,
                e.PositionId, p.PositionTitle AS PositionName,
                e.ManagerId, ISNULL(m.LastName + ', ' + m.FirstName, NULL) AS ManagerName,
                e.BranchId, b.BranchName,
                e.HireDate, e.BasicSalary, e.EmploymentType, e.ProfilePhotoPath,
                e.LastName + ', ' + e.FirstName AS FullName,
                e.DateOfBirth, e.Gender, e.MaritalStatus,
                e.TaxIdentificationNumber, e.SssNumber, e.PhilHealthNumber, e.PagIbigNumber,
                e.PersonalEmail, e.AlternatePhone,
                e.PresentAddress, e.PresentCity, e.PresentProvince, e.PresentZipCode,
                e.SameAsPresentAddress, e.PermanentAddress, e.PermanentCity, e.PermanentProvince, e.PermanentZipCode,
                e.TerminationDate, e.ProbationEndDate, e.RegularizationDate, e.LastWorkingDate,
                e.SalaryFrequency, e.SalaryEffectiveDate, e.BankAccountNumber, e.BankName,
                e.SssContribution, e.PhilHealthContribution, e.PagIbigContribution,
                e.BiometricId, e.StatusRemarks, e.StatusChangedAt, e.StatusChangedBy,
                e.CreatedAt, e.CreatedBy, e.UpdatedAt, e.UpdatedBy
            FROM Employees e
            LEFT JOIN Departments d ON d.Id = e.DepartmentId
            LEFT JOIN Positions p ON p.Id = e.PositionId
            LEFT JOIN Employees m ON m.Id = e.ManagerId
            LEFT JOIN Branches b ON b.Id = e.BranchId
            WHERE e.Id = @Id AND e.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_BY_CODE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_BY_CODE'
    BEGIN
        BEGIN TRY
            DECLARE @GbcEmpId INT;
            SELECT @GbcEmpId = Id FROM Employees WHERE EmployeeCode = @EmployeeCode AND IsDeleted = 0;
            IF @GbcEmpId IS NULL
            BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END

            -- Inline the GET_BY_ID query
            SELECT
                e.Id, e.EmployeeCode, e.FirstName, e.MiddleName, e.LastName, e.Suffix,
                e.Email, e.MobileNumber, e.Status,
                e.DepartmentId, d.DepartmentName,
                e.PositionId, p.PositionTitle AS PositionName,
                e.ManagerId, ISNULL(m.LastName + ', ' + m.FirstName, NULL) AS ManagerName,
                e.BranchId, b.BranchName,
                e.HireDate, e.BasicSalary, e.EmploymentType, e.ProfilePhotoPath,
                e.LastName + ', ' + e.FirstName AS FullName,
                e.DateOfBirth, e.Gender, e.MaritalStatus,
                e.TaxIdentificationNumber, e.SssNumber, e.PhilHealthNumber, e.PagIbigNumber,
                e.PersonalEmail, e.AlternatePhone,
                e.PresentAddress, e.PresentCity, e.PresentProvince, e.PresentZipCode,
                e.SameAsPresentAddress, e.PermanentAddress, e.PermanentCity, e.PermanentProvince, e.PermanentZipCode,
                e.TerminationDate, e.ProbationEndDate, e.RegularizationDate, e.LastWorkingDate,
                e.SalaryFrequency, e.SalaryEffectiveDate, e.BankAccountNumber, e.BankName,
                e.SssContribution, e.PhilHealthContribution, e.PagIbigContribution,
                e.BiometricId, e.StatusRemarks, e.StatusChangedAt, e.StatusChangedBy,
                e.CreatedAt, e.CreatedBy, e.UpdatedAt, e.UpdatedBy
            FROM Employees e
            LEFT JOIN Departments d ON d.Id = e.DepartmentId
            LEFT JOIN Positions p ON p.Id = e.PositionId
            LEFT JOIN Employees m ON m.Id = e.ManagerId
            LEFT JOIN Branches b ON b.Id = e.BranchId
            WHERE e.Id = @GbcEmpId AND e.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = @EmployeeCode AND IsDeleted = 0)
            BEGIN RAISERROR('Employee code already exists.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email AND IsDeleted = 0)
            BEGIN RAISERROR('Email already exists.', 16, 1); RETURN; END

            IF @ManagerId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Employees WHERE Id = @ManagerId AND Status = 1 AND IsDeleted = 0)
            BEGIN RAISERROR('Manager not found or not active.', 16, 1); RETURN; END

            IF @BranchId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Branches WHERE Id = @BranchId AND IsDeleted = 0)
            BEGIN RAISERROR('Branch not found.', 16, 1); RETURN; END

            DECLARE @CrDefSss DECIMAL(18,2) = 900, @CrDefPH DECIMAL(18,2) = 500, @CrDefPI DECIMAL(18,2) = 200;
            SELECT TOP 1 @CrDefSss = DefaultSssContribution, @CrDefPH = DefaultPhilHealthContribution, @CrDefPI = DefaultPagIbigContribution
            FROM CompanySettings WHERE IsDeleted = 0;

            IF @SameAsPresentAddress = 1
            BEGIN
                SET @PermanentAddress = @PresentAddress;
                SET @PermanentCity = @PresentCity;
                SET @PermanentProvince = @PresentProvince;
                SET @PermanentZipCode = @PresentZipCode;
            END

            DECLARE @CrDeptId INT = ISNULL(@DepartmentIdParam, @DepartmentId);

            INSERT INTO Employees (
                EmployeeCode, FirstName, MiddleName, LastName, Suffix, DateOfBirth, Gender, MaritalStatus,
                TaxIdentificationNumber, SssNumber, PhilHealthNumber, PagIbigNumber,
                Email, PersonalEmail, MobileNumber, AlternatePhone,
                PresentAddress, PresentCity, PresentProvince, PresentZipCode,
                SameAsPresentAddress, PermanentAddress, PermanentCity, PermanentProvince, PermanentZipCode,
                DepartmentId, PositionId, ManagerId, BranchId, EmploymentType,
                HireDate, ProbationEndDate, RegularizationDate,
                BasicSalary, SalaryFrequency, SalaryEffectiveDate, BankAccountNumber, BankName,
                SssContribution, PhilHealthContribution, PagIbigContribution,
                ProfilePhotoPath, BiometricId, Status,
                CreatedAt, CreatedBy
            )
            VALUES (
                @EmployeeCode, @FirstName, @MiddleName, @LastName, @Suffix, @DateOfBirth, @Gender, @MaritalStatus,
                ISNULL(@TaxIdentificationNumber,''), ISNULL(@SssNumber,''), ISNULL(@PhilHealthNumber,''), ISNULL(@PagIbigNumber,''),
                @Email, @PersonalEmail, ISNULL(@MobileNumber,''), @AlternatePhone,
                ISNULL(@PresentAddress,''), ISNULL(@PresentCity,''), ISNULL(@PresentProvince,''), ISNULL(@PresentZipCode,''),
                @SameAsPresentAddress, @PermanentAddress, @PermanentCity, @PermanentProvince, @PermanentZipCode,
                ISNULL(@CrDeptId, 0), ISNULL(@PositionId, 0), @ManagerId, @BranchId, @EmploymentType,
                ISNULL(@HireDate, GETDATE()), @ProbationEndDate, @RegularizationDate,
                @BasicSalary, @SalaryFrequency, ISNULL(@SalaryEffectiveDate, GETDATE()), ISNULL(@BankAccountNumber,''), ISNULL(@BankName,''),
                @CrDefSss, @CrDefPH, @CrDefPI,
                @ProfilePhotoPath, @BiometricId, 1,
                GETDATE(), @CreatedBy
            );

            DECLARE @CrNewId INT = SCOPE_IDENTITY();

            INSERT INTO SalaryHistory (EmployeeId, PreviousSalary, NewSalary, SalaryFrequency, EffectiveDate, ChangedBy, ChangedAt, CreatedAt, CreatedBy)
            VALUES (@CrNewId, 0, @BasicSalary, @SalaryFrequency, ISNULL(@SalaryEffectiveDate, GETDATE()), @CreatedBy, GETDATE(), GETDATE(), @CreatedBy);

            COMMIT TRANSACTION;

            -- Return created employee using inline query
            SELECT
                e.Id, e.EmployeeCode, e.FirstName, e.MiddleName, e.LastName, e.Suffix,
                e.Email, e.MobileNumber, e.Status,
                e.DepartmentId, d.DepartmentName,
                e.PositionId, p.PositionTitle AS PositionName,
                e.ManagerId, ISNULL(mg.LastName + ', ' + mg.FirstName, NULL) AS ManagerName,
                e.BranchId, b.BranchName,
                e.HireDate, e.BasicSalary, e.EmploymentType, e.ProfilePhotoPath,
                e.LastName + ', ' + e.FirstName AS FullName,
                e.DateOfBirth, e.Gender, e.MaritalStatus,
                e.TaxIdentificationNumber, e.SssNumber, e.PhilHealthNumber, e.PagIbigNumber,
                e.PersonalEmail, e.AlternatePhone,
                e.PresentAddress, e.PresentCity, e.PresentProvince, e.PresentZipCode,
                e.SameAsPresentAddress, e.PermanentAddress, e.PermanentCity, e.PermanentProvince, e.PermanentZipCode,
                e.TerminationDate, e.ProbationEndDate, e.RegularizationDate, e.LastWorkingDate,
                e.SalaryFrequency, e.SalaryEffectiveDate, e.BankAccountNumber, e.BankName,
                e.SssContribution, e.PhilHealthContribution, e.PagIbigContribution,
                e.BiometricId, e.StatusRemarks, e.StatusChangedAt, e.StatusChangedBy,
                e.CreatedAt, e.CreatedBy, e.UpdatedAt, e.UpdatedBy
            FROM Employees e
            LEFT JOIN Departments d ON d.Id = e.DepartmentId
            LEFT JOIN Positions p ON p.Id = e.PositionId
            LEFT JOIN Employees mg ON mg.Id = e.ManagerId
            LEFT JOIN Branches b ON b.Id = e.BranchId
            WHERE e.Id = @CrNewId;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF NOT EXISTS (SELECT 1 FROM Employees WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email AND Id <> @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Email already exists.', 16, 1); RETURN; END

            IF @ManagerId IS NOT NULL
            BEGIN
                IF @ManagerId = @Id BEGIN RAISERROR('Employee cannot be their own manager.', 16, 1); RETURN; END
                IF NOT EXISTS (SELECT 1 FROM Employees WHERE Id = @ManagerId AND Status = 1 AND IsDeleted = 0)
                BEGIN RAISERROR('Manager not found or not active.', 16, 1); RETURN; END
            END

            IF @BranchId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Branches WHERE Id = @BranchId AND IsDeleted = 0)
            BEGIN RAISERROR('Branch not found.', 16, 1); RETURN; END

            DECLARE @UpOldSalary DECIMAL(18,2);
            SELECT @UpOldSalary = BasicSalary FROM Employees WHERE Id = @Id;
            IF @UpOldSalary <> @BasicSalary
            BEGIN
                INSERT INTO SalaryHistory (EmployeeId, PreviousSalary, NewSalary, SalaryFrequency, EffectiveDate, ChangedBy, ChangedAt, CreatedAt, CreatedBy)
                VALUES (@Id, @UpOldSalary, @BasicSalary, @SalaryFrequency, ISNULL(@SalaryEffectiveDate, GETDATE()), @UpdatedBy, GETDATE(), GETDATE(), @UpdatedBy);
            END

            IF @SameAsPresentAddress = 1
            BEGIN
                SET @PermanentAddress = @PresentAddress;
                SET @PermanentCity = @PresentCity;
                SET @PermanentProvince = @PresentProvince;
                SET @PermanentZipCode = @PresentZipCode;
            END

            DECLARE @UpDeptId INT = ISNULL(@DepartmentIdParam, @DepartmentId);

            UPDATE Employees SET
                FirstName=@FirstName, MiddleName=@MiddleName, LastName=@LastName, Suffix=@Suffix,
                DateOfBirth=@DateOfBirth, Gender=@Gender, MaritalStatus=@MaritalStatus,
                TaxIdentificationNumber=ISNULL(@TaxIdentificationNumber,''), SssNumber=ISNULL(@SssNumber,''),
                PhilHealthNumber=ISNULL(@PhilHealthNumber,''), PagIbigNumber=ISNULL(@PagIbigNumber,''),
                Email=@Email, PersonalEmail=@PersonalEmail, MobileNumber=ISNULL(@MobileNumber,''), AlternatePhone=@AlternatePhone,
                PresentAddress=ISNULL(@PresentAddress,''), PresentCity=ISNULL(@PresentCity,''),
                PresentProvince=ISNULL(@PresentProvince,''), PresentZipCode=ISNULL(@PresentZipCode,''),
                SameAsPresentAddress=@SameAsPresentAddress,
                PermanentAddress=@PermanentAddress, PermanentCity=@PermanentCity,
                PermanentProvince=@PermanentProvince, PermanentZipCode=@PermanentZipCode,
                DepartmentId=ISNULL(@UpDeptId,DepartmentId), PositionId=ISNULL(@PositionId,PositionId),
                ManagerId=@ManagerId, BranchId=@BranchId, EmploymentType=@EmploymentType,
                HireDate=ISNULL(@HireDate,HireDate), ProbationEndDate=@ProbationEndDate, RegularizationDate=@RegularizationDate,
                BasicSalary=@BasicSalary, SalaryFrequency=@SalaryFrequency,
                SalaryEffectiveDate=ISNULL(@SalaryEffectiveDate,SalaryEffectiveDate),
                BankAccountNumber=ISNULL(@BankAccountNumber,''), BankName=ISNULL(@BankName,''),
                ProfilePhotoPath=@ProfilePhotoPath, BiometricId=@BiometricId,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @Id;

            COMMIT TRANSACTION;

            -- Return updated employee
            SELECT
                e.Id, e.EmployeeCode, e.FirstName, e.MiddleName, e.LastName, e.Suffix,
                e.Email, e.MobileNumber, e.Status,
                e.DepartmentId, d.DepartmentName,
                e.PositionId, p.PositionTitle AS PositionName,
                e.ManagerId, ISNULL(mg.LastName + ', ' + mg.FirstName, NULL) AS ManagerName,
                e.BranchId, b.BranchName,
                e.HireDate, e.BasicSalary, e.EmploymentType, e.ProfilePhotoPath,
                e.LastName + ', ' + e.FirstName AS FullName,
                e.DateOfBirth, e.Gender, e.MaritalStatus,
                e.TaxIdentificationNumber, e.SssNumber, e.PhilHealthNumber, e.PagIbigNumber,
                e.PersonalEmail, e.AlternatePhone,
                e.PresentAddress, e.PresentCity, e.PresentProvince, e.PresentZipCode,
                e.SameAsPresentAddress, e.PermanentAddress, e.PermanentCity, e.PermanentProvince, e.PermanentZipCode,
                e.TerminationDate, e.ProbationEndDate, e.RegularizationDate, e.LastWorkingDate,
                e.SalaryFrequency, e.SalaryEffectiveDate, e.BankAccountNumber, e.BankName,
                e.SssContribution, e.PhilHealthContribution, e.PagIbigContribution,
                e.BiometricId, e.StatusRemarks, e.StatusChangedAt, e.StatusChangedBy,
                e.CreatedAt, e.CreatedBy, e.UpdatedAt, e.UpdatedBy
            FROM Employees e
            LEFT JOIN Departments d ON d.Id = e.DepartmentId
            LEFT JOIN Positions p ON p.Id = e.PositionId
            LEFT JOIN Employees mg ON mg.Id = e.ManagerId
            LEFT JOIN Branches b ON b.Id = e.BranchId
            WHERE e.Id = @Id;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE'
    BEGIN
        BEGIN TRY
            DECLARE @DelEmpStatus INT;
            SELECT @DelEmpStatus = Status FROM Employees WHERE Id = @Id AND IsDeleted = 0;
            IF @DelEmpStatus IS NULL BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END
            IF @DelEmpStatus = 1 BEGIN RAISERROR('Cannot delete an active employee. Change status first.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM Employees WHERE ManagerId = @Id AND IsDeleted = 0 AND Status = 1)
            BEGIN RAISERROR('Cannot delete: employee has active subordinates.', 16, 1); RETURN; END

            BEGIN TRANSACTION;
            UPDATE EmployeeEmergencyContacts SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE EmployeeId=@Id AND IsDeleted=0;
            UPDATE EmployeeDocuments SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE EmployeeId=@Id AND IsDeleted=0;
            UPDATE Employees SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CHANGE_STATUS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CHANGE_STATUS'
    BEGIN
        BEGIN TRY
            DECLARE @CsOldStatus INT;
            SELECT @CsOldStatus = Status FROM Employees WHERE Id = @Id AND IsDeleted = 0;
            IF @CsOldStatus IS NULL BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END

            DECLARE @CsValid BIT = 0;
            IF @CsOldStatus = 1 AND @NewStatus IN (2,3,4,5,6) SET @CsValid = 1;
            IF @CsOldStatus = 2 AND @NewStatus IN (1,4,5) SET @CsValid = 1;
            IF @CsOldStatus = 3 AND @NewStatus IN (1,4) SET @CsValid = 1;
            IF @CsOldStatus = 6 AND @NewStatus IN (1,4) SET @CsValid = 1;
            IF @CsOldStatus = 4 AND @NewStatus = 1 SET @CsValid = 1;
            IF @CsOldStatus = 5 AND @NewStatus = 1 SET @CsValid = 1;

            IF @CsValid = 0
            BEGIN RAISERROR('Invalid status transition from %d to %d.', 16, 1, @CsOldStatus, @NewStatus); RETURN; END

            IF @NewStatus IN (4,5) AND @LastWorkingDate IS NULL
            BEGIN RAISERROR('Last working date is required for Terminated/Retired status.', 16, 1); RETURN; END

            BEGIN TRANSACTION;

            INSERT INTO EmployeeStatusHistory (EmployeeId, PreviousStatus, NewStatus, Remarks, ChangedBy, ChangedAt, CreatedAt, CreatedBy)
            VALUES (@Id, @CsOldStatus, @NewStatus, ISNULL(@Remarks,''), @ChangedBy, GETDATE(), GETDATE(), @ChangedBy);

            UPDATE Employees SET
                Status = @NewStatus, StatusRemarks = @Remarks, StatusChangedAt = GETDATE(), StatusChangedBy = @ChangedBy,
                LastWorkingDate = CASE WHEN @NewStatus IN (4,5) THEN @LastWorkingDate ELSE LastWorkingDate END,
                TerminationDate = CASE WHEN @NewStatus = 4 THEN @LastWorkingDate ELSE TerminationDate END,
                UpdatedBy = @ChangedBy, UpdatedAt = GETDATE()
            WHERE Id = @Id;

            COMMIT TRANSACTION;

            SELECT TOP 1 * FROM EmployeeStatusHistory WHERE EmployeeId = @Id ORDER BY ChangedAt DESC;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_STATUS_HISTORY
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_STATUS_HISTORY'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Employees WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END
            SELECT * FROM EmployeeStatusHistory WHERE EmployeeId = @Id ORDER BY ChangedAt DESC;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_EMERGENCY_CONTACTS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_EMERGENCY_CONTACTS'
    BEGIN
        BEGIN TRY
            SELECT * FROM EmployeeEmergencyContacts WHERE EmployeeId = @Id AND IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE_EMERGENCY_CONTACT
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE_EMERGENCY_CONTACT'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Employees WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END

            IF @IsPrimary = 1
                UPDATE EmployeeEmergencyContacts SET IsPrimary = 0 WHERE EmployeeId = @Id AND IsDeleted = 0;

            INSERT INTO EmployeeEmergencyContacts (EmployeeId, ContactName, Relationship, MobileNumber, AlternatePhone, IsPrimary, CreatedAt, CreatedBy)
            VALUES (@Id, @ContactName, @Relationship, @Phone, @AlternatePhone, @IsPrimary, GETDATE(), @CreatedBy);

            SELECT * FROM EmployeeEmergencyContacts WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE_EMERGENCY_CONTACT
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_EMERGENCY_CONTACT'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM EmployeeEmergencyContacts WHERE Id = @ContactId AND EmployeeId = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Emergency contact not found.', 16, 1); RETURN; END

            IF @IsPrimary = 1
                UPDATE EmployeeEmergencyContacts SET IsPrimary = 0 WHERE EmployeeId = @Id AND Id <> @ContactId AND IsDeleted = 0;

            UPDATE EmployeeEmergencyContacts SET ContactName=@ContactName, Relationship=@Relationship,
                MobileNumber=@Phone, AlternatePhone=@AlternatePhone, IsPrimary=@IsPrimary,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @ContactId;

            SELECT * FROM EmployeeEmergencyContacts WHERE Id = @ContactId;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE_EMERGENCY_CONTACT
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE_EMERGENCY_CONTACT'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM EmployeeEmergencyContacts WHERE Id = @ContactId AND EmployeeId = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Emergency contact not found.', 16, 1); RETURN; END

            UPDATE EmployeeEmergencyContacts SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@ContactId;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_DOCUMENTS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_DOCUMENTS'
    BEGIN
        BEGIN TRY
            SELECT * FROM EmployeeDocuments WHERE EmployeeId = @Id AND IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE_DOCUMENT
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE_DOCUMENT'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Employees WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Employee not found.', 16, 1); RETURN; END

            INSERT INTO EmployeeDocuments (EmployeeId, DocumentType, DocumentName, FilePath, ExpiryDate, IsVerified, CreatedAt, CreatedBy)
            VALUES (@Id, @DocumentType, @DocumentName, @FilePath, @ExpiryDate, 0, GETDATE(), @CreatedBy);

            SELECT * FROM EmployeeDocuments WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE_DOCUMENT
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_DOCUMENT'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM EmployeeDocuments WHERE Id = @DocumentId AND EmployeeId = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Document not found.', 16, 1); RETURN; END

            UPDATE EmployeeDocuments SET DocumentType=@DocumentType, DocumentName=@DocumentName,
                FilePath=@FilePath, ExpiryDate=@ExpiryDate,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @DocumentId;

            SELECT * FROM EmployeeDocuments WHERE Id = @DocumentId;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE_DOCUMENT
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE_DOCUMENT'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM EmployeeDocuments WHERE Id = @DocumentId AND EmployeeId = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Document not found.', 16, 1); RETURN; END

            UPDATE EmployeeDocuments SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@DocumentId;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_SALARY_HISTORY
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_SALARY_HISTORY'
    BEGIN
        BEGIN TRY
            SELECT * FROM SalaryHistory WHERE EmployeeId = @Id ORDER BY EffectiveDate DESC;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_Employee: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 5. sp_PayPeriod
-- ============================================================================
CREATE PROCEDURE dbo.sp_PayPeriod
    @ActionType  VARCHAR(50),
    @Id          INT = NULL,
    @Year        INT = NULL,
    @Status      NVARCHAR(20) = NULL,
    @StartDate   DATE = NULL,
    @EndDate     DATE = NULL,
    @PayDate     DATE = NULL,
    @PeriodType  INT = NULL,
    @CreatedBy   NVARCHAR(256) = NULL,
    @UpdatedBy   NVARCHAR(256) = NULL,
    @DeletedBy   NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- -----------------------------------------------------------------------
    -- GET_ALL
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_ALL'
    BEGIN
        BEGIN TRY
            SELECT pp.*,
                (SELECT COUNT(*) FROM PayrollRecords pr WHERE pr.PayrollPeriodId = pp.Id AND pr.IsDeleted = 0) AS PayrollRecordCount
            FROM PayrollPeriods pp
            WHERE pp.IsDeleted = 0
              AND (@Year IS NULL OR YEAR(pp.StartDate) = @Year)
              AND (@Status IS NULL OR pp.Status = @Status)
            ORDER BY pp.StartDate DESC;
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
            SELECT pp.*,
                (SELECT COUNT(*) FROM PayrollRecords pr WHERE pr.PayrollPeriodId = pp.Id AND pr.IsDeleted = 0) AS PayrollRecordCount
            FROM PayrollPeriods pp
            WHERE pp.Id = @Id AND pp.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            IF @EndDate <= @StartDate
            BEGIN RAISERROR('End date must be after start date.', 16, 1); RETURN; END

            IF @PayDate < @EndDate
            BEGIN RAISERROR('Pay date must be on or after end date.', 16, 1); RETURN; END

            DECLARE @CrHalf NVARCHAR(1) = CASE WHEN DAY(@StartDate) <= 15 THEN '1' ELSE '2' END;
            DECLARE @CrPeriodCode NVARCHAR(20) = FORMAT(@StartDate, 'yyyy-MM') + '-' + @CrHalf;

            IF EXISTS (SELECT 1 FROM PayrollPeriods WHERE PeriodCode = @CrPeriodCode AND IsDeleted = 0)
            BEGIN RAISERROR('Period code %s already exists.', 16, 1, @CrPeriodCode); RETURN; END

            DECLARE @CrLastName NVARCHAR(100);
            SELECT TOP 1 @CrLastName = Name FROM PayrollPeriods WHERE IsDeleted = 0 ORDER BY CreatedAt DESC;

            DECLARE @CrPrefix NVARCHAR(5) = 'AA';
            IF @CrLastName IS NOT NULL AND LEN(@CrLastName) >= 2
            BEGIN
                DECLARE @CrLastPrefix NVARCHAR(5) = LEFT(@CrLastName, CHARINDEX('-', @CrLastName) - 1);
                IF LEN(@CrLastPrefix) = 2
                BEGIN
                    DECLARE @CrC1 CHAR(1) = LEFT(@CrLastPrefix, 1);
                    DECLARE @CrC2 CHAR(1) = RIGHT(@CrLastPrefix, 1);
                    IF @CrC2 = 'Z'
                    BEGIN
                        SET @CrPrefix = CHAR(ASCII(@CrC1) + 1) + 'A';
                        IF ASCII(@CrC1) >= 90 SET @CrPrefix = 'AA';
                    END
                    ELSE
                        SET @CrPrefix = @CrC1 + CHAR(ASCII(@CrC2) + 1);
                END
            END

            DECLARE @CrPeriodName NVARCHAR(100) = @CrPrefix + '-' + FORMAT(@PayDate, 'MMddyyyy');

            INSERT INTO PayrollPeriods (Name, PeriodCode, StartDate, EndDate, PayDate, PeriodType, Status, IsClosed, CreatedAt, CreatedBy)
            VALUES (@CrPeriodName, @CrPeriodCode, @StartDate, @EndDate, @PayDate, @PeriodType, 'Draft', 0, GETDATE(), @CreatedBy);

            SELECT pp.*, 0 AS PayrollRecordCount FROM PayrollPeriods pp WHERE pp.Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE_STATUS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_STATUS'
    BEGIN
        BEGIN TRY
            DECLARE @UsCurStatus NVARCHAR(20);
            SELECT @UsCurStatus = Status FROM PayrollPeriods WHERE Id = @Id AND IsDeleted = 0;
            IF @UsCurStatus IS NULL BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END

            -- Validate status transitions
            DECLARE @UsValid BIT = 0;
            IF @UsCurStatus = 'Draft'    AND @Status = 'Open'     SET @UsValid = 1;
            IF @UsCurStatus = 'Open'     AND @Status IN ('Computed','Draft')  SET @UsValid = 1;
            IF @UsCurStatus = 'Computed' AND @Status IN ('Approved','Open')   SET @UsValid = 1;
            IF @UsCurStatus = 'Approved' AND @Status IN ('Locked','Computed') SET @UsValid = 1;
            IF @UsCurStatus = 'Locked'   AND @Status IN ('Exported','Approved') SET @UsValid = 1;

            IF @UsValid = 0
            BEGIN RAISERROR('Invalid period status transition from %s to %s.', 16, 1, @UsCurStatus, @Status); RETURN; END

            DECLARE @UsIsClosed BIT = CASE WHEN @Status IN ('Locked','Exported') THEN 1 ELSE 0 END;

            UPDATE PayrollPeriods SET Status = @Status, IsClosed = @UsIsClosed,
                UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE()
            WHERE Id = @Id;

            SELECT pp.*, (SELECT COUNT(*) FROM PayrollRecords pr WHERE pr.PayrollPeriodId = pp.Id AND pr.IsDeleted = 0) AS PayrollRecordCount
            FROM PayrollPeriods pp WHERE pp.Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE'
    BEGIN
        BEGIN TRY
            DECLARE @DelCurStatus NVARCHAR(20);
            SELECT @DelCurStatus = Status FROM PayrollPeriods WHERE Id = @Id AND IsDeleted = 0;
            IF @DelCurStatus IS NULL BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END
            IF @DelCurStatus <> 'Draft' BEGIN RAISERROR('Only Draft periods can be deleted.', 16, 1); RETURN; END

            UPDATE PayrollPeriods SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_PayPeriod: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 6. sp_WorkSchedule
-- ============================================================================
CREATE PROCEDURE dbo.sp_WorkSchedule
    @ActionType    VARCHAR(50),
    @Id            INT = NULL,
    @Name          NVARCHAR(100) = NULL,
    @Description   NVARCHAR(500) = NULL,
    @IsDefault     BIT = 0,
    @DaysJson      NVARCHAR(MAX) = NULL,
    @EmployeeIds   NVARCHAR(MAX) = NULL,
    @EmployeeId    INT = NULL,
    @EffectiveDate DATE = NULL,
    @CreatedBy     NVARCHAR(256) = NULL,
    @UpdatedBy     NVARCHAR(256) = NULL,
    @DeletedBy     NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET_ALL'
    BEGIN
        BEGIN TRY
            SELECT ws.*,
                (SELECT COUNT(DISTINCT es.EmployeeId) FROM EmployeeSchedules es WHERE es.WorkScheduleId = ws.Id AND es.EndDate IS NULL AND es.IsDeleted = 0) AS EmployeeCount
            FROM WorkSchedules ws
            WHERE ws.IsDeleted = 0
            ORDER BY ws.Name;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_BY_ID'
    BEGIN
        BEGIN TRY
            SELECT * FROM WorkSchedules WHERE Id = @Id AND IsDeleted = 0;
            SELECT * FROM WorkScheduleDays WHERE WorkScheduleId = @Id AND IsDeleted = 0 ORDER BY DayOfWeek;
            SELECT COUNT(DISTINCT es.EmployeeId) AS EmployeeCount
            FROM EmployeeSchedules es WHERE es.WorkScheduleId = @Id AND es.EndDate IS NULL AND es.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF EXISTS (SELECT 1 FROM WorkSchedules WHERE Name = @Name AND IsDeleted = 0)
            BEGIN RAISERROR('Schedule name already exists.', 16, 1); RETURN; END

            IF @IsDefault = 1
                UPDATE WorkSchedules SET IsDefault = 0 WHERE IsDefault = 1 AND IsDeleted = 0;

            INSERT INTO WorkSchedules (Name, Description, IsDefault, CreatedAt, CreatedBy)
            VALUES (@Name, @Description, @IsDefault, GETDATE(), @CreatedBy);

            DECLARE @WsCrSchedId INT = SCOPE_IDENTITY();

            INSERT INTO WorkScheduleDays (WorkScheduleId, DayOfWeek, IsRestDay, ShiftStart, ShiftEnd, BreakStart, BreakEnd, CreatedAt, CreatedBy)
            SELECT @WsCrSchedId,
                CAST(JSON_VALUE(d.value, '$.DayOfWeek') AS INT),
                CAST(ISNULL(JSON_VALUE(d.value, '$.IsRestDay'), '0') AS BIT),
                CAST(JSON_VALUE(d.value, '$.ShiftStart') AS TIME),
                CAST(JSON_VALUE(d.value, '$.ShiftEnd') AS TIME),
                CAST(JSON_VALUE(d.value, '$.BreakStart') AS TIME),
                CAST(JSON_VALUE(d.value, '$.BreakEnd') AS TIME),
                GETDATE(), @CreatedBy
            FROM OPENJSON(@DaysJson) d;

            COMMIT TRANSACTION;

            SELECT * FROM WorkSchedules WHERE Id = @WsCrSchedId;
            SELECT * FROM WorkScheduleDays WHERE WorkScheduleId = @WsCrSchedId AND IsDeleted = 0 ORDER BY DayOfWeek;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF NOT EXISTS (SELECT 1 FROM WorkSchedules WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Work schedule not found.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM WorkSchedules WHERE Name = @Name AND Id <> @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Schedule name already exists.', 16, 1); RETURN; END

            IF @IsDefault = 1
                UPDATE WorkSchedules SET IsDefault = 0 WHERE IsDefault = 1 AND Id <> @Id AND IsDeleted = 0;

            DELETE FROM WorkScheduleDays WHERE WorkScheduleId = @Id;

            INSERT INTO WorkScheduleDays (WorkScheduleId, DayOfWeek, IsRestDay, ShiftStart, ShiftEnd, BreakStart, BreakEnd, CreatedAt, CreatedBy)
            SELECT @Id,
                CAST(JSON_VALUE(d.value, '$.DayOfWeek') AS INT),
                CAST(ISNULL(JSON_VALUE(d.value, '$.IsRestDay'), '0') AS BIT),
                CAST(JSON_VALUE(d.value, '$.ShiftStart') AS TIME),
                CAST(JSON_VALUE(d.value, '$.ShiftEnd') AS TIME),
                CAST(JSON_VALUE(d.value, '$.BreakStart') AS TIME),
                CAST(JSON_VALUE(d.value, '$.BreakEnd') AS TIME),
                GETDATE(), @UpdatedBy
            FROM OPENJSON(@DaysJson) d;

            UPDATE WorkSchedules SET Name=@Name, Description=@Description, IsDefault=@IsDefault,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @Id;

            COMMIT TRANSACTION;

            SELECT * FROM WorkSchedules WHERE Id = @Id;
            SELECT * FROM WorkScheduleDays WHERE WorkScheduleId = @Id AND IsDeleted = 0 ORDER BY DayOfWeek;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    IF @ActionType = 'DELETE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM WorkSchedules WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Work schedule not found.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM EmployeeSchedules WHERE WorkScheduleId = @Id AND EndDate IS NULL AND IsDeleted = 0)
            BEGIN RAISERROR('Cannot delete: schedule has active employee assignments.', 16, 1); RETURN; END

            UPDATE WorkSchedules SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'ASSIGN_EMPLOYEES'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF NOT EXISTS (SELECT 1 FROM WorkSchedules WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Work schedule not found.', 16, 1); RETURN; END

            SELECT CAST(LTRIM(RTRIM(value)) AS INT) AS EmpId
            INTO #WsAssignEmpIds
            FROM STRING_SPLIT(@EmployeeIds, ',') WHERE LTRIM(RTRIM(value)) <> '';

            UPDATE EmployeeSchedules SET EndDate = @EffectiveDate, UpdatedBy = @CreatedBy, UpdatedAt = GETDATE()
            WHERE EmployeeId IN (SELECT EmpId FROM #WsAssignEmpIds) AND EndDate IS NULL AND IsDeleted = 0;

            INSERT INTO EmployeeSchedules (EmployeeId, WorkScheduleId, EffectiveDate, CreatedAt, CreatedBy)
            SELECT EmpId, @Id, @EffectiveDate, GETDATE(), @CreatedBy
            FROM #WsAssignEmpIds
            WHERE EmpId IN (SELECT Id FROM Employees WHERE IsDeleted = 0);

            DROP TABLE #WsAssignEmpIds;
            COMMIT TRANSACTION;

            SELECT e.Id, e.EmployeeCode, e.LastName + ', ' + e.FirstName AS EmployeeName,
                es.EffectiveDate, es.EndDate
            FROM EmployeeSchedules es
            INNER JOIN Employees e ON e.Id = es.EmployeeId
            WHERE es.WorkScheduleId = @Id AND es.EndDate IS NULL AND es.IsDeleted = 0;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#WsAssignEmpIds') IS NOT NULL DROP TABLE #WsAssignEmpIds;
            THROW;
        END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_EMPLOYEES'
    BEGIN
        BEGIN TRY
            SELECT e.Id, e.EmployeeCode, e.LastName + ', ' + e.FirstName AS EmployeeName,
                e.Status, es.EffectiveDate, es.EndDate
            FROM EmployeeSchedules es
            INNER JOIN Employees e ON e.Id = es.EmployeeId
            WHERE es.WorkScheduleId = @Id AND es.EndDate IS NULL AND es.IsDeleted = 0
            ORDER BY e.LastName, e.FirstName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UNASSIGN_EMPLOYEE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM EmployeeSchedules WHERE WorkScheduleId=@Id AND EmployeeId=@EmployeeId AND EndDate IS NULL AND IsDeleted=0)
            BEGIN RAISERROR('Active assignment not found.', 16, 1); RETURN; END

            UPDATE EmployeeSchedules SET EndDate = CAST(GETDATE() AS DATE), UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE()
            WHERE WorkScheduleId = @Id AND EmployeeId = @EmployeeId AND EndDate IS NULL AND IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_WorkSchedule: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 7. sp_ScheduleRule
-- ============================================================================
CREATE PROCEDURE dbo.sp_ScheduleRule
    @ActionType              VARCHAR(50),
    @Id                      INT = NULL,
    @HalfDayThresholdHours   DECIMAL(18,2) = NULL,
    @NightDiffStartTime      TIME = NULL,
    @NightDiffEndTime        TIME = NULL,
    @NightDiffRate           DECIMAL(18,2) = NULL,
    @OTMinimumMinutes        INT = NULL,
    @OTRequiresApproval      BIT = NULL,
    @OTStartAfterMinutes     INT = NULL,
    @GracePeriodMinutes      INT = NULL,
    @BreakDurationMinutes    INT = NULL,
    @RegularHoursPerDay      DECIMAL(18,2) = NULL,
    @UpdatedBy               NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM ScheduleRules WHERE IsDeleted = 0)
            BEGIN
                INSERT INTO ScheduleRules (HalfDayThresholdHours, NightDiffStartTime, NightDiffEndTime, NightDiffRate,
                    OTMinimumMinutes, OTRequiresApproval, OTStartAfterMinutes, GracePeriodMinutes, BreakDurationMinutes,
                    RegularHoursPerDay, CreatedAt, CreatedBy)
                VALUES (4.0, '22:00:00', '06:00:00', 1.10, 30, 1, 0, 15, 60, 8.0, GETDATE(), 'system');
            END

            SELECT TOP 1 * FROM ScheduleRules WHERE IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM ScheduleRules WHERE IsDeleted = 0)
            BEGIN RAISERROR('Schedule rules not found. Call GET first.', 16, 1); RETURN; END

            UPDATE ScheduleRules SET
                HalfDayThresholdHours=@HalfDayThresholdHours, NightDiffStartTime=@NightDiffStartTime,
                NightDiffEndTime=@NightDiffEndTime, NightDiffRate=@NightDiffRate,
                OTMinimumMinutes=@OTMinimumMinutes, OTRequiresApproval=@OTRequiresApproval,
                OTStartAfterMinutes=@OTStartAfterMinutes, GracePeriodMinutes=@GracePeriodMinutes,
                BreakDurationMinutes=@BreakDurationMinutes, RegularHoursPerDay=@RegularHoursPerDay,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE IsDeleted = 0;

            SELECT TOP 1 * FROM ScheduleRules WHERE IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_ScheduleRule: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 8. sp_Role
-- ============================================================================
CREATE PROCEDURE dbo.sp_Role
    @ActionType      VARCHAR(50),
    @Id              INT = NULL,
    @Name            NVARCHAR(100) = NULL,
    @Description     NVARCHAR(500) = NULL,
    @PermissionsJson NVARCHAR(MAX) = NULL,
    @UserId          INT = NULL,
    @UserRole        NVARCHAR(50) = NULL,
    @CreatedBy       NVARCHAR(256) = NULL,
    @UpdatedBy       NVARCHAR(256) = NULL,
    @DeletedBy       NVARCHAR(256) = NULL,
    @AssignedBy      NVARCHAR(256) = NULL,
    @RemovedBy       NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET_ALL'
    BEGIN
        BEGIN TRY
            SELECT r.*,
                (SELECT COUNT(*) FROM Users u WHERE u.RoleId = r.Id AND u.IsDeleted = 0 AND u.IsActive = 1) AS UserCount
            FROM Roles r
            WHERE r.IsDeleted = 0
            ORDER BY r.Name;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_WITH_PERMISSIONS'
    BEGIN
        BEGIN TRY
            SELECT * FROM Roles WHERE Id = @Id AND IsDeleted = 0;
            SELECT * FROM RolePermissions WHERE RoleId = @Id AND IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            IF EXISTS (SELECT 1 FROM Roles WHERE Name = @Name AND IsDeleted = 0)
            BEGIN RAISERROR('Role name already exists.', 16, 1); RETURN; END

            INSERT INTO Roles (Name, Description, CreatedAt, CreatedBy)
            VALUES (@Name, @Description, GETDATE(), @CreatedBy);

            SELECT * FROM Roles WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Role not found.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM Roles WHERE Name = @Name AND Id <> @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Role name already exists.', 16, 1); RETURN; END

            UPDATE Roles SET Name=@Name, Description=@Description, UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE() WHERE Id=@Id;
            SELECT * FROM Roles WHERE Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'DELETE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Role not found.', 16, 1); RETURN; END

            UPDATE Roles SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE_PERMISSIONS'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Role not found.', 16, 1); RETURN; END

            DELETE FROM RolePermissions WHERE RoleId = @Id;

            INSERT INTO RolePermissions (RoleId, ModuleKey, CanView, CanAdd, CanUpdate, CanDelete, CreatedAt, CreatedBy)
            SELECT @Id,
                JSON_VALUE(p.value, '$.ModuleKey'),
                CAST(ISNULL(JSON_VALUE(p.value, '$.CanView'), '0') AS BIT),
                CAST(ISNULL(JSON_VALUE(p.value, '$.CanAdd'), '0') AS BIT),
                CAST(ISNULL(JSON_VALUE(p.value, '$.CanUpdate'), '0') AS BIT),
                CAST(ISNULL(JSON_VALUE(p.value, '$.CanDelete'), '0') AS BIT),
                GETDATE(), @UpdatedBy
            FROM OPENJSON(@PermissionsJson) p;

            UPDATE Roles SET UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE() WHERE Id=@Id;

            COMMIT TRANSACTION;

            SELECT * FROM Roles WHERE Id = @Id;
            SELECT * FROM RolePermissions WHERE RoleId = @Id AND IsDeleted = 0;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_ALL_USERS'
    BEGIN
        BEGIN TRY
            SELECT u.Id, u.Username, u.Email, u.Role, u.RoleId,
                r.Name AS RoleName, u.IsActive, u.LastLoginAt, u.EmployeeId
            FROM Users u
            LEFT JOIN Roles r ON r.Id = u.RoleId
            WHERE u.IsDeleted = 0 AND u.IsActive = 1
            ORDER BY u.Username;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_USERS_FOR_ROLE'
    BEGIN
        BEGIN TRY
            SELECT u.Id, u.Username, u.Email, u.Role, u.RoleId, u.IsActive, u.LastLoginAt, u.EmployeeId
            FROM Users u
            WHERE u.RoleId = @Id AND u.IsDeleted = 0
            ORDER BY u.Username;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'ASSIGN_USER'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Role not found.', 16, 1); RETURN; END

            IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId AND IsDeleted = 0)
            BEGIN RAISERROR('User not found.', 16, 1); RETURN; END

            UPDATE Users SET RoleId = @Id, UpdatedBy = @AssignedBy, UpdatedAt = GETDATE() WHERE Id = @UserId;

            SELECT * FROM Users WHERE Id = @UserId;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'REMOVE_USER'
    BEGIN
        BEGIN TRY
            DECLARE @RuCurRoleId INT;
            SELECT @RuCurRoleId = RoleId FROM Users WHERE Id = @UserId AND IsDeleted = 0;

            IF @RuCurRoleId IS NULL OR @RuCurRoleId <> @Id
            BEGIN RAISERROR('User does not have this role.', 16, 1); RETURN; END

            UPDATE Users SET RoleId = NULL, UpdatedBy = @RemovedBy, UpdatedAt = GETDATE() WHERE Id = @UserId;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_USER_PERMISSIONS'
    BEGIN
        BEGIN TRY
            IF @UserRole = 'Admin'
            BEGIN
                SELECT m.ModuleKey, CAST(1 AS BIT) AS CanView, CAST(1 AS BIT) AS CanAdd,
                    CAST(1 AS BIT) AS CanUpdate, CAST(1 AS BIT) AS CanDelete
                FROM (VALUES
                    ('dashboard'),('pay-periods'),('attendance'),('payroll-run'),('approvals'),
                    ('payslips'),('employees'),('leave'),('reports'),('setup'),
                    ('user-roles'),('users'),('work-schedules')
                ) AS m(ModuleKey);
                RETURN;
            END

            DECLARE @UpRoleId INT;
            SELECT @UpRoleId = RoleId FROM Users WHERE Id = @UserId AND IsDeleted = 0;

            IF @UpRoleId IS NULL
            BEGIN
                SELECT NULL AS ModuleKey, CAST(0 AS BIT) AS CanView, CAST(0 AS BIT) AS CanAdd,
                    CAST(0 AS BIT) AS CanUpdate, CAST(0 AS BIT) AS CanDelete
                WHERE 1 = 0;
                RETURN;
            END

            SELECT ModuleKey, CanView, CanAdd, CanUpdate, CanDelete
            FROM RolePermissions WHERE RoleId = @UpRoleId AND IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_Role: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 9. sp_User
-- ============================================================================
CREATE PROCEDURE dbo.sp_User
    @ActionType      VARCHAR(50),
    @Id              INT = NULL,
    @Username        NVARCHAR(100) = NULL,
    @Email           NVARCHAR(256) = NULL,
    @PasswordHash    NVARCHAR(500) = NULL,
    @Role            NVARCHAR(50) = NULL,
    @RoleId          INT = NULL,
    @UsernameOrEmail NVARCHAR(256) = NULL,
    @CreatedBy       NVARCHAR(256) = NULL,
    @UpdatedBy       NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email AND IsDeleted = 0)
            BEGIN RAISERROR('Email already exists.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username AND IsDeleted = 0)
            BEGIN RAISERROR('Username already exists.', 16, 1); RETURN; END

            IF @RoleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @RoleId AND IsDeleted = 0)
            BEGIN RAISERROR('Role not found.', 16, 1); RETURN; END

            INSERT INTO Users (Username, Email, PasswordHash, Role, RoleId, IsActive, CreatedAt, CreatedBy)
            VALUES (@Username, @Email, @PasswordHash, @Role, @RoleId, 1, GETDATE(), @CreatedBy);

            SELECT Id, Username, Email, Role, RoleId, IsActive, CreatedAt, CreatedBy FROM Users WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'RESET_PASSWORD'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('User not found.', 16, 1); RETURN; END

            UPDATE Users SET PasswordHash = @PasswordHash, UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE() WHERE Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_FOR_LOGIN'
    BEGIN
        BEGIN TRY
            SELECT u.Id, u.Username, u.Email, u.PasswordHash, u.IsActive, u.Role, u.RoleId, u.EmployeeId,
                u.LastLoginAt, u.CreatedAt
            FROM Users u
            WHERE (u.Username = @UsernameOrEmail OR u.Email = @UsernameOrEmail)
              AND u.IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_User: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 10. sp_CompanySettings
-- ============================================================================
CREATE PROCEDURE dbo.sp_CompanySettings
    @ActionType                    VARCHAR(50),
    @CompanyName                   NVARCHAR(256) = NULL,
    @Address                       NVARCHAR(500) = NULL,
    @DateStarted                   DATETIME = NULL,
    @TaxNo                         NVARCHAR(50) = NULL,
    @BirNo                         NVARCHAR(50) = NULL,
    @EmployerSssNo                 NVARCHAR(50) = NULL,
    @IndustryClassification        NVARCHAR(200) = NULL,
    @DateFormat                    NVARCHAR(50) = NULL,
    @DefaultSssContribution        DECIMAL(18,2) = NULL,
    @DefaultPhilHealthContribution DECIMAL(18,2) = NULL,
    @DefaultPagIbigContribution    DECIMAL(18,2) = NULL,
    @UpdatedBy                     NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM CompanySettings WHERE IsDeleted = 0)
            BEGIN
                INSERT INTO CompanySettings (CompanyName, DateFormat, DefaultSssContribution, DefaultPhilHealthContribution,
                    DefaultPagIbigContribution, CreatedAt, CreatedBy)
                VALUES ('ETSS', 'MM/dd/yyyy', 900, 500, 200, GETDATE(), 'system');
            END

            SELECT TOP 1 * FROM CompanySettings WHERE IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM CompanySettings WHERE IsDeleted = 0)
            BEGIN
                INSERT INTO CompanySettings (CompanyName, DateFormat, DefaultSssContribution, DefaultPhilHealthContribution,
                    DefaultPagIbigContribution, CreatedAt, CreatedBy)
                VALUES ('ETSS', 'MM/dd/yyyy', 900, 500, 200, GETDATE(), 'system');
            END

            UPDATE CompanySettings SET
                CompanyName=ISNULL(@CompanyName, CompanyName),
                Address=@Address, DateStarted=@DateStarted,
                TaxNo=@TaxNo, BirNo=@BirNo, EmployerSssNo=@EmployerSssNo,
                IndustryClassification=@IndustryClassification,
                DateFormat=ISNULL(@DateFormat, DateFormat),
                DefaultSssContribution=ISNULL(@DefaultSssContribution, DefaultSssContribution),
                DefaultPhilHealthContribution=ISNULL(@DefaultPhilHealthContribution, DefaultPhilHealthContribution),
                DefaultPagIbigContribution=ISNULL(@DefaultPagIbigContribution, DefaultPagIbigContribution),
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE IsDeleted = 0;

            SELECT TOP 1 * FROM CompanySettings WHERE IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE_DEDUCTIONS'
    BEGIN
        BEGIN TRY
            UPDATE CompanySettings SET
                DefaultSssContribution=@DefaultSssContribution,
                DefaultPhilHealthContribution=@DefaultPhilHealthContribution,
                DefaultPagIbigContribution=@DefaultPagIbigContribution,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE IsDeleted = 0;

            SELECT TOP 1 * FROM CompanySettings WHERE IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_CompanySettings: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 11. sp_GlobalConfig
-- ============================================================================
CREATE PROCEDURE dbo.sp_GlobalConfig
    @ActionType  VARCHAR(50),
    @ConfigName  NVARCHAR(100) = NULL,
    @ConfigValue VARBINARY(MAX) = NULL,
    @MimeType    NVARCHAR(100) = NULL,
    @UpdatedBy   NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET_BY_NAME'
    BEGIN
        BEGIN TRY
            SELECT * FROM GlobalConfig WHERE ConfigName = @ConfigName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPSERT'
    BEGIN
        BEGIN TRY
            IF EXISTS (SELECT 1 FROM GlobalConfig WHERE ConfigName = @ConfigName)
            BEGIN
                UPDATE GlobalConfig SET ConfigValue = @ConfigValue, MimeType = @MimeType,
                    UpdatedDate = GETDATE(), UpdatedBy = @UpdatedBy
                WHERE ConfigName = @ConfigName;
            END
            ELSE
            BEGIN
                INSERT INTO GlobalConfig (ConfigName, ConfigValue, MimeType, CreatedDate, CreatedBy)
                VALUES (@ConfigName, @ConfigValue, @MimeType, GETDATE(), @UpdatedBy);
            END

            SELECT * FROM GlobalConfig WHERE ConfigName = @ConfigName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_GlobalConfig: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 12. sp_LeaveType
-- ============================================================================
CREATE PROCEDURE dbo.sp_LeaveType
    @ActionType              VARCHAR(50),
    @Id                      INT = NULL,
    @Name                    NVARCHAR(100) = NULL,
    @Code                    NVARCHAR(20) = NULL,
    @Description             NVARCHAR(500) = NULL,
    @IsActive                BIT = 1,
    @DefaultDaysPerYear      DECIMAL(18,2) = 0,
    @EntitlementBasis        INT = 1,
    @TenureIncrementDays     DECIMAL(18,2) = NULL,
    @TenureMaxDays           DECIMAL(18,2) = NULL,
    @EligibleEmploymentTypes NVARCHAR(200) = NULL,
    @AccrualMethod           INT = 1,
    @PayCategory             INT = 1,
    @PaidPercentage          DECIMAL(18,2) = NULL,
    @BalanceDeductionMode    INT = 1,
    @CarryForwardPolicy      INT = 1,
    @CarryForwardMaxDays     DECIMAL(18,2) = NULL,
    @MinimumNoticeDays       INT = 0,
    @RequiresApproval        BIT = 1,
    @RequiresAttachment      BIT = 0,
    @Granularity             INT = 1,
    @CountWeekendsAsLeave    BIT = 0,
    @CountHolidaysAsLeave    BIT = 0,
    @AllowCashConversion     BIT = 0,
    @MaxCashConversionDays   DECIMAL(18,2) = NULL,
    @GenderRestriction       INT = NULL,
    @MinServiceMonths        INT = NULL,
    @CreatedBy               NVARCHAR(256) = NULL,
    @UpdatedBy               NVARCHAR(256) = NULL,
    @DeletedBy               NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET_ALL'
    BEGIN
        BEGIN TRY
            SELECT * FROM LeaveTypes WHERE IsDeleted = 0 ORDER BY Name;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_ALL_ACTIVE'
    BEGIN
        BEGIN TRY
            SELECT * FROM LeaveTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_BY_ID'
    BEGIN
        BEGIN TRY
            SELECT * FROM LeaveTypes WHERE Id = @Id AND IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            IF EXISTS (SELECT 1 FROM LeaveTypes WHERE Name = @Name AND IsDeleted = 0)
            BEGIN RAISERROR('Leave type name already exists.', 16, 1); RETURN; END

            IF @Code IS NOT NULL AND EXISTS (SELECT 1 FROM LeaveTypes WHERE Code = @Code AND IsDeleted = 0)
            BEGIN RAISERROR('Leave type code already exists.', 16, 1); RETURN; END

            INSERT INTO LeaveTypes (
                Name, Code, Description, IsActive, DefaultDaysPerYear, EntitlementBasis,
                TenureIncrementDays, TenureMaxDays, EligibleEmploymentTypes, AccrualMethod,
                PayCategory, PaidPercentage, BalanceDeductionMode, CarryForwardPolicy, CarryForwardMaxDays,
                MinimumNoticeDays, RequiresApproval, RequiresAttachment, Granularity,
                CountWeekendsAsLeave, CountHolidaysAsLeave, AllowCashConversion, MaxCashConversionDays,
                GenderRestriction, MinServiceMonths, CreatedAt, CreatedBy
            )
            VALUES (
                @Name, @Code, @Description, @IsActive, @DefaultDaysPerYear, @EntitlementBasis,
                @TenureIncrementDays, @TenureMaxDays, @EligibleEmploymentTypes, @AccrualMethod,
                @PayCategory, @PaidPercentage, @BalanceDeductionMode, @CarryForwardPolicy, @CarryForwardMaxDays,
                @MinimumNoticeDays, @RequiresApproval, @RequiresAttachment, @Granularity,
                @CountWeekendsAsLeave, @CountHolidaysAsLeave, @AllowCashConversion, @MaxCashConversionDays,
                @GenderRestriction, @MinServiceMonths, GETDATE(), @CreatedBy
            );

            SELECT * FROM LeaveTypes WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM LeaveTypes WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Leave type not found.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM LeaveTypes WHERE Name = @Name AND Id <> @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Leave type name already exists.', 16, 1); RETURN; END

            IF @Code IS NOT NULL AND EXISTS (SELECT 1 FROM LeaveTypes WHERE Code = @Code AND Id <> @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Leave type code already exists.', 16, 1); RETURN; END

            UPDATE LeaveTypes SET
                Name=@Name, Code=@Code, Description=@Description, IsActive=@IsActive,
                DefaultDaysPerYear=@DefaultDaysPerYear, EntitlementBasis=@EntitlementBasis,
                TenureIncrementDays=@TenureIncrementDays, TenureMaxDays=@TenureMaxDays,
                EligibleEmploymentTypes=@EligibleEmploymentTypes, AccrualMethod=@AccrualMethod,
                PayCategory=@PayCategory, PaidPercentage=@PaidPercentage,
                BalanceDeductionMode=@BalanceDeductionMode, CarryForwardPolicy=@CarryForwardPolicy,
                CarryForwardMaxDays=@CarryForwardMaxDays, MinimumNoticeDays=@MinimumNoticeDays,
                RequiresApproval=@RequiresApproval, RequiresAttachment=@RequiresAttachment,
                Granularity=@Granularity, CountWeekendsAsLeave=@CountWeekendsAsLeave,
                CountHolidaysAsLeave=@CountHolidaysAsLeave, AllowCashConversion=@AllowCashConversion,
                MaxCashConversionDays=@MaxCashConversionDays, GenderRestriction=@GenderRestriction,
                MinServiceMonths=@MinServiceMonths,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @Id;

            SELECT * FROM LeaveTypes WHERE Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'DELETE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM LeaveTypes WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Leave type not found.', 16, 1); RETURN; END

            UPDATE LeaveTypes SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_LeaveType: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 13. sp_AllowanceType
-- ============================================================================
CREATE PROCEDURE dbo.sp_AllowanceType
    @ActionType     VARCHAR(50),
    @Id             INT = NULL,
    @Name           NVARCHAR(100) = NULL,
    @IsDeMinimis    BIT = 0,
    @TaxExemptLimit DECIMAL(18,2) = 0,
    @CreatedBy      NVARCHAR(256) = NULL,
    @UpdatedBy      NVARCHAR(256) = NULL,
    @DeletedBy      NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET_ALL'
    BEGIN
        BEGIN TRY
            SELECT * FROM AllowanceTypes WHERE IsDeleted = 0 ORDER BY Name;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_BY_ID'
    BEGIN
        BEGIN TRY
            SELECT * FROM AllowanceTypes WHERE Id = @Id AND IsDeleted = 0;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'CREATE'
    BEGIN
        BEGIN TRY
            IF EXISTS (SELECT 1 FROM AllowanceTypes WHERE Name = @Name AND IsDeleted = 0)
            BEGIN RAISERROR('Allowance type name already exists.', 16, 1); RETURN; END

            INSERT INTO AllowanceTypes (Name, IsDeMinimis, TaxExemptLimit, CreatedAt, CreatedBy)
            VALUES (@Name, @IsDeMinimis, @TaxExemptLimit, GETDATE(), @CreatedBy);

            SELECT * FROM AllowanceTypes WHERE Id = SCOPE_IDENTITY();
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'UPDATE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM AllowanceTypes WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Allowance type not found.', 16, 1); RETURN; END

            IF EXISTS (SELECT 1 FROM AllowanceTypes WHERE Name = @Name AND Id <> @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Allowance type name already exists.', 16, 1); RETURN; END

            UPDATE AllowanceTypes SET Name=@Name, IsDeMinimis=@IsDeMinimis, TaxExemptLimit=@TaxExemptLimit,
                UpdatedBy=@UpdatedBy, UpdatedAt=GETDATE()
            WHERE Id = @Id;

            SELECT * FROM AllowanceTypes WHERE Id = @Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'DELETE'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM AllowanceTypes WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Allowance type not found.', 16, 1); RETURN; END

            UPDATE AllowanceTypes SET IsDeleted=1, DeletedAt=GETDATE(), DeletedBy=@DeletedBy WHERE Id=@Id;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_AllowanceType: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- 14. sp_Lookup
-- ============================================================================
CREATE PROCEDURE dbo.sp_Lookup
    @ActionType   VARCHAR(50),
    @DepartmentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET_DEPARTMENTS'
    BEGIN
        BEGIN TRY
            SELECT Id, DepartmentCode, DepartmentName, Description, ManagerId
            FROM Departments WHERE IsDeleted = 0 ORDER BY DepartmentName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_POSITIONS'
    BEGIN
        BEGIN TRY
            SELECT Id, PositionCode, PositionTitle, DepartmentId, MinSalary, MaxSalary
            FROM Positions
            WHERE IsDeleted = 0
              AND (@DepartmentId IS NULL OR DepartmentId = @DepartmentId)
            ORDER BY PositionTitle;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_BRANCHES'
    BEGIN
        BEGIN TRY
            SELECT Id, BranchCode, BranchName, Address, IsHeadOffice
            FROM Branches WHERE IsDeleted = 0 ORDER BY BranchName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_Lookup: %s', 16, 1, @ActionType);
END
GO

-- ============================================================================
-- END OF CONSOLIDATED STORED PROCEDURES
-- ============================================================================
