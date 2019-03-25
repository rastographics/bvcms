ALTER FUNCTION [dbo].[OrgMembersAsOfDate](@orgid INT, @meetingdt DATETIME) 
 
RETURNS TABLE  
AS 
RETURN  
( 
SELECT  
	p.PeopleId, 
	p.FamilyId, 
	p.PreferredName, 
	p.LastName, 
	p.BirthDay, 
	p.BirthMonth, 
	p.BirthYear, 
	p.PrimaryAddress, 
	p.PrimaryAddress2, 
	p.PrimaryCity, 
	p.PrimaryState, 
	p.PrimaryZip, 
	p.HomePhone, 
	p.CellPhone, 
	p.WorkPhone, 
	p.EmailAddress, 
	ms.Description MemberStatus, 
	pp.Name BFTeacher, 
	pp.PeopleId BFTeacherId, 
	p.Age, 
	mt.Description MemberType, 
	mt.Id MemberTypeId, 
	ISNULL(ee.EnrollmentDate, ee.TransactionDate) Joined 
FROM dbo.EnrollmentTransaction ee 
JOIN dbo.People p ON ee.PeopleId = p.PeopleId 
JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id 
JOIN lookup.MemberType mt ON ee.MemberTypeId = mt.Id 
LEFT JOIN dbo.Organizations o ON p.BibleFellowshipClassId = o.OrganizationId 
LEFT JOIN dbo.People pp ON o.LeaderId = pp.PeopleId 
WHERE ee.OrganizationId = @orgid 
AND ee.TransactionTypeId <= 3 -- Enrollment transaction 
AND ee.TransactionStatus = 0 
AND ee.MemberTypeId != 311 -- not a prospect 
AND ISNULL(ee.Pending, 0) = 0 -- not a pending member 
AND ee.EnrollmentDate < @meetingdt -- *enrolled before the meetingdate
AND (ee.NextTranChangeDate >= @meetingdt -- still enrolled 
	OR ee.NextTranChangeDate IS NULL) -- or no change in status 
AND (ee.NextTranChangeDate != NULL 
	OR ee.TransactionId =  
		(SELECT TOP 1 TransactionId 
		 FROM dbo.EnrollmentTransaction  
		 WHERE PeopleId = p.PeopleId  
		 AND OrganizationId = ee.OrganizationId  
		 AND TransactionTypeId <= 3  
		 AND ee.EnrollmentDate < @meetingdt -- *enrolled before the meetingdate
		 ORDER BY TransactionDate DESC 
		 ) 
	) 
)
GO
