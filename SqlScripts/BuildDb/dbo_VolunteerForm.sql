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
[Name] [nvarchar] (50) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
