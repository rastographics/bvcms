CREATE TABLE [dbo].[MemberDocForm]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NOT NULL,
[DocDate] [datetime] NULL,
[UploaderId] [int] NULL,
[IsDocument] [bit] NULL,
[Purpose] [nvarchar] (30) NULL,
[LargeId] [int] NULL,
[MediumId] [int] NULL,
[SmallId] [int] NULL,
[Name] [nvarchar] (100) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
