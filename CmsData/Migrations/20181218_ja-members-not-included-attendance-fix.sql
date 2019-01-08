IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[RepairEnrollmentDate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE [dbo].[RepairEnrollmentDate]
END
GO

CREATE PROC [dbo].[RepairEnrollmentDate](@orgID INT)
AS
BEGIN
    UPDATE om
    SET	om.EnrollmentDate = DATEADD(SECOND,1,om.EnrollmentDate)
    --SELECT OM.ORGANIZATIONID, OM.PEOPLEID, OM.ENROLLMENTDATE, ET.ENROLLMENTDATE 
    FROM
        dbo.OrganizationMembers AS om
        left outer join EnrollmentTransaction et ON om.OrganizationId = et.OrganizationId and om.PeopleId = et.PeopleId 
   WHERE om.Organizationid = @orgID AND et.EnrollmentDate is null
    
END
GO

ALTER PROC [dbo].[RepairEnrollmentTransactions](@oid INT)
AS 
BEGIN
    DECLARE @t TABLE (id INT, typ INT, dt DATETIME, pid INT)
    INSERT @t (id, typ, dt, pid)
    SELECT TransactionId, TransactionTypeId, TransactionDate, PeopleId FROM dbo.EnrollmentTransaction WHERE OrganizationId = @oid

    UPDATE dbo.EnrollmentTransaction
    SET NextTranChangeDate = (SELECT TOP 1 dt FROM @t WHERE e.TransactionTypeId <= 3 AND pid = e.PeopleId AND dt > e.TransactionDate ORDER BY dt) 
      , EnrollmentTransactionId = (SELECT TOP 1 id FROM @t WHERE typ <= 2 AND pid = e.PeopleId AND dt < e.TransactionDate AND e.TransactionTypeId >= 3 ORDER BY dt DESC) 
    FROM dbo.EnrollmentTransaction e
    WHERE OrganizationId = @oid AND e.TransactionId = TransactionId

    exec [RepairEnrollmentDate] @orgid = @oid

    UPDATE dbo.Organizations
    SET MemberCount = dbo.OrganizationMemberCount(OrganizationId),
        ProspectCount = dbo.OrganizationProspectCount(OrganizationId)
    WHERE OrganizationId = @oid
END
GO
