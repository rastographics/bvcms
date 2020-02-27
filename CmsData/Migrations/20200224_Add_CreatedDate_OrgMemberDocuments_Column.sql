IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'CreatedDate'
          AND Object_ID = Object_ID(N'dbo.OrgMemberDocuments'))
BEGIN
	ALTER TABLE dbo.OrgMemberDocuments
	ADD [CreatedDate] DATETIME
END
GO

