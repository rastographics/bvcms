CREATE TABLE [dbo].[IpWarmup]
(
[epoch] [datetime] NULL,
[sentsince] [int] NULL,
[since] [datetime] NULL,
[totalsent] [int] NULL,
[totaltries] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
