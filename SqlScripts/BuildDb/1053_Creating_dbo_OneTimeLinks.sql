CREATE TABLE [dbo].[OneTimeLinks]
(
[Id] [uniqueidentifier] NOT NULL,
[querystring] [nvarchar] (2000) NULL,
[used] [bit] NOT NULL CONSTRAINT [DF_OneTimeLinks_used] DEFAULT ((0)),
[expires] [datetime] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
