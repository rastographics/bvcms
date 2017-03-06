CREATE TABLE [dbo].[RelatedFamilies]
(
[FamilyId] [int] NOT NULL,
[RelatedFamilyId] [int] NOT NULL,
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[FamilyRelationshipDesc] [nvarchar] (256) NOT NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
