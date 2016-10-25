CREATE FUNCTION [dbo].[OneHeadOfHouseholdIsMember](@fid INT)
RETURNS BIT
AS
	BEGIN
	IF EXISTS(SELECT NULL FROM dbo.People WHERE FamilyId = @fid AND PeopleId IN (dbo.HeadOfHouseholdId(@fid), dbo.HeadOfHouseHoldSpouseId(@fid)) AND MemberStatusId = 10)
		RETURN 1
	RETURN 0
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
