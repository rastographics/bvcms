CREATE TABLE [dbo].[Tag]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (200) NOT NULL,
[TypeId] [int] NOT NULL CONSTRAINT [DF_Tag_TypeId] DEFAULT ((1)),
[Owner] [nvarchar] (50) NULL,
[Active] [bit] NULL,
[PeopleId] [int] NULL,
[OwnerName] AS ([dbo].[UName]([PeopleId])),
[Created] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
