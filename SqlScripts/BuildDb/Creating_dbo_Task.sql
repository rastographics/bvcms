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
[Notes] [nvarchar] (max) NULL,
[ModifiedBy] [int] NULL,
[ModifiedOn] [datetime] NULL,
[Project] [nvarchar] (50) NULL,
[Archive] [bit] NOT NULL CONSTRAINT [DF_Task_Archive] DEFAULT ((0)),
[Priority] [int] NULL,
[WhoId] [int] NULL,
[Due] [datetime] NULL,
[Location] [nvarchar] (50) NULL,
[Description] [nvarchar] (100) NULL,
[CompletedOn] [datetime] NULL,
[ForceCompleteWContact] [bit] NULL,
[OrginatorId] [int] NULL,
[DeclineReason] [nvarchar] (max) NULL,
[LimitToRole] [nvarchar] (50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
