CREATE TABLE [dbo].[LongRunningOp]
(
[id] [int] NOT NULL,
[operation] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[started] [datetime] NULL,
[count] [int] NULL,
[processed] [int] NULL,
[completed] [datetime] NULL,
[ElapsedTime] AS (substring(CONVERT([varchar],[completed]-[started],(121)),(12),(20))),
[CustomMessage] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
