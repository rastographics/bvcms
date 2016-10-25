CREATE PROCEDURE [dbo].[UpdateQuestion](@oid INT, @pid INT, @n INT, @answer VARCHAR(1000))
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @question VARCHAR(MAX)
	SELECT @question = Question
	FROM dbo.OrgMemberQuestions(@oid, @pid)
	WHERE [row] = @n

	UPDATE dbo.OrganizationMembers
	SET OnlineRegData.modify('replace value of (//*[@question=sql:variable("@question")]/text())[1] with sql:variable("@answer")')
	WHERE OrganizationId = @oid AND PeopleId = @pid
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
