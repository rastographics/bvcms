CREATE TABLE [dbo].[Downline]
(
[CategoryId] [int] NULL,
[DownlineId] [int] NULL,
[Generation] [int] NULL,
[OrgId] [int] NULL,
[LeaderId] [int] NULL,
[DiscipleId] [int] NULL,
[StartDt] [datetime] NULL,
[Trace] [varchar] (400) NULL,
[EndDt] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
