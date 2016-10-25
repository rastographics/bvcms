CREATE TABLE [dbo].[RelatedFamilies]
(
[FamilyId] [int] NOT NULL,
[RelatedFamilyId] [int] NOT NULL,
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[FamilyRelationshipDesc] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
