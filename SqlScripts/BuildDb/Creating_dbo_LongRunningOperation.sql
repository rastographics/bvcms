CREATE TABLE [dbo].[LongRunningOperation]
(
[QueryId] [uniqueidentifier] NOT NULL,
[operation] [nvarchar] (25) NULL,
[started] [datetime] NULL,
[count] [int] NULL,
[processed] [int] NULL,
[completed] [datetime] NULL,
[ElapsedTime] AS (substring(CONVERT([varchar],[completed]-[started],(121)),(12),(20))),
[CustomMessage] [nvarchar] (200) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
