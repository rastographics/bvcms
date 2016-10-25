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
IF @@ERROR <> 0 SET NOEXEC ON
GO
