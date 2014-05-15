-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[FirstMeetingDateLastLear] ( @pid INT )
RETURNS nvarchar(20)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @dt DATETIME

	-- Add the T-SQL statements to compute the return value here
SELECT TOP 1 @dt = MeetingDate FROM dbo.Attend
WHERE PeopleId = @pid
AND AttendanceFlag = 1
AND MeetingDate >= DATEADD(yy, -1, GETDATE())
ORDER BY MeetingDate
	-- Return the result of the function
	RETURN CONVERT(nvarchar, @dt, 111)

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
