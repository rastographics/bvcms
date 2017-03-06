CREATE TABLE [dbo].[RegistrationData]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Data] [xml] NULL,
[Stamp] [datetime] NULL,
[completed] [bit] NULL,
[OrganizationId] [int] NULL,
[UserPeopleId] [int] NULL,
[abandoned] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
