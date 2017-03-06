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
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
