CREATE PROCEDURE [dbo].[InsertQuestion](@oid INT, @pid INT, @question NVARCHAR(200), @answer NVARCHAR(1000))
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.OrganizationMembers
	SET OnlineRegData.modify('insert <ExtraQuestion question=''{sql:variable("@question")}''>{sql:variable("@answer")}</ExtraQuestion> 
		into (/OnlineRegPersonModel)[1]')
	WHERE OrganizationId = @oid AND PeopleId = @pid
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
