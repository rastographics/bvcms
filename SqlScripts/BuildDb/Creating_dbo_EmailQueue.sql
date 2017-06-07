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
[CClist] [nvarchar] (max) NULL,
[Testing] [bit] NULL,
[ReadyToSend] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
