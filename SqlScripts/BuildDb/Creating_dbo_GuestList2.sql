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
IF @@ERROR <> 0 SET NOEXEC ON
GO
