CREATE FUNCTION [dbo].[FilterOnlineReg](@onlinereg INT)
RETURNS 
@t TABLE ( oid INT )
AS
BEGIN

	DECLARE @picklist TABLE(oid INT)
	DECLARE @orgmaster TABLE(oid INT, isPickList BIT)
	DECLARE @activeReg TABLE(oid INT)
	DECLARE @appRegistrations TABLE(oid INT)

	IF @onlinereg IN (94,95,96,97)
	BEGIN
		INSERT @picklist ( oid )
			SELECT PickListOrgId
			FROM Organizations o 
			CROSS APPLY ( SELECT value PickListOrgId 
				FROM dbo.SplitInts(o.OrgPickList) ) T 
			WHERE o.OrgPickList IS NOT NULL
		INSERT @orgmaster ( oid, isPickList )
			SELECT 
				o.OrganizationId, 
				CASE WHEN p.oid > 0 THEN 1 ELSE 0 END
			FROM Organizations o 
			LEFT JOIN @picklist p ON p.oid = o.OrganizationId
			WHERE o.RegistrationTypeId > 0
		INSERT @activeReg ( oid )
			SELECT o.OrganizationId 
			FROM dbo.Organizations o
			JOIN @orgmaster m ON m.oid = o.OrganizationId
			WHERE m.IsPickList = 0
			AND ISNULL(o.RegistrationClosed, 0) = 0
			AND ISNULL(o.ClassFilled, 0) = 0
			AND (o.RegStart IS NULL OR o.RegStart <= DATEADD(DAY, 21, GETDATE()))
			AND (o.RegEnd IS NULL OR o.RegEnd > GETDATE())
		INSERT @appRegistrations ( oid )
			SELECT o.OrganizationId
			FROM dbo.Organizations o
			JOIN @activeReg a ON a.oid = o.OrganizationId
				AND o.RegStart IS NOT NULL
				AND o.RegEND > GETDATE()
				AND o.OrganizationStatusId = 30
			WHERE ISNULL(o.AppCategory, '') <> 'Invitation Only'
	END
    

	; WITH anyOnlineRegOnApp94 AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @appRegistrations a ON a.oid = o.OrganizationId
	),
	anyOnlineRegNotOnApp95 AS ( 
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @activeReg a ON a.oid = o.OrganizationId
		LEFT JOIN @appRegistrations ap ON ap.oid = o.OrganizationId
		WHERE ap.oid IS NULL
	),
	anyOnlineRegActive96 AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @activeReg a ON a.oid = o.OrganizationId
	),
	anyOnlineRegNonPicklist97 AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @orgmaster m ON m.oid = o.OrganizationId
		WHERE m.IsPickList = 0
	),
	anyOnlineRegMissionTrip98 AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE ISNULL(o.RegistrationTypeId, 0) > 0 
		AND o.IsMissionTrip = 1
	),
	anyOnlineReg99 AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE ISNULL(o.RegistrationTypeId, 0) > 0 
	),
	noOnlineReg AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE ISNULL(o.RegistrationTypeId, 0) = 0
	),
	filterReg AS (
		SELECT OrganizationId oid
		FROM dbo.Organizations o
		WHERE (@onlinereg = 0 
			AND o.OrganizationId IN (SELECT oid FROM noOnlineReg)
		)
		OR (@onlinereg = 94
			AND o.OrganizationId IN (SELECT oid FROM anyOnlineRegOnApp94)
		) 
		OR (@onlinereg = 95
			AND o.OrganizationId IN (SELECT oid FROM anyOnlineRegNotOnApp95)
		) 
		OR (@onlinereg = 96
			AND o.OrganizationId IN (SELECT oid FROM anyOnlineRegActive96)
		) 
		OR (@onlinereg = 97
			AND o.OrganizationId IN (SELECT oid FROM anyOnlineRegNonPicklist97)
		) 
		OR (@onlinereg = 98
			AND o.OrganizationId IN (SELECT oid FROM anyOnlineRegMissionTrip98)
		) 
		OR (@onlinereg = 99
			AND o.OrganizationId IN (SELECT oid FROM anyOnlineReg99)
		) 
		OR (@onlinereg > 0 AND @onlinereg < 90
			AND o.RegistrationTypeId = @onlinereg
		) 
	) 
	INSERT @t SELECT oid FROM filterReg
	RETURN 
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
