SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE VIEW [dbo].[OrgsWithoutFees]
AS
(
	SELECT OrganizationId 
	FROM (
		SELECT 
			OrganizationId, 
			CONVERT(MONEY, dbo.RegexMatch(ISNULL(RegSetting, ''), '(?<=^Fee:\s)(.*)$')) fee
		FROM dbo.Organizations
	) tt
	WHERE fee = 0
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
