ALTER TABLE OrgMemberDocuments ALTER COLUMN DocumentName nvarchar(max) NOT NULL;
ALTER TABLE OrgTemporaryDocuments ALTER COLUMN DocumentName nvarchar(max) NOT NULL;
ALTER TABLE OrgTemporaryDocuments ALTER COLUMN LastName nvarchar(max) NULL;
ALTER TABLE OrgTemporaryDocuments ALTER COLUMN FirstName nvarchar(max) NULL;
ALTER TABLE OrgTemporaryDocuments ALTER COLUMN EmailAddress nvarchar(max) NULL;
GO
