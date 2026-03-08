CREATE OR ALTER PROCEDURE dbo.sp_PayPeriod
    @ActionType  VARCHAR(50),
    @Id          INT = NULL,
    @Year        INT = NULL,
    @Status      NVARCHAR(50) = NULL,
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

    -- Map integer status values to string names for backward compatibility
    IF @Status IS NOT NULL AND ISNUMERIC(@Status) = 1
    BEGIN
        SET @Status = CASE CAST(@Status AS INT)
            WHEN 1 THEN 'Draft'
            WHEN 2 THEN 'Open'
            WHEN 3 THEN 'Computed'
            WHEN 4 THEN 'Approved'
            WHEN 5 THEN 'Locked'
            WHEN 6 THEN 'Exported'
            ELSE @Status
        END;
    END

    -- -----------------------------------------------------------------------
    -- GET_ALL
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_ALL'
    BEGIN
        SELECT pp.Id, pp.Name, pp.PeriodCode, pp.StartDate, pp.EndDate, pp.PayDate,
               pp.PeriodType, pp.Status, pp.IsClosed, pp.CreatedAt,
               (SELECT COUNT(*) FROM PayrollRecords pr WHERE pr.PayrollPeriodId = pp.Id AND pr.IsDeleted = 0) AS EmployeeCount
        FROM PayrollPeriods pp
        WHERE pp.IsDeleted = 0
          AND (@Year IS NULL OR YEAR(pp.StartDate) = @Year)
          AND (@Status IS NULL OR pp.Status = @Status)
        ORDER BY pp.StartDate DESC;
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- GET_BY_ID
    -- -----------------------------------------------------------------------
    IF @ActionType = 'GET_BY_ID'
    BEGIN
        SELECT pp.Id, pp.Name, pp.PeriodCode, pp.StartDate, pp.EndDate, pp.PayDate,
               pp.PeriodType, pp.Status, pp.IsClosed, pp.CreatedAt,
               (SELECT COUNT(*) FROM PayrollRecords pr WHERE pr.PayrollPeriodId = pp.Id AND pr.IsDeleted = 0) AS EmployeeCount
        FROM PayrollPeriods pp
        WHERE pp.Id = @Id AND pp.IsDeleted = 0;
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- CREATE
    -- -----------------------------------------------------------------------
    IF @ActionType = 'CREATE'
    BEGIN
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

        SELECT pp.Id, pp.Name, pp.PeriodCode, pp.StartDate, pp.EndDate, pp.PayDate,
               pp.PeriodType, pp.Status, pp.IsClosed, pp.CreatedAt, 0 AS EmployeeCount
        FROM PayrollPeriods pp WHERE pp.Id = SCOPE_IDENTITY();
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- UPDATE_STATUS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_STATUS'
    BEGIN
        DECLARE @UsCurStatus NVARCHAR(50);
        SELECT @UsCurStatus = Status FROM PayrollPeriods WHERE Id = @Id AND IsDeleted = 0;
        IF @UsCurStatus IS NULL BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END

        DECLARE @UsValid BIT = 0;
        IF @UsCurStatus = 'Draft'    AND @Status = 'Open'     SET @UsValid = 1;
        IF @UsCurStatus = 'Open'     AND @Status IN ('Computed','Draft') SET @UsValid = 1;
        IF @UsCurStatus = 'Computed' AND @Status IN ('Approved','Open')  SET @UsValid = 1;
        IF @UsCurStatus = 'Approved' AND @Status IN ('Locked','Computed') SET @UsValid = 1;
        IF @UsCurStatus = 'Locked'   AND @Status IN ('Exported','Approved') SET @UsValid = 1;

        IF @UsValid = 0
        BEGIN
            DECLARE @UsMsg NVARCHAR(200) = 'Cannot change status from ' + @UsCurStatus + ' to ' + @Status + '.';
            RAISERROR(@UsMsg, 16, 1);
            RETURN;
        END

        UPDATE PayrollPeriods
        SET Status = @Status,
            IsClosed = CASE WHEN @Status IN ('Locked','Exported') THEN 1 ELSE 0 END,
            UpdatedAt = GETDATE(),
            UpdatedBy = @UpdatedBy
        WHERE Id = @Id;

        SELECT pp.Id, pp.Name, pp.PeriodCode, pp.StartDate, pp.EndDate, pp.PayDate,
               pp.PeriodType, pp.Status, pp.IsClosed, pp.CreatedAt,
               (SELECT COUNT(*) FROM PayrollRecords pr WHERE pr.PayrollPeriodId = pp.Id AND pr.IsDeleted = 0) AS EmployeeCount
        FROM PayrollPeriods pp WHERE pp.Id = @Id;
        RETURN;
    END

    -- -----------------------------------------------------------------------
    -- DELETE (soft)
    -- -----------------------------------------------------------------------
    IF @ActionType = 'DELETE'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM PayrollPeriods WHERE Id = @Id AND IsDeleted = 0)
        BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END

        UPDATE PayrollPeriods
        SET IsDeleted = 1, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
        WHERE Id = @Id;
        RETURN;
    END
END
