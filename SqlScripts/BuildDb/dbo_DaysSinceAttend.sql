-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[DaysSinceAttend](@pid INT, @oid INT)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @days int

	-- Add the T-SQL statements to compute the return value here
	SELECT @days = MAX(DATEDIFF(D,a.MeetingDate,GETDATE())) FROM dbo.Attend a
	JOIN dbo.Meetings m
	ON a.MeetingId = m.MeetingId
	WHERE a.PeopleId = @pid AND m.OrganizationId = @oid
	

	-- Return the result of the function
	RETURN @days

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
