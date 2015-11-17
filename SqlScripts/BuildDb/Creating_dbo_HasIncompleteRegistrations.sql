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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
