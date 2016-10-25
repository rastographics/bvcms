-- =============================================
-- Author:		Kenny
-- Create date: 4/17/2008
-- Description:	Finds the Couple Flag given a family ID
-- This flag is used for figuring out how mailings should
-- be addressed (see codes below)
-- =============================================

CREATE FUNCTION [dbo].[CoupleFlag] 
(
	@family_id int
)

-- Returns:
--  0 - Individual
--  1 - Couple
--  2 - Couple + Family
--  3 - Single Parent + Family
--  4 - No Primary Family

RETURNS int
AS
BEGIN
	DECLARE @Result int

    SELECT top 1 @Result = 
        case (sum(case PositionInFamilyId when 10 then 1 else 0 end))
            when 2 then (case count(*) when 2 then (case min(MaritalStatusId) when 30 then (CASE MAX(MaritalStatusId) WHEN 30 THEN 1 ELSE 4 end) else 4 end) else 2 end)
            when 1 then (case count(*) when 1 then 0 else 3 end)
            else (case count(*) when 1 then 0 else 4 end)
        end
      FROM dbo.People
     WHERE FamilyId = @family_id
       AND DeceasedDate IS NULL
       AND FirstName <> 'Duplicate'

	-- Return the result of the function
	RETURN @Result

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
