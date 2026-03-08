-- Create a test schedule
EXEC sp_WorkSchedule
    @ActionType = 'CREATE',
    @Name = N'Test Schedule',
    @Description = N'Test',
    @IsDefault = 0,
    @DaysJson = N'[{"DayOfWeek":1,"IsRestDay":false,"ShiftStart":"08:00","ShiftEnd":"17:00","BreakStart":"12:00","BreakEnd":"13:00"},{"DayOfWeek":0,"IsRestDay":true,"ShiftStart":null,"ShiftEnd":null,"BreakStart":null,"BreakEnd":null}]',
    @CreatedBy = N'test';

PRINT '--- GET_ALL ---';
EXEC sp_WorkSchedule @ActionType = 'GET_ALL';
