CREATE TABLE [dbo].[Task]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[OwnerId] [int] NOT NULL,
[ListId] [int] NOT NULL,
[CoOwnerId] [int] NULL,
[CoListId] [int] NULL,
[StatusId] [int] NULL,
[CreatedOn] [datetime] NOT NULL,
[SourceContactId] [int] NULL,
[CompletedContactId] [int] NULL,
[Notes] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModifiedBy] [int] NULL,
[ModifiedOn] [datetime] NULL,
[Project] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Archive] [bit] NOT NULL CONSTRAINT [DF_Task_Archive] DEFAULT ((0)),
[Priority] [int] NULL,
[WhoId] [int] NULL,
[Due] [datetime] NULL,
[Location] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Description] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CompletedOn] [datetime] NULL,
[ForceCompleteWContact] [bit] NULL,
[OrginatorId] [int] NULL,
[DeclineReason] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
