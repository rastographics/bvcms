CREATE TABLE [dbo].[Downline]
(
[CategoryId] [int] NULL,
[DownlineId] [int] NULL,
[Generation] [int] NULL,
[OrgId] [int] NULL,
[LeaderId] [int] NULL,
[DiscipleId] [int] NULL,
[StartDt] [datetime] NULL,
[Trace] [varchar] (400) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[EndDt] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
