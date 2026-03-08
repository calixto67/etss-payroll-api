-- Patch: Add duplicate check to IMPORT action in sp_Attendance
-- This script extracts the full SP, replaces the IMPORT block, and re-creates it.

DECLARE @def NVARCHAR(MAX) = CAST(OBJECT_DEFINITION(OBJECT_ID('sp_Attendance')) AS NVARCHAR(MAX));

-- Find the old IMPORT block: from "IF @ActionType = 'IMPORT'" to its "RETURN;"
DECLARE @blockStart INT = CHARINDEX('IF @ActionType = ''IMPORT''', @def);
DECLARE @returnSearchStart INT = @blockStart + 100;
-- Find the RETURN; that closes this block (after END CATCH)
DECLARE @returnEnd INT = CHARINDEX('RETURN;', @def, @returnSearchStart);
-- Move past "RETURN;" + the trailing "    END" that closes the outer IF block
SET @returnEnd = @returnEnd + LEN('RETURN;');
-- Skip whitespace and the closing END
DECLARE @endPos INT = CHARINDEX('END', @def, @returnEnd);
IF @endPos > 0 AND @endPos - @returnEnd < 20
    SET @returnEnd = @endPos + LEN('END');

DECLARE @oldLen INT = @returnEnd - @blockStart;

-- New IMPORT block with duplicate validation
DECLARE @newBlock NVARCHAR(MAX) = N'IF @ActionType = ''IMPORT''
    BEGIN
        -- Check for duplicates BEFORE starting transaction
        DECLARE @DupCodes NVARCHAR(MAX) = NULL;

        SELECT JSON_VALUE(j.value, ''$.EmployeeCode'') AS EmployeeCode
        INTO #ImpCheckRows
        FROM OPENJSON(@RowsJson) j;

        SELECT @DupCodes = STRING_AGG(ic.EmployeeCode, '', '')
        FROM #ImpCheckRows ic
        INNER JOIN Employees e ON e.EmployeeCode = ic.EmployeeCode AND e.IsDeleted = 0
        INNER JOIN Attendances a ON a.EmployeeId = e.Id AND a.PayrollPeriodId = @PeriodId AND a.IsDeleted = 0;

        DROP TABLE #ImpCheckRows;

        IF @DupCodes IS NOT NULL
        BEGIN
            DECLARE @DupMsg NVARCHAR(500) = N''Attendance records already exist for: '' + @DupCodes;
            RAISERROR(@DupMsg, 16, 1);
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;
            DECLARE @ImpCreatedCount INT = 0;

            SELECT
                JSON_VALUE(j.value, ''$.EmployeeCode'') AS EmployeeCode,
                ISNULL(CAST(JSON_VALUE(j.value, ''$.DaysWorked'') AS DECIMAL(18,2)), 0) AS DaysWorked,
                ISNULL(CAST(JSON_VALUE(j.value, ''$.TotalDays'') AS DECIMAL(18,2)), 0) AS TotalDays,
                ISNULL(CAST(JSON_VALUE(j.value, ''$.LateHours'') AS DECIMAL(18,2)), 0) AS LateHours,
                ISNULL(CAST(JSON_VALUE(j.value, ''$.UndertimeHours'') AS DECIMAL(18,2)), 0) AS UndertimeHours,
                ISNULL(CAST(JSON_VALUE(j.value, ''$.OtHours'') AS DECIMAL(18,2)), 0) AS OtHours,
                ISNULL(CAST(JSON_VALUE(j.value, ''$.NightDiffHours'') AS DECIMAL(18,2)), 0) AS NightDiffHours,
                JSON_QUERY(j.value, ''$.Details'') AS DetailsJson,
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

                IF @ImpEId IS NOT NULL
                BEGIN
                    DECLARE @ImpAS INT = CASE WHEN @ImpDW=0 THEN 3 WHEN @ImpDW<@ImpTD THEN 2 ELSE 1 END;
                    INSERT INTO Attendances (PayrollPeriodId,EmployeeId,DaysWorked,TotalDays,LateHours,UndertimeHours,OtHours,NightDiffHours,Status,IsDeleted,CreatedAt,CreatedBy)
                    VALUES (@PeriodId,@ImpEId,@ImpDW,@ImpTD,@ImpLH,@ImpUH,@ImpOH,@ImpNH,@ImpAS,0,GETDATE(),@CreatedBy);
                    DECLARE @ImpAId INT = SCOPE_IDENTITY();

                    IF @ImpDJ IS NOT NULL AND LEN(@ImpDJ) > 2
                        INSERT INTO AttendanceDetails (AttendanceId,[Date],TimeIn,TimeOut,LateHours,UndertimeHours,OtHours,NightDiffHours,[Status],Remarks,IsDeleted,CreatedAt,CreatedBy)
                        SELECT @ImpAId, CAST(JSON_VALUE(d.value,''$.Date'') AS DATE), CAST(JSON_VALUE(d.value,''$.TimeIn'') AS TIME), CAST(JSON_VALUE(d.value,''$.TimeOut'') AS TIME),
                            ISNULL(CAST(JSON_VALUE(d.value,''$.LateHours'') AS DECIMAL(18,2)),0), ISNULL(CAST(JSON_VALUE(d.value,''$.UndertimeHours'') AS DECIMAL(18,2)),0),
                            ISNULL(CAST(JSON_VALUE(d.value,''$.OtHours'') AS DECIMAL(18,2)),0), ISNULL(CAST(JSON_VALUE(d.value,''$.NightDiffHours'') AS DECIMAL(18,2)),0),
                            ISNULL(JSON_VALUE(d.value,''$.Status''),''Present''), JSON_VALUE(d.value,''$.Remarks''), 0, GETDATE(), @CreatedBy
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
            IF OBJECT_ID(''tempdb..#ImportRows'') IS NOT NULL DROP TABLE #ImportRows;
            INSERT INTO ErrorLogs (ModuleName, ProcedureName, ActionType, ErrorNumber, ErrorMessage, ErrorSeverity, ErrorState, ErrorLine, ErrorProcedure, CreatedDate)
            VALUES (''Attendance'', ''sp_Attendance'', ''IMPORT'', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_SEVERITY(), ERROR_STATE(), ERROR_LINE(), ERROR_PROCEDURE(), GETDATE());
            THROW;
        END CATCH
        RETURN;
    END';

-- Replace old block with new
SET @def = STUFF(@def, @blockStart, @oldLen, @newBlock);

-- Ensure CREATE OR ALTER — handle varying whitespace like "CREATE   PROCEDURE"
IF CHARINDEX('CREATE OR ALTER', @def) = 0
BEGIN
    DECLARE @cpPos INT = PATINDEX('%CREATE%PROCEDURE%', @def);
    IF @cpPos > 0
    BEGIN
        -- Find where 'PROCEDURE' starts
        DECLARE @procPos INT = CHARINDEX('PROCEDURE', @def, @cpPos);
        -- Replace everything from CREATE to PROCEDURE with 'CREATE OR ALTER PROCEDURE'
        SET @def = STUFF(@def, @cpPos, @procPos - @cpPos + LEN('PROCEDURE'), 'CREATE OR ALTER PROCEDURE');
    END
END

EXEC sp_executesql @def;
PRINT 'sp_Attendance IMPORT block updated with duplicate check.';
GO
