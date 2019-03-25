IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[RepairEnrollmentDate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE [dbo].[RepairEnrollmentDate]
END
GO

CREATE PROC [dbo].[RepairEnrollmentDate](@orgID INT)
AS
BEGIN
    UPDATE om
    SET	om.EnrollmentDate = DATEADD(SECOND,1,isnull(om.EnrollmentDate, om.createddate))
    --SELECT OM.ORGANIZATIONID, OM.PEOPLEID, OM.ENROLLMENTDATE, ET.ENROLLMENTDATE 
    FROM
        dbo.OrganizationMembers AS om
        left outer join EnrollmentTransaction et ON om.OrganizationId = et.OrganizationId and om.PeopleId = et.PeopleId 
   WHERE om.Organizationid = @orgID AND et.EnrollmentDate is null
    
END
GO
