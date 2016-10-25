CREATE TABLE [dbo].[ContributionsRun]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[started] [datetime] NULL,
[count] [int] NULL,
[processed] [int] NULL,
[completed] [datetime] NULL,
[error] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LastSet] [int] NULL,
[CurrSet] [int] NULL,
[Sets] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[running] AS (case  when [completed] IS NULL AND [error] IS NULL AND datediff(minute,[started],getdate())<(120) then (1) else (0) end)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
