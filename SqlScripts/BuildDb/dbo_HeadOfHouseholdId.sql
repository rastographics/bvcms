
CREATE FUNCTION [dbo].[HeadOfHouseholdId] ( @family_id INT )
RETURNS INT
AS 
    BEGIN
        DECLARE @Result INT

        SELECT TOP 1 @Result = PeopleId FROM dbo.People
		WHERE FamilyId = @family_id
		ORDER BY 
			DeceasedDate,
			PositionInFamilyId, 
			CASE PositionInFamilyId WHEN 10 THEN GenderId ELSE 0 END, 
			Age DESC
		
        RETURN @Result

    END



GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
