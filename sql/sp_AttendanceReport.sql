CREATE OR ALTER PROCEDURE sp_AttendanceReport
    @ActionType       VARCHAR(50),
    @StartDate        DATE         = NULL,
    @EndDate          DATE         = NULL,
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
            ad.Remarks
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND ad.Date >= @StartDate AND ad.Date <= @EndDate
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
            ad.UndertimeHours
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND ad.Date >= @StartDate AND ad.Date <= @EndDate
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
            ad.Remarks
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND ad.Date >= @StartDate AND ad.Date <= @EndDate
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
            ad.Status
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND ad.Date >= @StartDate AND ad.Date <= @EndDate
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
            ISNULL(SUM(CASE WHEN ad.Status NOT IN ('Absent','Rest Day') THEN 1 ELSE 0 END), 0) AS DaysWorked,
            ISNULL(SUM(CASE WHEN ad.Status <> 'Rest Day' THEN 1 ELSE 0 END), 0) AS TotalDays,
            ISNULL(SUM(ad.LateHours), 0) AS TotalLateHours,
            ISNULL(SUM(ad.UndertimeHours), 0) AS TotalUndertimeHours,
            ISNULL(SUM(ad.OtHours), 0) AS TotalOtHours,
            ISNULL(SUM(ad.NightDiffHours), 0) AS TotalNightDiffHours,
            ISNULL(SUM(CASE WHEN ad.Status IN ('Absent') THEN 1 ELSE 0 END), 0) AS DaysAbsent,
            CASE
                WHEN SUM(CASE WHEN ad.Status <> 'Rest Day' THEN 1 ELSE 0 END) > 0
                THEN CAST(SUM(CASE WHEN ad.Status NOT IN ('Absent','Rest Day') THEN 1 ELSE 0 END) * 100.0
                     / SUM(CASE WHEN ad.Status <> 'Rest Day' THEN 1 ELSE 0 END) AS DECIMAL(5,2))
                ELSE 0
            END AS AttendanceRate,
            CASE
                WHEN SUM(CASE WHEN ad.LateHours > 0 THEN 1 ELSE 0 END) > 0 THEN 'Late'
                ELSE 'Ok'
            END AS Status
        FROM AttendanceDetails ad
        INNER JOIN Attendances a     ON a.Id = ad.AttendanceId AND a.IsDeleted = 0
        INNER JOIN Employees e       ON e.Id = a.EmployeeId AND e.IsDeleted = 0
        LEFT  JOIN Departments d     ON d.Id = e.DepartmentId AND d.IsDeleted = 0
        LEFT  JOIN Branches b        ON b.Id = e.BranchId AND b.IsDeleted = 0
        WHERE ad.IsDeleted = 0
          AND ad.Date >= @StartDate AND ad.Date <= @EndDate
          AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
          AND (@BranchId     IS NULL OR e.BranchId     = @BranchId)
          AND (@EmployeeId   IS NULL OR e.Id           = @EmployeeId)
        GROUP BY e.Id, e.EmployeeCode, e.FirstName, e.MiddleName, e.LastName, e.Suffix,
                 d.DepartmentName, b.BranchName
        ORDER BY d.DepartmentName, e.LastName, e.FirstName;
    END
END
