CREATE TABLE [dbo].[Content]
(
[Name] [nvarchar] (400) NOT NULL,
[Title] [nvarchar] (500) NULL,
[Body] [nvarchar] (max) NULL,
[DateCreated] [datetime] NULL,
[Id] [int] NOT NULL IDENTITY(1, 1),
[TextOnly] [bit] NULL,
[TypeID] [int] NOT NULL CONSTRAINT [DF_Content_Type] DEFAULT ((0)),
[ThumbID] [int] NOT NULL CONSTRAINT [DF_Content_ThumbID] DEFAULT ((0)),
[RoleID] [int] NOT NULL CONSTRAINT [DF_Content_RoleID] DEFAULT ((0)),
[OwnerID] [int] NOT NULL CONSTRAINT [DF_Content_OwnerID] DEFAULT ((0)),
[CreatedBy] [nvarchar] (50) NULL,
[Archived] [datetime] NULL,
[ArchivedFromId] [int] NULL,
[UseTimes] [int] NULL,
[Snippet] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
