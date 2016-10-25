CREATE TABLE [dbo].[ManagedGiving]
(
[PeopleId] [int] NOT NULL,
[StartWhen] [datetime] NULL,
[NextDate] [datetime] NULL,
[SemiEvery] [nvarchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Day1] [int] NULL,
[Day2] [int] NULL,
[EveryN] [int] NULL,
[Period] [nvarchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StopWhen] [datetime] NULL,
[StopAfter] [int] NULL,
[type] [nvarchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
