CREATE FUNCTION [dbo].[WidowedDate] 
(
	@peopleid int
)
RETURNS DATETIME
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result DATETIME

	-- Add the T-SQL statements to compute the return value here
	SELECT TOP 1 @Result = s.DeceasedDate 
      FROM ( SELECT p1.FamilyId,
                    p1.MaritalStatusId,
                    p1.PositionInFamilyId
               FROM dbo.People p1
              WHERE p1.PeopleId =  @peopleid
                AND p1.MaritalStatusId = 50
                AND p1.DeceasedDate IS NULL
           ) p
     INNER JOIN dbo.Families f
             ON f.FamilyId = p.FamilyId
     INNER JOIN dbo.People s
             ON s.FamilyId = f.FamilyId
     WHERE s.PeopleId <> @peopleid 
       AND p.PositionInFamilyId = s.PositionInFamilyId
       AND s.DeceasedDate IS NOT NULL

	-- Return the result of the function
	RETURN @Result

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
