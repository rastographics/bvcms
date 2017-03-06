CREATE TABLE [lookup].[OrganizationStatus]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (20) NULL,
[Description] [nvarchar] (50) NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
