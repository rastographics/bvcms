CREATE TABLE [dbo].[Tag]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[TypeId] [int] NOT NULL CONSTRAINT [DF_Tag_TypeId] DEFAULT ((1)),
[Owner] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Active] [bit] NULL,
[PeopleId] [int] NULL,
[OwnerName] AS ([dbo].[UName]([PeopleId])),
[Created] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
