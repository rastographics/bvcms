CREATE TABLE [dbo].[ZipCodes]
(
[zip] [nvarchar] (10) NOT NULL,
[state] [char] (2) NULL,
[City] [nvarchar] (50) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
