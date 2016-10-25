CREATE FUNCTION [dbo].[HasIncompleteRegistrations]
(	
	@prog INT
	,@div INT 
	,@org INT 
	,@begdt DATETIME
	,@enddt DATETIME
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT  PeopleId
	FROM dbo.RecentIncompleteRegistrations(@prog, @div, @org, @begdt, @enddt) r
	WHERE EXISTS(SELECT NULL FROM dbo.People WHERE PeopleId = r.PeopleId)
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
