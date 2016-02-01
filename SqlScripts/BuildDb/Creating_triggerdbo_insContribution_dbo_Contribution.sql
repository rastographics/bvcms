CREATE TRIGGER [dbo].[insContribution] 
   ON  [dbo].[Contribution]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.Contribution
	SET CampusId = p.CampusId
	FROM dbo.Contribution c
	JOIN INSERTED i ON i.ContributionId = c.ContributionId
	JOIN dbo.People p ON p.PeopleId = i.PeopleId

END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
