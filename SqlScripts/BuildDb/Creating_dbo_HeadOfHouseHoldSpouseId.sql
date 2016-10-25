
CREATE FUNCTION [dbo].[HeadOfHouseHoldSpouseId] 
(
	@family_id int
)

RETURNS int
AS
BEGIN
	DECLARE @Result int

    SELECT @Result = 
           dbo.SpouseId(dbo.HeadOfHouseholdId(@family_id))

	-- Return the result of the function
	RETURN @Result

END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
