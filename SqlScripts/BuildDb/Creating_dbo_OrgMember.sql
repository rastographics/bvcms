CREATE FUNCTION [dbo].[OrgMember] 
(
	 @oid INT
	,@grouptype VARCHAR(20)
	,@first VARCHAR(30)
	,@last VARCHAR(30) 
	,@sgfilter VARCHAR(300)
	,@showhidden BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT PeopleId
	FROM dbo.OrgPeople(@oid, @grouptype, @first, @last, @sgfilter, @showhidden, NULL, NULL, NULL, NULL, NULL, NULL)
)

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
