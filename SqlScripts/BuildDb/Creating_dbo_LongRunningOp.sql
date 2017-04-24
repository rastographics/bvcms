CREATE TABLE [dbo].[LongRunningOp]
(
[id] [int] NOT NULL,
[operation] [nvarchar] (25) NOT NULL,
[started] [datetime] NULL,
[count] [int] NULL,
[processed] [int] NULL,
[completed] [datetime] NULL,
[ElapsedTime] AS (substring(CONVERT([varchar],[completed]-[started],(121)),(12),(20))),
[CustomMessage] [nvarchar] (80) NULL,
[QueryId] [uniqueidentifier] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
