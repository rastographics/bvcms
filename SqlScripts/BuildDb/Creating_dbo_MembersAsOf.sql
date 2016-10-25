-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[MembersAsOf]
(	
	@from DATETIME,
	@to DATETIME,
	@progid INT,
	@divid INT,
	@orgid INT
)
RETURNS @t TABLE ( PeopleId INT )
AS
BEGIN
	INSERT INTO @t (PeopleId) SELECT p.PeopleId FROM dbo.People p
	WHERE
	EXISTS (
		SELECT NULL FROM dbo.EnrollmentTransaction et
		WHERE et.PeopleId = p.PeopleId
		AND et.TransactionTypeId <= 3
		AND @from <= COALESCE(et.NextTranChangeDate, GETDATE())
		AND et.TransactionDate <= @to
		AND (et.OrganizationId = @orgid OR @orgid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d1
				WHERE d1.OrgId = et.OrganizationId
				AND d1.DivId = @divid) OR @divid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d2
				WHERE d2.OrgId = et.OrganizationId
				AND EXISTS(SELECT NULL FROM Division d
						WHERE d2.DivId = d.Id
						AND d.ProgId = @progid)) OR @progid = 0)
		)
	RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
