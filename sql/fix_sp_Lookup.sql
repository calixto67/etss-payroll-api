CREATE OR ALTER PROCEDURE dbo.sp_Lookup
    @ActionType   VARCHAR(50),
    @DepartmentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ActionType = 'GET_DEPARTMENTS'
    BEGIN
        BEGIN TRY
            SELECT Id, DepartmentCode AS Code, DepartmentName AS Name
            FROM Departments WHERE IsDeleted = 0 ORDER BY DepartmentName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    IF @ActionType = 'GET_POSITIONS'
    BEGIN
        BEGIN TRY
            SELECT Id, PositionCode AS Code, PositionTitle AS Title, DepartmentId
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
            SELECT Id, BranchCode AS Code, BranchName AS Name
            FROM Branches WHERE IsDeleted = 0 ORDER BY BranchName;
        END TRY
        BEGIN CATCH THROW; END CATCH
        RETURN;
    END

    RAISERROR('Invalid @ActionType for sp_Lookup: %s', 16, 1, @ActionType);
END
GO
