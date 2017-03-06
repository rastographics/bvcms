CREATE TABLE [dbo].[GoerSupporter]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[GoerId] [int] NOT NULL,
[SupporterId] [int] NULL,
[NoDbEmail] [varchar] (80) NULL,
[Active] [bit] NULL,
[Unsubscribe] [bit] NULL,
[Created] [datetime] NOT NULL,
[Salutation] [nvarchar] (80) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
