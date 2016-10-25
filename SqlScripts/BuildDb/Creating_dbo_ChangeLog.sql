CREATE TABLE [dbo].[ChangeLog]
(
[PeopleId] [int] NOT NULL,
[FamilyId] [int] NULL,
[UserPeopleId] [int] NOT NULL,
[Created] [datetime] NOT NULL,
[Field] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Data] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Id] [int] NOT NULL IDENTITY(1, 1),
[Before] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[After] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
