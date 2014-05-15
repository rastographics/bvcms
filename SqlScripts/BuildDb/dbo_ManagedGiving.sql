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
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
