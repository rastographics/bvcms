CREATE TABLE [dbo].[EmailQueueToFail]
(
[Id] [int] NULL,
[PeopleId] [int] NULL,
[time] [datetime] NULL,
[event] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[reason] [nvarchar] (300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[bouncetype] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[email] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[pkey] [int] NOT NULL IDENTITY(1, 1),
[timestamp] [bigint] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
