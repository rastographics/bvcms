CREATE FUNCTION [dbo].[FirstMondayOfMonth] (@inputDate DATETIME)RETURNS DATETIME BEGIN     RETURN DATEADD(wk, DATEDIFF(wk, 0, dateadd(dd, 6 - datepart(day, @inputDate), @inputDate)), 0)  END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
