CREATE TABLE [dbo].[OneTimeLinks]
(
[Id] [uniqueidentifier] NOT NULL,
[querystring] [nvarchar] (2000) NULL,
[used] [bit] NOT NULL CONSTRAINT [DF_OneTimeLinks_used] DEFAULT ((0)),
[expires] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
