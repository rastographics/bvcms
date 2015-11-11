CREATE FUNCTION [dbo].[OrgMemberQuestions](
	 @oid INT
	 ,@pid INT
) RETURNS TABLE
AS
RETURN
(
	SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) [row], [type], [set], Question, Answer
	FROM dbo.OnlineRegQA
	WHERE PeopleId = @pid
	AND OrganizationId = @oid
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
