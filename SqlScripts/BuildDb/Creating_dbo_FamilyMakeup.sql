CREATE FUNCTION [dbo].[FamilyMakeup](@fid INT)
RETURNS VARCHAR(30)
AS
BEGIN
	DECLARE @ret VARCHAR(30)
	-- PositionInFamily = 10 = primary adult
	-- MaritalStatus = 0 = unknown
	-- MaritalStatus = 10 = single
	-- MaritalStatus = 20 = married

	SELECT @ret =
		CASE 
			WHEN (SELECT COUNT(*) FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10
			) = 2 THEN '2primary'
			WHEN (SELECT COUNT(*) FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10
			) = 1 AND EXISTS(
				SELECT NULL FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10 
				AND MaritalStatusId = 20
			) THEN '1married'
			WHEN (SELECT COUNT(*) FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10
			) = 1 AND EXISTS(
				SELECT NULL FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10 
				AND MaritalStatusId > 20
			) THEN '1married formerly'
			WHEN (SELECT COUNT(*) FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10
			) = 1 AND EXISTS(
				SELECT NULL FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10 
				AND MaritalStatusId = 0
			) THEN '1married unknown'
			WHEN (SELECT COUNT(*) FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10
			) = 1 AND EXISTS(
				SELECT NULL FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10 
				AND MaritalStatusId = 10
			) THEN '1single'
			WHEN (SELECT COUNT(*) FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10
			) = 0 THEN '0primary'
			WHEN (SELECT COUNT(*) FROM dbo.People 
				WHERE FamilyId = f.FamilyId 
				AND PositionInFamilyId = 10
			) > 2 THEN '2primary+'
		END
	FROM dbo.Families f
	WHERE f.FamilyId = @fid

	RETURN ISNULL(@ret,'0primary')
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
