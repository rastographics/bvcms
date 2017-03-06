CREATE TABLE [dbo].[ManagedGiving]
(
[PeopleId] [int] NOT NULL,
[StartWhen] [datetime] NULL,
[NextDate] [datetime] NULL,
[SemiEvery] [nvarchar] (2) NULL,
[Day1] [int] NULL,
[Day2] [int] NULL,
[EveryN] [int] NULL,
[Period] [nvarchar] (2) NULL,
[StopWhen] [datetime] NULL,
[StopAfter] [int] NULL,
[type] [nvarchar] (2) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
