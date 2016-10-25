CREATE TABLE [dbo].[GoerSupporter]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[GoerId] [int] NOT NULL,
[SupporterId] [int] NULL,
[NoDbEmail] [varchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Active] [bit] NULL,
[Unsubscribe] [bit] NULL,
[Created] [datetime] NOT NULL,
[Salutation] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
