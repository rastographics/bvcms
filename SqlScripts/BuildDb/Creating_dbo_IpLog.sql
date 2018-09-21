CREATE TABLE [dbo].[IpLog]
(
[ip] [varchar] (50) NOT NULL,
[id] [varchar] (300) NOT NULL,
[tm] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
