IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'OrgTemporaryDocuments' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN	
		CREATE TABLE [dbo].[OrgTemporaryDocuments](
			[TemporaryDocumentId] [INT] PRIMARY KEY IDENTITY (1,1),
			[DocumentName] [NVARCHAR](100) NOT NULL,
			[LastName] [NVARCHAR](100),
			[FirstName] [NVARCHAR](100),
			[EmailAddress] [NVARCHAR](100),
			[ImageId] [INT] NOT NULL UNIQUE,
			[OrganizationId] [INT] NOT NULL FOREIGN KEY REFERENCES Organizations(OrganizationId),
			[CreatedDate] [DATETIME]
			)
	END
GO
