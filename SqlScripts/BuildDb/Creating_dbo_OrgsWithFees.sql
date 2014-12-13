CREATE VIEW [dbo].[OrgsWithFees]
AS
(
SELECT OrganizationId 
FROM (
	SELECT 
		OrganizationId, 
		CONVERT(MONEY, dbo.RegexMatch(ISNULL(RegSetting, ''), '(?<=^Fee:\s)(.*)$')) fee
	FROM dbo.Organizations
) tt
WHERE fee > 0
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
