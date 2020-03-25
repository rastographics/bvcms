UPDATE d
SET CreatedDate = ISNULL(m.CreatedDate, GETDATE())
FROM dbo.OrgMemberDocuments d
LEFT JOIN dbo.OrganizationMembers m ON m.OrganizationId = d.OrganizationId AND m.PeopleId = d.PeopleId
WHERE d.CreatedDate IS NULL
GO


IF NOT EXISTS (SELECT OBJECT_ID('[DF_OrgMemberDocuments_CreatedDate]'))
ALTER TABLE [dbo].[OrgMemberDocuments]
ADD CONSTRAINT [DF_OrgMemberDocuments_CreatedDate] DEFAULT (GETDATE()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[OrgMemberDocuments] 
ALTER COLUMN [CreatedDate] DATETIME NOT NULL 
GO