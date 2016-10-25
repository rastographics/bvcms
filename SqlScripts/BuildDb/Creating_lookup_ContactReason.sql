CREATE TABLE [lookup].[ContactReason]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Description] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
