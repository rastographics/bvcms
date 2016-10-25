-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[DownlineSingleTrace]
(
	@categoryid INT,
	@leaderid INT,
	@trace VARCHAR(400)
)
RETURNS 
@ret TABLE
(
	Generation INT,
	LeaderId INT,
	LeaderName VARCHAR(100),
	DiscipleId INT,
	DiscipleName VARCHAR(100),
	OrgName VARCHAR(100),
	StartDt DATETIME,
	EndDt DATETIME,
	Trace VARCHAR(400)
)
AS
BEGIN
	--DECLARE @trace VARCHAR(400) = '828612/903260/806150/839437/839438/31583/815290/828728/816281/1024044'
	--DECLARE @leaderid INT = 828612
	--DECLARE @categoryid INT = 2

	DECLARE @ids TABLE (rownum INT IDENTITY (1, 1) PRIMARY KEY NOT NULL, id INT)
	INSERT @ids ( id )
	SELECT Value FROM dbo.SPLIT(@trace, '/')
	ORDER BY TokenID

	DECLARE @t TABLE (rownum INT, lid INT, did INT)
	INSERT @t ( rownum, lid, did )
	SELECT t1.rownum, t1.id, t2.id
	FROM @ids t1
	JOIN @ids t2 ON t1.rownum = t2.rownum - 1

	;WITH orgmembers AS (
		SELECT
			d.Generation,
			d.OrgId,
			d.LeaderId,
			d.DiscipleId,
			d.StartDt,
			d.EndDt,
			d.TRACE
		FROM dbo.Downline d
		JOIN @t t ON d.LeaderId = t.lid AND d.DiscipleId = t.did
		WHERE d.CategoryId = @categoryid
		AND d.DownlineId = @leaderid
	)
	INSERT @ret
	        ( Generation ,
	          LeaderId ,
	          LeaderName ,
	          DiscipleId ,
	          DiscipleName ,
	          OrgName ,
			  StartDt,
			  EndDt,
	          Trace
	        )
	SELECT
		om.Generation 
		,om.LeaderId
		,l.NAME
		,om.DiscipleId
		,d.NAME
		,o.OrganizationName
		,om.StartDt
		,EndDt = NULLIF(om.EndDt, '1/1/3000')
		,om.TRACE
	FROM  orgmembers om
	JOIN dbo.Organizations o ON o.OrganizationId = om.OrgId
	JOIN dbo.People l ON l.PeopleId = om.LeaderId
	JOIN dbo.People d ON d.PeopleId = om.DiscipleId
	ORDER BY om.Generation
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
