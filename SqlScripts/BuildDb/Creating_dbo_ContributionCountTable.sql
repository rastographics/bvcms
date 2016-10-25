CREATE FUNCTION [dbo].[ContributionCountTable](@days INT, @cnt INT, @fundid INT, @op nvarchar(2))
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
IF @op = '>'
	INSERT INTO @t SELECT p.PeopleId FROM dbo.People p
	WHERE ( SELECT COUNT(*) FROM dbo.Contribution c
		WHERE ContributionDate >= DATEADD(D, -@days, GETDATE())
			AND (c.FundId = @fundid OR @fundid IS NULL)
			AND ContributionStatusId = 0 --Recorded
			AND ContributionTypeId NOT IN (6,7,8) --Reversed or returned
			AND c.PeopleId IN (p.PeopleId, p.SpouseId)
			AND ((ISNULL(ContributionOptionsId,1) <>  2 AND c.PeopleId = p.PeopleId)
				 OR (ISNULL(ContributionOptionsId,1) = 2 AND c.PeopleId IN (p.PeopleId, p.SpouseId)))
			) > @cnt	
ELSE IF @op = '>='
	INSERT INTO @t SELECT p.PeopleId FROM dbo.People p
	WHERE ( SELECT COUNT(*) FROM dbo.Contribution c
		WHERE ContributionDate >= DATEADD(D, -@days, GETDATE())
			AND (c.FundId = @fundid OR @fundid IS NULL)
			AND ContributionStatusId = 0 --Recorded
			AND ContributionTypeId NOT IN (6,7,8) --Reversed or returned
			AND c.PeopleId IN (p.PeopleId, p.SpouseId)
			AND ((ISNULL(ContributionOptionsId,1) <>  2 AND c.PeopleId = p.PeopleId)
				 OR (ISNULL(ContributionOptionsId,1) = 2 AND c.PeopleId IN (p.PeopleId, p.SpouseId)))
			) >= @cnt	
ELSE IF @op = '<'
	INSERT INTO @t SELECT p.PeopleId FROM dbo.People p
	WHERE ( SELECT COUNT(*) FROM dbo.Contribution c
		WHERE ContributionDate >= DATEADD(D, -@days, GETDATE())
			AND (c.FundId = @fundid OR @fundid IS NULL)
			AND ContributionStatusId = 0 --Recorded
			AND ContributionTypeId NOT IN (6,7,8) --Reversed or returned
			AND c.PeopleId IN (p.PeopleId, p.SpouseId)
			AND ((ISNULL(ContributionOptionsId,1) <>  2 AND c.PeopleId = p.PeopleId)
				 OR (ISNULL(ContributionOptionsId,1) = 2 AND c.PeopleId IN (p.PeopleId, p.SpouseId)))
			) < @cnt	
ELSE IF @op = '<='
	INSERT INTO @t SELECT p.PeopleId FROM dbo.People p
	WHERE ( SELECT COUNT(*) FROM dbo.Contribution c
		WHERE ContributionDate >= DATEADD(D, -@days, GETDATE())
			AND (c.FundId = @fundid OR @fundid IS NULL)
			AND ContributionStatusId = 0 --Recorded
			AND ContributionTypeId NOT IN (6,7,8) --Reversed or returned
			AND c.PeopleId IN (p.PeopleId, p.SpouseId)
			AND ((ISNULL(ContributionOptionsId,1) <>  2 AND c.PeopleId = p.PeopleId)
				 OR (ISNULL(ContributionOptionsId,1) = 2 AND c.PeopleId IN (p.PeopleId, p.SpouseId)))
			) <= @cnt	
ELSE IF @op = '='
	INSERT INTO @t SELECT p.PeopleId FROM dbo.People p
	WHERE ( SELECT COUNT(*) FROM dbo.Contribution c
		WHERE ContributionDate >= DATEADD(D, -@days, GETDATE())
			AND (c.FundId = @fundid OR @fundid IS NULL)
			AND ContributionStatusId = 0 --Recorded
			AND ContributionTypeId NOT IN (6,7,8) --Reversed or returned
			AND c.PeopleId IN (p.PeopleId, p.SpouseId)
			AND ((ISNULL(ContributionOptionsId,1) <>  2 AND c.PeopleId = p.PeopleId)
				 OR (ISNULL(ContributionOptionsId,1) = 2 AND c.PeopleId IN (p.PeopleId, p.SpouseId)))
			) = @cnt	
ELSE IF @op = '<>'
	INSERT INTO @t SELECT p.PeopleId FROM dbo.People p
	WHERE ( SELECT COUNT(*) FROM dbo.Contribution c
		WHERE ContributionDate >= DATEADD(D, -@days, GETDATE())
			AND (c.FundId = @fundid OR @fundid IS NULL)
			AND ContributionStatusId = 0 --Recorded
			AND ContributionTypeId NOT IN (6,7,8) --Reversed or returned
			AND c.PeopleId IN (p.PeopleId, p.SpouseId)
			AND ((ISNULL(ContributionOptionsId,1) <>  2 AND c.PeopleId = p.PeopleId)
				 OR (ISNULL(ContributionOptionsId,1) = 2 AND c.PeopleId IN (p.PeopleId, p.SpouseId)))
			) <> @cnt	

    RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
