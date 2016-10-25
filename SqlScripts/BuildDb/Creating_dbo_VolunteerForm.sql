CREATE TABLE [dbo].[VolunteerForm]
(
[PeopleId] [int] NOT NULL,
[AppDate] [datetime] NULL,
[LargeId] [int] NULL,
[MediumId] [int] NULL,
[SmallId] [int] NULL,
[Id] [int] NOT NULL IDENTITY(1, 1),
[UploaderId] [int] NULL,
[IsDocument] [bit] NULL,
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
