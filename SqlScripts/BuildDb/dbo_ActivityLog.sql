CREATE TABLE [dbo].[ActivityLog]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[ActivityDate] [datetime] NULL,
[UserId] [int] NULL,
[Activity] [nvarchar] (200) NULL,
[PageUrl] [nvarchar] (410) NULL,
[Machine] [nvarchar] (50) NULL,
[OrgId] [int] NULL,
[PeopleId] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
