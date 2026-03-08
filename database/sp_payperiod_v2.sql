CREATE OR ALTER PROCEDURE dbo.sp_PayPeriod
    @ActionType  VARCHAR(50),
    @Id          INT = NULL,
    @Year        INT = NULL,
    @Status      NVARCHAR(20) = NULL,
    @StartDate   DATE = NULL,
    @EndDate     DATE = NULL,
    @PayDate     DATE = NULL,
    @PeriodType  INT = NULL,
    @DeductSss       BIT = NULL,
    @DeductPhilHealth BIT = NULL,
    @DeductPagIbig   BIT = NULL,
    @CreatedBy   NVARCHAR(256) = NULL,
    @UpdatedBy   NVARCHAR(256) = NULL,
    @DeletedBy   NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

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

            -- For weekly periods, use a more specific code
            IF @PeriodType = 3
                SET @CrPeriodCode = FORMAT(@StartDate, 'yyyy-MM-dd');

            IF EXISTS (SELECT 1 FROM PayrollPeriods WHERE PeriodCode = @CrPeriodCode AND IsDeleted = 0)
            BEGIN RAISERROR('Period code %s already exists.', 16, 1, @CrPeriodCode); RETURN; END

            -- Auto-generate period name
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

            -- Auto-set contribution flags from CompanySettings if not explicitly provided
            IF @DeductSss IS NULL OR @DeductPhilHealth IS NULL OR @DeductPagIbig IS NULL
            BEGIN
                DECLARE @TimingSss VARCHAR(20), @TimingPH VARCHAR(20), @TimingPI VARCHAR(20);
                SELECT @TimingSss = ContributionTimingSss,
                       @TimingPH = ContributionTimingPhilHealth,
                       @TimingPI = ContributionTimingPagIbig
                FROM CompanySettings;

                DECLARE @IsFirst BIT = CASE WHEN DAY(@StartDate) <= 15 THEN 1 ELSE 0 END;

                IF @DeductSss IS NULL
                    SET @DeductSss = CASE
                        WHEN @PeriodType = 2 THEN 1  -- Monthly: always deduct
                        WHEN @TimingSss = 'split' THEN 1
                        WHEN @TimingSss = '1st_half' AND @IsFirst = 1 THEN 1
                        WHEN @TimingSss = '2nd_half' AND @IsFirst = 0 THEN 1
                        ELSE 0
                    END;

                IF @DeductPhilHealth IS NULL
                    SET @DeductPhilHealth = CASE
                        WHEN @PeriodType = 2 THEN 1
                        WHEN @TimingPH = 'split' THEN 1
                        WHEN @TimingPH = '1st_half' AND @IsFirst = 1 THEN 1
                        WHEN @TimingPH = '2nd_half' AND @IsFirst = 0 THEN 1
                        ELSE 0
                    END;

                IF @DeductPagIbig IS NULL
                    SET @DeductPagIbig = CASE
                        WHEN @PeriodType = 2 THEN 1
                        WHEN @TimingPI = 'split' THEN 1
                        WHEN @TimingPI = '1st_half' AND @IsFirst = 1 THEN 1
                        WHEN @TimingPI = '2nd_half' AND @IsFirst = 0 THEN 1
                        ELSE 0
                    END;
            END

            INSERT INTO PayrollPeriods (Name, PeriodCode, StartDate, EndDate, PayDate, PeriodType, Status, IsClosed,
                DeductSss, DeductPhilHealth, DeductPagIbig, CreatedAt, CreatedBy)
            VALUES (@CrPeriodName, @CrPeriodCode, @StartDate, @EndDate, @PayDate, @PeriodType, 'Draft', 0,
                @DeductSss, @DeductPhilHealth, @DeductPagIbig, GETDATE(), @CreatedBy);

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
    -- UPDATE_CONTRIBUTION_FLAGS
    -- -----------------------------------------------------------------------
    IF @ActionType = 'UPDATE_CONTRIBUTION_FLAGS'
    BEGIN
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM PayrollPeriods WHERE Id = @Id AND IsDeleted = 0)
            BEGIN RAISERROR('Payroll period not found.', 16, 1); RETURN; END

            UPDATE PayrollPeriods
            SET DeductSss = ISNULL(@DeductSss, DeductSss),
                DeductPhilHealth = ISNULL(@DeductPhilHealth, DeductPhilHealth),
                DeductPagIbig = ISNULL(@DeductPagIbig, DeductPagIbig),
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

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC usp_LogError
            @ModuleName      = 'PayPeriod',
            @ProcedureName   = 'sp_PayPeriod',
            @ActionType      = @ActionType;
        THROW;
    END CATCH
END
