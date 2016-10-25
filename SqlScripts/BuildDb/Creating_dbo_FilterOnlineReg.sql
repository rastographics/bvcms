CREATE FUNCTION [dbo].[FilterOnlineReg](@onlinereg INT)
RETURNS 
@t TABLE ( oid INT )
AS
BEGIN

	DECLARE @picklist TABLE(oid INT)
	DECLARE @orgmaster TABLE(oid INT, isPickList BIT)
	DECLARE @activeReg TABLE(oid INT)
	DECLARE @appReg TABLE(oid INT)

    DECLARE @AnyReg INT = 99
    DECLARE @MissionTrip INT = 98
    DECLARE @MasterOrStandalone INT = 97
    DECLARE @Active INT = 96
    DECLARE @NotOnApp INT = 95
    DECLARE @OnApp INT = 94
    DECLARE @Standalone INT = 93
    DECLARE @ChildOfMaster INT = 92

	IF @onlinereg IN (
		@ChildOfMaster,
		@Standalone,
		@OnApp,
		@NotOnApp,
		@Active,
		@MasterOrStandalone
	)
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
		INSERT @appReg ( oid )
			SELECT o.OrganizationId
			FROM dbo.Organizations o
			JOIN @activeReg a ON a.oid = o.OrganizationId
				AND o.RegStart IS NOT NULL
				AND o.RegEND > GETDATE()
				AND o.OrganizationStatusId = 30
			WHERE ISNULL(o.AppCategory, '') <> 'Invitation Only'
	END
    

	; WITH OnApp AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @appReg a ON a.oid = o.OrganizationId
	),
	NotOnApp AS ( 
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @activeReg a ON a.oid = o.OrganizationId
		LEFT JOIN @appReg ap ON ap.oid = o.OrganizationId
		WHERE ap.oid IS NULL
	),
	Active AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @activeReg a ON a.oid = o.OrganizationId
	),
	MasterOrStandalone AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		JOIN @orgmaster m ON m.oid = o.OrganizationId
		WHERE m.IsPickList = 0
	),
	MissionTrip AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE ISNULL(o.RegistrationTypeId, 0) > 0 
		AND o.IsMissionTrip = 1
	),
	AnyReg AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE ISNULL(o.RegistrationTypeId, 0) > 0 
	),
	NotReg AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE ISNULL(o.RegistrationTypeId, 0) = 0
	),
	Filter AS (
		SELECT OrganizationId oid
		FROM dbo.Organizations o
		WHERE (ISNULL(@onlinereg, 0) = 0 
			AND o.OrganizationId IN (SELECT oid FROM NotReg)
		)
		OR (@onlinereg = @ChildOfMaster
			AND o.OrganizationId IN (SELECT oid FROM @picklist)
			AND ISNULL(o.RegistrationTypeId, 0) > 0
		) 
		OR (@onlinereg = @Standalone
			AND o.OrganizationId NOT IN (SELECT oid FROM @picklist)
			AND ISNULL(o.RegistrationTypeId, 0) = 1 
		) 
		OR (@onlinereg = @OnApp
			AND o.OrganizationId IN (SELECT oid FROM OnApp)
		) 
		OR (@onlinereg = @NotOnApp
			AND o.OrganizationId IN (SELECT oid FROM NotOnApp)
		) 
		OR (@onlinereg = @Active
			AND o.OrganizationId IN (SELECT oid FROM Active)
		) 
		OR (@onlinereg = @MasterOrStandalone
			AND o.OrganizationId IN (SELECT oid FROM MasterOrStandalone)
		) 
		OR (@onlinereg = @MissionTrip
			AND o.OrganizationId IN (SELECT oid FROM MissionTrip)
		) 
		OR (@onlinereg = @AnyReg
			AND o.OrganizationId IN (SELECT oid FROM AnyReg)
		) 
		OR (@onlinereg > 0 AND @onlinereg < 90
			AND o.RegistrationTypeId = @onlinereg
		) 
	) 
	INSERT @t SELECT oid FROM Filter
	RETURN 
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
