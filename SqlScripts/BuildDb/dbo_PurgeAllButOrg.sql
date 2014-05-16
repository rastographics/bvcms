-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PurgeAllButOrg](@orgid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

DECLARE @people TABLE ( id INT )

INSERT INTO @people
SELECT PeopleId
FROM dbo.People p
WHERE NOT EXISTS(
	SELECT NULL FROM dbo.OrganizationMembers om
	JOIN dbo.People pp ON om.PeopleId = pp.PeopleId
	WHERE OrganizationId = @orgid AND pp.FamilyId = p.FamilyId
	)
	AND NOT EXISTS(
	SELECT NULL FROM dbo.EnrollmentTransaction et
	JOIN dbo.People pp ON et.PeopleId = pp.PeopleId
	WHERE OrganizationId = @orgid AND pp.FamilyId = p.FamilyId
	)
	
DELETE dbo.SoulMate
	
DELETE dbo.LoveRespect

DELETE dbo.OrgMemMemTags
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.OrganizationMembers
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.BadET
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.EnrollmentTransaction
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.MOBSReg
WHERE PeopleId IN (SELECT id FROM @people)
	
DELETE dbo.Attend
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.BundleDetail
FROM dbo.BundleDetail bd
JOIN dbo.Contribution c ON bd.ContributionId = c.ContributionId
WHERE PeopleId IN (SELECT id FROM @people)
	
DELETE dbo.Contribution
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.VolunteerForm
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.Volunteer
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.Contactees
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.Contactors
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.TagPerson
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.Task
WHERE WhoId IN (SELECT id FROM @people)

DELETE dbo.Task
WHERE OwnerId IN (SELECT id FROM @people)

DELETE dbo.Task
WHERE CoOwnerId IN (SELECT id FROM @people)

DELETE dbo.TaskListOwners
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.Preferences
WHERE UserId IN (SELECT UserId FROM Users
WHERE PeopleId IN (SELECT id FROM @people))

DELETE dbo.ActivityLog
WHERE UserId IN (SELECT UserId FROM Users
WHERE PeopleId IN (SELECT id FROM @people))

DELETE dbo.UserRole
WHERE UserId IN (SELECT UserId FROM Users
WHERE PeopleId IN (SELECT id FROM @people))

DELETE FROM dbo.PeopleCanEmailFor WHERE OnBehalfOf IN (SELECT id FROM @people)
DELETE FROM dbo.PeopleCanEmailFor WHERE CanEmail IN (SELECT id FROM @people)
		
DELETE dbo.VolunteerForm
WHERE UploaderId IN (SELECT UserId FROM Users
WHERE PeopleId IN (SELECT id FROM @people))

DELETE dbo.Users
WHERE UserId IN (SELECT UserId FROM Users
WHERE PeopleId IN (SELECT id FROM @people))
	
DELETE dbo.TagPerson
DELETE dbo.TagShare
DELETE dbo.Tag

DELETE dbo.RecReg
WHERE PeopleId IN (SELECT id FROM @people)
	
DELETE dbo.VBSApp
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.VolInterestInterestCodes

DELETE dbo.VolInterestCodes

UPDATE dbo.Families
SET HeadOfHouseholdId = NULL, HeadOfHouseholdSpouseId = NULL

UPDATE dbo.People
SET PictureId = NULL 
WHERE PeopleId IN (SELECT id FROM @people)

DELETE dbo.Picture
FROM dbo.Picture pic
WHERE NOT EXISTS(SELECT NULL FROM dbo.People WHERE PictureId = pic.PictureId)

---------------------------------------
DELETE dbo.People
WHERE PeopleId IN (SELECT id FROM @people)
---------------------------------------

DELETE dbo.RelatedFamilies
FROM dbo.RelatedFamilies r
JOIN dbo.Families f ON r.FamilyId = f.FamilyId
WHERE (SELECT COUNT(*) FROM dbo.People WHERE FamilyId = f.FamilyId) = 0

DELETE dbo.RelatedFamilies
FROM dbo.RelatedFamilies r
JOIN dbo.Families f ON r.RelatedFamilyId = f.FamilyId
WHERE (SELECT COUNT(*) FROM dbo.People WHERE FamilyId = f.FamilyId) = 0

DELETE dbo.Families 
FROM dbo.Families f
WHERE (SELECT COUNT(*) FROM dbo.People WHERE FamilyId = f.FamilyId) = 0
	
----------------------------------------------------------------------------
DELETE dbo.Contact
FROM dbo.Contact nc
WHERE NOT EXISTS(SELECT NULL FROM dbo.Contactees WHERE ContactId = nc.ContactId)
AND NOT EXISTS(SELECT NULL FROM dbo.Contactors WHERE ContactId = nc.ContactId)

DELETE dbo.Meetings
FROM dbo.Meetings m
WHERE NOT EXISTS(SELECT NULL FROM dbo.Attend WHERE MeetingId = m.MeetingId)

DELETE dbo.DivOrg
FROM dbo.DivOrg od
JOIN dbo.Organizations o ON od.OrgId = o.OrganizationId
WHERE NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers WHERE OrganizationId = o.OrganizationId)
AND NOT EXISTS(SELECT NULL FROM dbo.EnrollmentTransaction WHERE OrganizationId = o.OrganizationId)

