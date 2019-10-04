USE [CMS_parksidechurch]
GO

/****** Object:  StoredProcedure [dbo].[TopGivers]    Script Date: 02/10/2019 04:16:57 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[TopGivers](@top INT, @sdate DATETIME, @edate DATETIME)
AS
BEGIN

	--SELECT TOP (@top) c.PeopleId, Name, SUM(ContributionAmount) FROM dbo.People p
	--JOIN dbo.Contribution c ON p.PeopleId = c.PeopleId
	--WHERE c.ContributionDate >= @sdate
	--AND c.ContributionDate <= @edate
	--AND c.ContributionTypeId NOT IN (6,7,8)
	--GROUP BY c.PeopleId, Name
	--ORDER BY SUM(ContributionAmount) DESC

	SELECT TOP (@top)
	c.CreditGiverId PeopleId, HeadName Name, SUM(Amount) Amount
    FROM dbo.Contributions2(@sdate, @edate, 0, 0, NULL, 1) c 
    GROUP BY c.CreditGiverId, c.HeadName
    ORDER BY SUM(c.Amount) DESC

END
GO

EXEC sys.sp_addextendedproperty @name=N'ReturnType', @value=N'TopGiver' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'TopGivers'
GO


EXEC [dbo].[TopGivers] 1, '01-01-2019', '01-12-2019'