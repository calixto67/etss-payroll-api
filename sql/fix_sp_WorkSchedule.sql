CREATE OR ALTER PROCEDURE dbo.sp_WorkSchedule
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

    -- ── GET_ALL: returns schedule + days joined (one row per day) ──
    IF @ActionType = 'GET_ALL'
    BEGIN
        BEGIN TRY
            SELECT ws.Id, ws.Name, ws.Description, ws.IsDefault,
                (SELECT COUNT(DISTINCT es.EmployeeId) FROM EmployeeSchedules es WHERE es.WorkScheduleId = ws.Id AND es.EndDate IS NULL AND es.IsDeleted = 0) AS EmployeeCount,
                wsd.Id AS DayId, wsd.DayOfWeek, wsd.IsRestDay, wsd.ShiftStart, wsd.ShiftEnd, wsd.BreakStart, wsd.BreakEnd
            FROM WorkSchedules ws
            LEFT JOIN WorkScheduleDays wsd ON wsd.WorkScheduleId = ws.Id AND wsd.IsDeleted = 0
            WHERE ws.IsDeleted = 0
            ORDER BY ws.Name, wsd.DayOfWeek;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- ── GET_BY_ID: same joined shape ──
    IF @ActionType = 'GET_BY_ID'
    BEGIN
        BEGIN TRY
            SELECT ws.Id, ws.Name, ws.Description, ws.IsDefault,
                (SELECT COUNT(DISTINCT es.EmployeeId) FROM EmployeeSchedules es WHERE es.WorkScheduleId = ws.Id AND es.EndDate IS NULL AND es.IsDeleted = 0) AS EmployeeCount,
                wsd.Id AS DayId, wsd.DayOfWeek, wsd.IsRestDay, wsd.ShiftStart, wsd.ShiftEnd, wsd.BreakStart, wsd.BreakEnd
            FROM WorkSchedules ws
            LEFT JOIN WorkScheduleDays wsd ON wsd.WorkScheduleId = ws.Id AND wsd.IsDeleted = 0
            WHERE ws.Id = @Id AND ws.IsDeleted = 0
            ORDER BY wsd.DayOfWeek;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- ── CREATE ──
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

            -- Return joined result
            SELECT ws.Id, ws.Name, ws.Description, ws.IsDefault,
                0 AS EmployeeCount,
                wsd.Id AS DayId, wsd.DayOfWeek, wsd.IsRestDay, wsd.ShiftStart, wsd.ShiftEnd, wsd.BreakStart, wsd.BreakEnd
            FROM WorkSchedules ws
            LEFT JOIN WorkScheduleDays wsd ON wsd.WorkScheduleId = ws.Id AND wsd.IsDeleted = 0
            WHERE ws.Id = @WsCrSchedId
            ORDER BY wsd.DayOfWeek;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- ── UPDATE ──
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

            -- Return joined result
            SELECT ws.Id, ws.Name, ws.Description, ws.IsDefault,
                (SELECT COUNT(DISTINCT es.EmployeeId) FROM EmployeeSchedules es WHERE es.WorkScheduleId = ws.Id AND es.EndDate IS NULL AND es.IsDeleted = 0) AS EmployeeCount,
                wsd.Id AS DayId, wsd.DayOfWeek, wsd.IsRestDay, wsd.ShiftStart, wsd.ShiftEnd, wsd.BreakStart, wsd.BreakEnd
            FROM WorkSchedules ws
            LEFT JOIN WorkScheduleDays wsd ON wsd.WorkScheduleId = ws.Id AND wsd.IsDeleted = 0
            WHERE ws.Id = @Id
            ORDER BY wsd.DayOfWeek;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH
        RETURN;
    END

    -- ── DELETE ──
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

    -- ── ASSIGN_EMPLOYEES ──
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

            SELECT es.Id,
                   e.Id AS EmployeeId,
                   e.EmployeeCode,
                   CONCAT(e.LastName, ', ', e.FirstName) AS EmployeeName,
                   es.WorkScheduleId,
                   ws.Name AS WorkScheduleName,
                   es.EffectiveDate,
                   es.EndDate
            FROM EmployeeSchedules es
            INNER JOIN Employees e ON e.Id = es.EmployeeId
            INNER JOIN WorkSchedules ws ON ws.Id = es.WorkScheduleId
            WHERE es.WorkScheduleId = @Id AND es.EndDate IS NULL AND es.IsDeleted = 0;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#WsAssignEmpIds') IS NOT NULL DROP TABLE #WsAssignEmpIds;
            THROW;
        END CATCH
        RETURN;
    END

    -- ── GET_EMPLOYEES ──
    IF @ActionType = 'GET_EMPLOYEES'
    BEGIN
        BEGIN TRY
            SELECT es.Id,
                   e.Id AS EmployeeId,
                   e.EmployeeCode,
                   CONCAT(e.LastName, ', ', e.FirstName) AS EmployeeName,
                   es.WorkScheduleId,
                   ws.Name AS WorkScheduleName,
                   es.EffectiveDate,
                   es.EndDate
            FROM EmployeeSchedules es
            INNER JOIN Employees e ON e.Id = es.EmployeeId
            INNER JOIN WorkSchedules ws ON ws.Id = es.WorkScheduleId
            WHERE es.WorkScheduleId = @Id AND es.EndDate IS NULL AND es.IsDeleted = 0
            ORDER BY e.LastName, e.FirstName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    -- ── UNASSIGN_EMPLOYEE ──
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
