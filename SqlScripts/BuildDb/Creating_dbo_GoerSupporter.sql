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
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
