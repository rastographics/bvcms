CREATE TRIGGER [dbo].[insOrganizationMember] 
   ON  [dbo].[OrganizationMembers]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE 
		@skipInsertTriggerProcessing BIT

	SELECT
		@skipInsertTriggerProcessing = SkipInsertTriggerProcessing
	FROM Inserted

	IF @skipInsertTriggerProcessing = 1
		RETURN

	UPDATE dbo.Organizations
	SET MemberCount = dbo.OrganizationMemberCount(OrganizationId),
		ProspectCount = dbo.OrganizationProspectCount(OrganizationId)
	WHERE OrganizationId IN 
	(SELECT OrganizationId FROM INSERTED)

	UPDATE dbo.People
	SET Grade = ISNULL(dbo.SchoolGrade(PeopleId), Grade),
	BibleFellowshipClassId = dbo.BibleFellowshipClassId(PeopleId)
	WHERE PeopleId IN (SELECT PeopleId FROM INSERTED)

	--DECLARE c CURSOR FOR
	--SELECT d.OrganizationId, MemberTypeId, o.LeaderMemberTypeId FROM INSERTED d
	--JOIN dbo.Organizations o ON o.OrganizationId = d.OrganizationId
	--OPEN c;
	--DECLARE @oid INT, @mt INT, @lmt INT
	--FETCH NEXT FROM c INTO @oid, @mt, @lmt;
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
	--	IF (@mt = @lmt)
	--		UPDATE dbo.Organizations
	--		SET LeaderId = dbo.OrganizationLeaderId(OrganizationId),
	--		LeaderName = dbo.OrganizationLeaderName(OrganizationId)
	--		WHERE OrganizationId = @oid

	--		UPDATE dbo.People
	--		SET BibleFellowshipClassId = dbo.BibleFellowshipClassId(p.PeopleId)
	--		FROM dbo.People p
	--		JOIN dbo.OrganizationMembers om ON p.PeopleId = om.PeopleId
	--		WHERE om.OrganizationId = @oid
	--	FETCH NEXT FROM c INTO @oid, @mt, @lmt;
	--END;
	--CLOSE c;
	--DEALLOCATE c;
	
	UPDATE dbo.Organizations
	SET LeaderId = dbo.OrganizationLeaderId(o.OrganizationId),
	LeaderName = dbo.OrganizationLeaderName(o.OrganizationId)
	FROM dbo.Organizations o
	JOIN INSERTED m ON o.OrganizationId = m.OrganizationId
	WHERE m.MemberTypeId = o.LeaderMemberTypeId

	UPDATE dbo.OrganizationMembers
	SET LastAttended = dbo.LastAttended(i.OrganizationId, i.PeopleId)
	FROM dbo.OrganizationMembers m
	JOIN Inserted i ON i.PeopleId = m.PeopleId AND i.OrganizationId = m.OrganizationId

	--UPDATE dbo.People
	--SET BibleFellowshipClassId = dbo.BibleFellowshipClassId(p.PeopleId)
	--FROM dbo.People p
	--JOIN dbo.OrganizationMembers om ON p.PeopleId = om.PeopleId
	--WHERE om.OrganizationId IN (SELECT OrganizationId FROM INSERTED)
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
