CREATE TABLE [dbo].[MemberTags]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (200) NULL,
[OrgId] [int] NULL,
[VolFrequency] [nvarchar] (2) NULL,
[VolStartDate] [datetime] NULL,
[VolEndDate] [datetime] NULL,
[NoCancelWeeks] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
