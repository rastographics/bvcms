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
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
