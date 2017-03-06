CREATE TABLE [lookup].[VolunteerCodes]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (10) NULL,
[Description] [nvarchar] (50) NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
