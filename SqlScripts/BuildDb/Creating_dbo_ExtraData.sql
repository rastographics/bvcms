CREATE TABLE [dbo].[ExtraData]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Data] [nvarchar] (max) NULL,
[Stamp] [datetime] NULL,
[completed] [bit] NULL,
[OrganizationId] [int] NULL,
[UserPeopleId] [int] NULL,
[abandoned] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
