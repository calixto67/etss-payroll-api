-- Add GET_EMPLOYEE_SCHEDULE and REPROCESS_DETAILS action types to sp_Attendance
-- Uses CREATE OR ALTER to replace the entire SP

CREATE OR ALTER PROCEDURE dbo.sp_Attendance
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

    -- GET_BY_PERIOD
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

    -- GET_BY_ID
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

    -- CREATE
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

    -- UPDATE
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

    -- RESOLVE
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

    -- DELETE
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

    -- GET_DETAILS
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

    -- UPDATE_DETAILS
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

    -- IMPORT
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

    -- IMPORT_RAW_PUNCHES
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
            DECLARE @RpAllowND BIT = 1, @RpAllowOT BIT = 1;

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

                    -- Read per-schedule ND/OT flags and rule overrides
                    SET @RpAllowND = 1;
                    SET @RpAllowOT = 1;
                    IF @RpSchedId IS NOT NULL
                    BEGIN
                        SELECT
                            @RpAllowND = ISNULL(AllowNightDifferential, 1),
                            @RpAllowOT = ISNULL(AllowOvertime, 1),
                            @RpGracePeriod = ISNULL(GracePeriodMinutes, @RpGracePeriod),
                            @RpOTStartAfter = ISNULL(OTStartAfterMinutes, @RpOTStartAfter),
                            @RpOTMinimum = ISNULL(OTMinimumMinutes, @RpOTMinimum),
                            @RpNDStart = ISNULL(NightDiffStartTime, @RpNDStart),
                            @RpNDEnd = ISNULL(NightDiffEndTime, @RpNDEnd)
                        FROM WorkSchedules
                        WHERE Id = @RpSchedId AND IsDeleted = 0;
                    END

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

                                -- Only calculate OT if schedule allows overtime
                                IF @RpAllowOT = 1 AND @RpTimeOut IS NOT NULL AND @RpTimeOut > DATEADD(MINUTE, @RpOTStartAfter, @RpShiftEnd)
                                BEGIN
                                    DECLARE @RpOTCandidate DECIMAL(18,2) = DATEDIFF(MINUTE, @RpShiftEnd, @RpTimeOut);
                                    IF @RpOTCandidate >= @RpOTMinimum
                                        SET @RpOTMins = @RpOTCandidate;
                                END

                                -- Only calculate ND if schedule allows night differential
                                IF @RpAllowND = 1 AND @RpTimeOut IS NOT NULL AND @RpTimeOut > @RpNDStart
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
    -- GET_EMPLOYEE_SCHEDULE - returns schedule day info for an employee
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_EMPLOYEE_SCHEDULE'
    BEGIN
        BEGIN TRY
            DECLARE @GesSchedId INT = NULL;

            SELECT TOP 1 @GesSchedId = WorkScheduleId
            FROM EmployeeSchedules
            WHERE EmployeeId = @EmployeeId AND EndDate IS NULL AND IsDeleted = 0
            ORDER BY EffectiveDate DESC;

            IF @GesSchedId IS NULL
                SELECT TOP 1 @GesSchedId = Id FROM WorkSchedules WHERE IsDefault = 1 AND IsDeleted = 0;

            IF @GesSchedId IS NULL
            BEGIN
                -- Return empty result set with correct shape
                SELECT
                    CAST(0 AS INT) AS DayOfWeek,
                    CAST('08:00' AS NVARCHAR(5)) AS ShiftStart,
                    CAST('17:00' AS NVARCHAR(5)) AS ShiftEnd,
                    CAST(0 AS BIT) AS IsRestDay,
                    CAST(15 AS INT) AS GracePeriodMinutes,
                    CAST(0 AS INT) AS OTStartAfterMinutes,
                    CAST(30 AS INT) AS OTMinimumMinutes,
                    CAST('22:00' AS NVARCHAR(5)) AS NightDiffStartTime,
                    CAST('06:00' AS NVARCHAR(5)) AS NightDiffEndTime
                WHERE 1 = 0;
                RETURN;
            END

            SELECT
                wsd.DayOfWeek,
                LEFT(CONVERT(VARCHAR, wsd.ShiftStart, 108), 5) AS ShiftStart,
                LEFT(CONVERT(VARCHAR, wsd.ShiftEnd, 108), 5) AS ShiftEnd,
                wsd.IsRestDay,
                ws.GracePeriodMinutes,
                ws.OTStartAfterMinutes,
                ws.OTMinimumMinutes,
                LEFT(CONVERT(VARCHAR, ws.NightDiffStartTime, 108), 5) AS NightDiffStartTime,
                LEFT(CONVERT(VARCHAR, ws.NightDiffEndTime, 108), 5) AS NightDiffEndTime
            FROM WorkScheduleDays wsd
            INNER JOIN WorkSchedules ws ON ws.Id = wsd.WorkScheduleId
            WHERE wsd.WorkScheduleId = @GesSchedId AND wsd.IsDeleted = 0
            ORDER BY wsd.DayOfWeek;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- REPROCESS_DETAILS - recalculates late/UT/OT/ND from existing TimeIn/TimeOut
    -- using the employee's current schedule rules and AllowND/AllowOT flags
    -- -----------------------------------------------------------------------
    IF @ActionType = 'REPROCESS_DETAILS'
    BEGIN
        BEGIN TRY
            DECLARE @RepAttId INT = @Id;
            DECLARE @RepEmpId INT;

            SELECT @RepEmpId = EmployeeId FROM Attendances WHERE Id = @RepAttId AND IsDeleted = 0;
            IF @RepEmpId IS NULL
            BEGIN RAISERROR('Attendance record not found.', 16, 1); RETURN; END

            -- Get employee schedule
            DECLARE @RepSchedId INT = NULL;
            SELECT TOP 1 @RepSchedId = WorkScheduleId
            FROM EmployeeSchedules
            WHERE EmployeeId = @RepEmpId AND EndDate IS NULL AND IsDeleted = 0
            ORDER BY EffectiveDate DESC;

            IF @RepSchedId IS NULL
                SELECT TOP 1 @RepSchedId = Id FROM WorkSchedules WHERE IsDefault = 1 AND IsDeleted = 0;

            -- Load schedule rules with defaults
            DECLARE @RepGrace INT = 15, @RepOTAfter INT = 0, @RepOTMin INT = 30;
            DECLARE @RepNDStart TIME = '22:00:00', @RepNDEnd TIME = '06:00:00';
            DECLARE @RepAllowND BIT = 1, @RepAllowOT BIT = 1;
            DECLARE @RepDefShiftStart TIME = '08:00:00', @RepDefShiftEnd TIME = '17:00:00';

            -- Load from global ScheduleRules first
            SELECT TOP 1
                @RepGrace = GracePeriodMinutes,
                @RepOTAfter = OTStartAfterMinutes,
                @RepOTMin = OTMinimumMinutes,
                @RepNDStart = NightDiffStartTime,
                @RepNDEnd = NightDiffEndTime
            FROM ScheduleRules WHERE IsDeleted = 0;

            -- Override with per-schedule settings
            IF @RepSchedId IS NOT NULL
            BEGIN
                SELECT
                    @RepAllowND = ISNULL(AllowNightDifferential, 1),
                    @RepAllowOT = ISNULL(AllowOvertime, 1),
                    @RepGrace = ISNULL(GracePeriodMinutes, @RepGrace),
                    @RepOTAfter = ISNULL(OTStartAfterMinutes, @RepOTAfter),
                    @RepOTMin = ISNULL(OTMinimumMinutes, @RepOTMin),
                    @RepNDStart = ISNULL(NightDiffStartTime, @RepNDStart),
                    @RepNDEnd = ISNULL(NightDiffEndTime, @RepNDEnd)
                FROM WorkSchedules
                WHERE Id = @RepSchedId AND IsDeleted = 0;
            END

            BEGIN TRANSACTION;

            -- Cursor through each attendance detail and recalculate
            DECLARE @RepDetId INT, @RepDate DATE, @RepTimeIn TIME, @RepTimeOut TIME, @RepDetStatus NVARCHAR(50);

            DECLARE rep_cursor CURSOR LOCAL FAST_FORWARD FOR
                SELECT Id, [Date], TimeIn, TimeOut, Status
                FROM AttendanceDetails
                WHERE AttendanceId = @RepAttId AND IsDeleted = 0;

            OPEN rep_cursor;
            FETCH NEXT FROM rep_cursor INTO @RepDetId, @RepDate, @RepTimeIn, @RepTimeOut, @RepDetStatus;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                -- Skip non-present days
                IF @RepDetStatus NOT IN ('Rest Day', 'Absent') AND (@RepTimeIn IS NOT NULL OR @RepTimeOut IS NOT NULL)
                BEGIN
                    DECLARE @RepDOW INT = DATEPART(WEEKDAY, @RepDate) - 1;
                    DECLARE @RepShiftStart TIME = @RepDefShiftStart, @RepShiftEnd TIME = @RepDefShiftEnd;

                    IF @RepSchedId IS NOT NULL
                    BEGIN
                        SELECT @RepShiftStart = ISNULL(ShiftStart, @RepDefShiftStart),
                               @RepShiftEnd = ISNULL(ShiftEnd, @RepDefShiftEnd)
                        FROM WorkScheduleDays
                        WHERE WorkScheduleId = @RepSchedId AND DayOfWeek = @RepDOW AND IsDeleted = 0;
                    END

                    DECLARE @RepLate DECIMAL(18,2) = 0, @RepUT DECIMAL(18,2) = 0;
                    DECLARE @RepOT DECIMAL(18,2) = 0, @RepND DECIMAL(18,2) = 0;

                    -- Late
                    IF @RepTimeIn IS NOT NULL AND @RepTimeIn > DATEADD(MINUTE, @RepGrace, @RepShiftStart)
                        SET @RepLate = ROUND(DATEDIFF(MINUTE, @RepShiftStart, @RepTimeIn) / 60.0, 2);

                    -- Undertime
                    IF @RepTimeOut IS NOT NULL AND @RepTimeOut < @RepShiftEnd
                        SET @RepUT = ROUND(DATEDIFF(MINUTE, @RepTimeOut, @RepShiftEnd) / 60.0, 2);

                    -- OT (only if allowed)
                    IF @RepAllowOT = 1 AND @RepTimeOut IS NOT NULL AND @RepTimeOut > DATEADD(MINUTE, @RepOTAfter, @RepShiftEnd)
                    BEGIN
                        DECLARE @RepOTCand DECIMAL(18,2) = DATEDIFF(MINUTE, @RepShiftEnd, @RepTimeOut);
                        IF @RepOTCand >= @RepOTMin
                            SET @RepOT = ROUND(@RepOTCand / 60.0, 2);
                    END

                    -- ND (only if allowed)
                    IF @RepAllowND = 1 AND @RepTimeOut IS NOT NULL AND @RepTimeOut > @RepNDStart
                        SET @RepND = ROUND(DATEDIFF(MINUTE, @RepNDStart, @RepTimeOut) / 60.0, 2);

                    UPDATE AttendanceDetails SET
                        LateHours = @RepLate,
                        UndertimeHours = @RepUT,
                        OtHours = @RepOT,
                        NightDiffHours = @RepND,
                        UpdatedBy = @UpdatedBy,
                        UpdatedAt = GETDATE()
                    WHERE Id = @RepDetId;
                END

                FETCH NEXT FROM rep_cursor INTO @RepDetId, @RepDate, @RepTimeIn, @RepTimeOut, @RepDetStatus;
            END

            CLOSE rep_cursor;
            DEALLOCATE rep_cursor;

            -- Recalculate header totals
            UPDATE Attendances SET
                LateHours      = ISNULL((SELECT SUM(d.LateHours) FROM AttendanceDetails d WHERE d.AttendanceId=@RepAttId AND d.IsDeleted=0), 0),
                UndertimeHours = ISNULL((SELECT SUM(d.UndertimeHours) FROM AttendanceDetails d WHERE d.AttendanceId=@RepAttId AND d.IsDeleted=0), 0),
                OtHours        = ISNULL((SELECT SUM(d.OtHours) FROM AttendanceDetails d WHERE d.AttendanceId=@RepAttId AND d.IsDeleted=0), 0),
                NightDiffHours = ISNULL((SELECT SUM(d.NightDiffHours) FROM AttendanceDetails d WHERE d.AttendanceId=@RepAttId AND d.IsDeleted=0), 0),
                DaysWorked     = ISNULL((SELECT COUNT(*) FROM AttendanceDetails d WHERE d.AttendanceId=@RepAttId AND d.IsDeleted=0 AND d.Status NOT IN ('Absent','Rest Day')), 0),
                UpdatedBy = @UpdatedBy,
                UpdatedAt = GETDATE()
            WHERE Id = @RepAttId;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- CHECK_SCHEDULES
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
