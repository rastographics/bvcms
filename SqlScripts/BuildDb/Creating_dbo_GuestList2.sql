CREATE FUNCTION [dbo].[GuestList2](
	@oid INT
	,@since DATETIME
	,@showHidden BIT
	)
RETURNS TABLE 
AS
RETURN 
(
	SELECT PeopleId, LastAttendDt, Hidden, MemberTypeId FROM dbo.GuestList(@oid, @since, @showHidden, NULL, NULL)
)

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
