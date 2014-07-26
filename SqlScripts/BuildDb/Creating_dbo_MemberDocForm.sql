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
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
