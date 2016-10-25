CREATE FUNCTION [dbo].[RegistrationSmallGroups](@orgid int)
RETURNS TABLE 
AS
RETURN 
(
	WITH groups AS (
		SELECT o.OrganizationId, x.value('(.)[1]', 'varchar(100)') SmallGroup
		FROM    dbo.Organizations o
		CROSS APPLY o.RegSettingXml.nodes('/Settings/AskItems//SmallGroup') tt ( x )
		WHERE @orgid IS NULL OR o.OrganizationId = @orgid
	), summary AS (
		SELECT groups.OrganizationId, SmallGroup, COUNT(*) Cnt
		FROM groups
		WHERE groups.SmallGroup NOT IN ('nocheckbox', 'comment')
		GROUP BY groups.OrganizationId, groups.SmallGroup
	)
	SELECT o.OrganizationId, o.OrganizationName, s.SmallGroup, s.Cnt
	FROM summary s
	JOIN dbo.Organizations o ON o.OrganizationId = s.OrganizationId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
