CREATE FUNCTION [dbo].[SchoolGrade] (@pid INT)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @g INT

	SELECT TOP 1 @g = o.GradeAgeStart 
	FROM dbo.OrganizationMembers AS om 
		JOIN dbo.Organizations AS o ON om.OrganizationId = o.OrganizationId 
	WHERE o.IsBibleFellowshipOrg = 1
		AND om.PeopleId = @pid 
		AND om.MemberTypeId = 220 

	-- Return the result of the function
	RETURN @g

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
