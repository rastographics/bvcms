CREATE VIEW [export].[XpSubGroup] AS 
SELECT 
	omm.OrgId ,
    PeopleId ,
    mt.[Name] 
FROM dbo.OrgMemMemTags omm
JOIN dbo.MemberTags mt ON mt.Id = omm.MemberTagId AND mt.OrgId = omm.OrgId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
