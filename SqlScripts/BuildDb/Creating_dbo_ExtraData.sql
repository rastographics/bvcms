CREATE TABLE [dbo].[ExtraData]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Data] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Stamp] [datetime] NULL,
[completed] [bit] NULL,
[OrganizationId] [int] NULL,
[UserPeopleId] [int] NULL,
[abandoned] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
