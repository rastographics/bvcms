CREATE TABLE [dbo].[EmailQueue]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[SendWhen] [datetime] NULL,
[Subject] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Body] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FromAddr] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Sent] [datetime] NULL,
[Started] [datetime] NULL,
[Queued] [datetime] NOT NULL,
[FromName] [nvarchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[QueuedBy] [int] NULL,
[Redacted] [bit] NULL,
[Transactional] [bit] NULL,
[Public] [bit] NULL,
[Error] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CCParents] [bit] NULL,
[NoReplacements] [bit] NULL,
[SendFromOrgId] [int] NULL,
[FinanceOnly] [bit] NULL,
[CClist] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
