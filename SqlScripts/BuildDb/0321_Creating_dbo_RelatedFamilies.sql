CREATE TABLE [dbo].[RelatedFamilies]
(
[FamilyId] [int] NOT NULL,
[RelatedFamilyId] [int] NOT NULL,
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[FamilyRelationshipDesc] [nvarchar] (256) NOT NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
