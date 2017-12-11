CREATE VIEW [export].[RelatedFamily] AS
SELECT
	FamilyId ,
    RelatedFamilyId ,
    FamilyRelationshipDesc 
FROM dbo.RelatedFamilies
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
