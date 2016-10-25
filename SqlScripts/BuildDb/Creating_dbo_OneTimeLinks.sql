CREATE TABLE [dbo].[OneTimeLinks]
(
[Id] [uniqueidentifier] NOT NULL,
[querystring] [nvarchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[used] [bit] NOT NULL CONSTRAINT [DF_OneTimeLinks_used] DEFAULT ((0)),
[expires] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
