CREATE TABLE [dbo].[Content]
(
[Name] [nvarchar] (400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Title] [nvarchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Body] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DateCreated] [datetime] NULL,
[Id] [int] NOT NULL IDENTITY(1, 1),
[TextOnly] [bit] NULL,
[TypeID] [int] NOT NULL CONSTRAINT [DF_Content_Type] DEFAULT ((0)),
[ThumbID] [int] NOT NULL CONSTRAINT [DF_Content_ThumbID] DEFAULT ((0)),
[RoleID] [int] NOT NULL CONSTRAINT [DF_Content_RoleID] DEFAULT ((0)),
[OwnerID] [int] NOT NULL CONSTRAINT [DF_Content_OwnerID] DEFAULT ((0)),
[CreatedBy] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Archived] [datetime] NULL,
[ArchivedFromId] [int] NULL,
[UseTimes] [int] NULL,
[Snippet] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
