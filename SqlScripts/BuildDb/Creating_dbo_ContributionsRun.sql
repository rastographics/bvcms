CREATE TABLE [dbo].[ContributionsRun]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[started] [datetime] NULL,
[count] [int] NULL,
[processed] [int] NULL,
[completed] [datetime] NULL,
[error] [nvarchar] (200) NULL,
[LastSet] [int] NULL,
[CurrSet] [int] NULL,
[Sets] [nvarchar] (50) NULL,
[running] AS (case  when [completed] IS NULL AND [error] IS NULL AND datediff(minute,[started],getdate())<(120) then (1) else (0) end)
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
