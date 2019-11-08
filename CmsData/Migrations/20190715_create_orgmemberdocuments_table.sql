IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'OrgMemberDocuments' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN				
		CREATE TABLE [dbo].[OrgMemberDocuments](
			[DocumentId][int] IDENTITY(1,1) PRIMARY KEY,
			[DocumentName]NVARCHAR(100) NOT NULL,
			[ImageId][int] NOT NULL UNIQUE,
			[PeopleId][int] FOREIGN KEY REFERENCES [dbo].[People](PeopleId) NOT NULL,
			[OrganizationId][int] FOREIGN KEY REFERENCES [dbo].[Organizations](OrganizationId) NOT NULL
		);
	END
GO
