CREATE TABLE [dbo].[EmailQueueToFail]
(
[Id] [int] NULL,
[PeopleId] [int] NULL,
[time] [datetime] NULL,
[event] [nvarchar] (20) NULL,
[reason] [nvarchar] (300) NULL,
[bouncetype] [nvarchar] (20) NULL,
[email] [nvarchar] (100) NULL,
[pkey] [int] NOT NULL IDENTITY(1, 1),
[timestamp] [bigint] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
