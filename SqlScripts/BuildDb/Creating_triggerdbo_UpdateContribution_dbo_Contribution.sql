-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[UpdateContribution] 
   ON  [dbo].[Contribution]
   AFTER INSERT,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF(UPDATE(FundId))
	BEGIN
		UPDATE dbo.Contribution
		SET ContributionTypeId = 9
		FROM dbo.Contribution c
		JOIN INSERTED i ON c.ContributionId = i.ContributionId
		JOIN dbo.ContributionFund f ON i.FundId = f.FundId
		WHERE f.NonTaxDeductible = 1 AND i.ContributionTypeId NOT IN (6,7)
	END

	IF(UPDATE(PeopleId))
	BEGIN
		UPDATE dbo.Contribution
		SET CampusId = p.CampusId
		FROM dbo.Contribution c
		JOIN INSERTED i ON i.ContributionId = c.ContributionId
		JOIN dbo.People p ON p.PeopleId = i.PeopleId
    END

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
