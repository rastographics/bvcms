-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[Contributions0]
(
	@fd DATETIME, 
	@td DATETIME,
	@fundid INT,
	@campusid INT,
	@pledges BIT,
	@nontaxded BIT,
	@includeUnclosed BIT
)
RETURNS 
@t TABLE ( PeopleId INT )
AS
BEGIN
	DECLARE @cc TABLE (PeopleId INT)
	INSERT INTO @cc
	SELECT CreditGiverId 
	FROM Contributions2(@fd, @td, @campusid, @pledges, @nontaxded, @includeUnclosed)
	UNION
	SELECT CreditGiverId2 
	FROM Contributions2(@fd, @td, @campusid, @pledges, @nontaxded, @includeUnclosed)
	WHERE CreditGiverId2 IS NOT NULL

	INSERT @t ( PeopleId )
	SELECT p.PeopleId 
	FROM dbo.People p
	LEFT JOIN @cc c ON c.PeopleId = p.PeopleId
	WHERE c.PeopleId IS NULL
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
