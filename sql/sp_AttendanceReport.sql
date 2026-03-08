CREATE OR ALTER PROCEDURE sp_AttendanceReport
    @ActionType       VARCHAR(50),
    @PayrollPeriodId  INT          = NULL,
    @DepartmentId     INT          = NULL,
    @BranchId         INT          = NULL,
    @EmployeeId       INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- ── DAILY ATTENDANCE ────────────────────────────────────────
    IF @ActionType = 'DAILY'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            d.DepartmentName,
            b.BranchName,
            ad.Date,
            ad.TimeIn,
            ad.TimeOut,
            ad.LateHours,
            ad.UndertimeHours,
            ad.OtHours,
            ad.NightDiffHours,
            ad.Status,
            ad.Remarks,
            pp.Name           AS PeriodName
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = a.PayrollPeriodId
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND a.PayrollPeriodId = @PayrollPeriodId
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY e.LastName, e.FirstName, ad.Date;
    END

    -- ── TARDINESS & UNDERTIME ───────────────────────────────────
    ELSE IF @ActionType = 'TARDINESS'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            d.DepartmentName,
            b.BranchName,
            ad.Date,
            ad.TimeIn,
            ad.TimeOut,
            ad.LateHours,
            ad.UndertimeHours,
            pp.Name           AS PeriodName
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = a.PayrollPeriodId
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND a.PayrollPeriodId = @PayrollPeriodId
          AND (ad.LateHours > 0 OR ad.UndertimeHours > 0)
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY e.LastName, e.FirstName, ad.Date;
    END

    -- ── ABSENTEEISM ─────────────────────────────────────────────
    ELSE IF @ActionType = 'ABSENTEEISM'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            d.DepartmentName,
            b.BranchName,
            ad.Date,
            ad.Status,
            ad.Remarks,
            pp.Name           AS PeriodName
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = a.PayrollPeriodId
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND a.PayrollPeriodId = @PayrollPeriodId
          AND ad.Status IN ('A', 'AWOL', 'Absent')
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY e.LastName, e.FirstName, ad.Date;
    END

    -- ── OVERTIME ────────────────────────────────────────────────
    ELSE IF @ActionType = 'OVERTIME'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            d.DepartmentName,
            b.BranchName,
            ad.Date,
            ad.TimeIn,
            ad.TimeOut,
            ad.OtHours,
            ad.NightDiffHours,
            ad.Status,
            pp.Name           AS PeriodName
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = a.PayrollPeriodId
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND a.PayrollPeriodId = @PayrollPeriodId
          AND ad.OtHours > 0
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY e.LastName, e.FirstName, ad.Date;
    END

    -- ── LEAVE USAGE ─────────────────────────────────────────────
    ELSE IF @ActionType = 'LEAVE_USAGE'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            lb.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            d.DepartmentName,
            b.BranchName,
            lb.LeaveType,
            lb.Entitlement,
            lb.Used,
            lb.Pending,
            lb.CarryOver,
            (lb.Entitlement + lb.CarryOver - lb.Used) AS Remaining
        FROM LeaveBalances lb
        INNER JOIN Employees e   ON lb.EmployeeCode = e.EmployeeCode AND e.IsDeleted = 0
        LEFT  JOIN Departments d ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b    ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE lb.IsDeleted = 0
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY e.LastName, e.FirstName, lb.LeaveType;
    END

    -- ── ATTENDANCE SUMMARY ──────────────────────────────────────
    ELSE IF @ActionType = 'SUMMARY'
    BEGIN
        SELECT
            e.Id              AS EmployeeId,
            e.EmployeeCode,
            CONCAT(e.FirstName, ' ', ISNULL(e.MiddleName + ' ', ''), e.LastName,
                   CASE WHEN e.Suffix IS NOT NULL AND e.Suffix <> '' THEN ' ' + e.Suffix ELSE '' END)
                              AS EmployeeName,
            d.DepartmentName,
            b.BranchName,
            a.DaysWorked,
            a.TotalDays,
            a.LateHours       AS TotalLateHours,
            a.UndertimeHours  AS TotalUndertimeHours,
            a.OtHours         AS TotalOtHours,
            a.NightDiffHours  AS TotalNightDiffHours,
            (a.TotalDays - a.DaysWorked) AS DaysAbsent,
            CASE WHEN a.TotalDays > 0 THEN CAST(a.DaysWorked * 100.0 / a.TotalDays AS DECIMAL(5,2)) ELSE 0 END AS AttendanceRate,
            a.Status,
            pp.Name           AS PeriodName
        FROM Attendances a
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        INNER JOIN PayrollPeriods pp ON pp.Id = a.PayrollPeriodId
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE a.IsDeleted = 0
          AND a.PayrollPeriodId = @PayrollPeriodId
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        ORDER BY d.DepartmentName, e.LastName, e.FirstName;
    END
END