DELETE dbo.MemberTags
FROM dbo.MemberTags mt
JOIN dbo.Organizations o ON mt.OrgId = o.OrganizationId
WHERE NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers WHERE OrganizationId = o.OrganizationId)

DELETE dbo.Organizations
FROM dbo.Organizations o
WHERE NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers WHERE OrganizationId = o.OrganizationId)
AND NOT EXISTS(SELECT NULL FROM dbo.EnrollmentTransaction WHERE OrganizationId = o.OrganizationId)
AND NOT EXISTS(SELECT NULL FROM dbo.Attend WHERE OrganizationId = o.OrganizationId)

DELETE dbo.Promotion

DELETE dbo.Division
FROM dbo.Division d
WHERE NOT EXISTS(SELECT NULL FROM dbo.DivOrg WHERE DivId = d.Id)
AND NOT EXISTS(SELECT NULL FROM dbo.Organizations WHERE DivisionId = d.Id)
AND NOT EXISTS(SELECT NULL FROM dbo.RecLeague WHERE DivId = d.Id)

DELETE dbo.Program
FROM dbo.Program p
WHERE NOT EXISTS(SELECT NULL FROM dbo.Division WHERE ProgId = p.Id)

DELETE dbo.TaskList
FROM dbo.TaskList t
WHERE NOT EXISTS(SELECT NULL FROM  dbo.Users WHERE UserId = t.CreatedBy)

DELETE Tag
FROM dbo.Tag t
WHERE PeopleId IS NULL
AND NOT EXISTS(SELECT NULL FROM dbo.TagPerson WHERE Id = t.Id)

DELETE dbo.BundleDetail
FROM dbo.BundleDetail bd
WHERE EXISTS(SELECT NULL FROM dbo.Contribution WHERE ContributionId = bd.ContributionId AND PeopleId IS NULL)

DELETE FROM dbo.Contribution WHERE PeopleId IS NULL

DELETE dbo.BundleHeader
FROM dbo.BundleHeader h
WHERE NOT EXISTS(SELECT NULL FROM dbo.BundleDetail WHERE BundleHeaderId = h.BundleHeaderId)

DELETE FROM dbo.AuditValues
DELETE FROM dbo.Audits

EXEC dbo.DeleteAllQueriesWithNoChildren
EXEC dbo.DeleteAllQueriesWithNoChildren
EXEC dbo.DeleteAllQueriesWithNoChildren
EXEC dbo.DeleteAllQueriesWithNoChildren

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
