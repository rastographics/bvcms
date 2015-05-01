CREATE TABLE [dbo].[LongRunningOp]
(
[id] [int] NOT NULL,
[operation] [nvarchar] (25) NOT NULL,
[started] [datetime] NULL,
[count] [int] NULL,
[processed] [int] NULL,
[completed] [datetime] NULL,
[ElapsedTime] AS (substring(CONVERT([varchar],[completed]-[started],(121)),(12),(20))),
[CustomMessage] [nvarchar] (80) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
