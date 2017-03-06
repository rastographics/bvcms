CREATE TABLE [dbo].[ChangeLog]
(
[PeopleId] [int] NOT NULL,
[FamilyId] [int] NULL,
[UserPeopleId] [int] NOT NULL,
[Created] [datetime] NOT NULL,
[Field] [nvarchar] (50) NULL,
[Data] [nvarchar] (max) NULL,
[Id] [int] NOT NULL IDENTITY(1, 1),
[Before] [nvarchar] (max) NULL,
[After] [nvarchar] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
