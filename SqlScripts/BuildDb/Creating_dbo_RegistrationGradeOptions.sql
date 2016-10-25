CREATE FUNCTION [dbo].[RegistrationGradeOptions](@orgid INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH groups AS (
		SELECT o.OrganizationId 
			,x.value('(.)[1]', 'varchar(100)') GradeOption
			,x.value('(@Code)[1]', 'varchar(100)') GradeCode
		FROM    dbo.Organizations o
		CROSS APPLY o.RegSettingXml.nodes('/Settings/AskItems//GradeOption') tt ( x )
		WHERE @orgid IS NULL OR o.OrganizationId = @orgid
	), summary AS (
		SELECT groups.OrganizationId, groups.GradeOption, groups.GradeCode, COUNT(*) Cnt
		FROM groups
		GROUP BY groups.OrganizationId, groups.GradeOption, groups.GradeCode
	)
	SELECT 
		o.OrganizationId, 
		o.OrganizationName, 
		s.GradeOption, 
		s.GradeCode, 
		Cnt = (SELECT COUNT(*) FROM summary ss WHERE ss.GradeCode = s.GradeCode)
	FROM summary s
	JOIN dbo.Organizations o ON o.OrganizationId = s.OrganizationId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
