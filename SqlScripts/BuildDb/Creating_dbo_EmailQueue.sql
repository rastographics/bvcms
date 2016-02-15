CREATE TABLE [dbo].[EmailQueue]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[SendWhen] [datetime] NULL,
[Subject] [nvarchar] (200) NULL,
[Body] [nvarchar] (max) NULL,
[FromAddr] [nvarchar] (100) NULL,
[Sent] [datetime] NULL,
[Started] [datetime] NULL,
[Queued] [datetime] NOT NULL,
[FromName] [nvarchar] (60) NULL,
[QueuedBy] [int] NULL,
[Redacted] [bit] NULL,
[Transactional] [bit] NULL,
[Public] [bit] NULL,
[Error] [nvarchar] (200) NULL,
[CCParents] [bit] NULL,
[NoReplacements] [bit] NULL,
[SendFromOrgId] [int] NULL,
[FinanceOnly] [bit] NULL,
[CClist] [nvarchar] (max) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
