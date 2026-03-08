EXEC sp_Attendance
  @ActionType = 'IMPORT',
  @PeriodId = 4,
  @RowsJson = N'[{"EmployeeCode":"EMP-0010","DaysWorked":10,"TotalDays":10,"LateHours":0,"UndertimeHours":0,"OtHours":0,"NightDiffHours":0},{"EmployeeCode":"EMP-0009","DaysWorked":10,"TotalDays":10,"LateHours":0,"UndertimeHours":0,"OtHours":0,"NightDiffHours":0}]',
  @CreatedBy = 'test';
GO
