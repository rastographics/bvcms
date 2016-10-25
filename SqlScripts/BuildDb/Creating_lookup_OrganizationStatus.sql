CREATE TABLE [lookup].[OrganizationStatus]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Description] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
