CREATE TRIGGER [dbo].[updOrganizationMember] 
   ON  [dbo].[OrganizationMembers]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF UPDATE(Pending) OR UPDATE(MemberTypeId) OR UPDATE(EnrollmentDate)
	BEGIN
		UPDATE dbo.People
		SET Grade = ISNULL(dbo.SchoolGrade(PeopleId), Grade)
		WHERE PeopleId IN (SELECT PeopleId FROM INSERTED)

		UPDATE dbo.Organizations
		SET MemberCount = dbo.OrganizationMemberCount(OrganizationId),
			ProspectCount = dbo.OrganizationProspectCount(OrganizationId)
		WHERE OrganizationId IN 
		(SELECT OrganizationId FROM INSERTED GROUP BY OrganizationId)
	END

	DECLARE c CURSOR FOR
	SELECT d.OrganizationId, d.MemberTypeId, i.MemberTypeId, o.LeaderMemberTypeId, d.PeopleId, i.Pending, d.Pending, d.EnrollmentDate, i.EnrollmentDate
	FROM DELETED d
	JOIN INSERTED i ON i.OrganizationId = d.OrganizationId AND i.PeopleId = d.PeopleId
	JOIN dbo.Organizations o ON o.OrganizationId = d.OrganizationId
	
    IF UPDATE(MemberTypeId) OR UPDATE(Pending) OR UPDATE(EnrollmentDate)
    BEGIN
		OPEN c;
		DECLARE @oid INT, @dmt INT, @imt INT, @lmt INT, @pid INT, @ipending BIT, @dpending BIT, @denrolldt DATETIME, @ienrolldt DATETIME
		FETCH NEXT FROM c INTO @oid, @dmt, @imt, @lmt, @pid, @ipending, @dpending, @denrolldt, @ienrolldt;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (@imt <> @dmt OR  @ipending <> @dpending OR @ienrolldt <> @denrolldt)
			BEGIN
				DECLARE @oname nvarchar(100), @att REAL
				SELECT @oname = o.OrganizationName, @att = om.AttendPct
				FROM dbo.OrganizationMembers om
				JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
				WHERE om.OrganizationId = @oid AND om.PeopleId = @pid
				
				INSERT dbo.EnrollmentTransaction
				        ( CreatedDate ,
				          TransactionDate ,
				          TransactionTypeId ,
				          OrganizationId ,
				          OrganizationName ,
				          PeopleId ,
				          MemberTypeId ,
				          AttendancePercentage ,
				          Pending,
				          EnrollmentDate
				        )
				VALUES  ( GETDATE() , -- CreatedDate - datetime
				          GETDATE() , -- TransactionDate - datetime
				          3 , -- TransactionTypeId - int
				          @oid , -- OrganizationId - int
				          @oname , -- OrganizationName - nvarchar(100)
				          @pid , -- PeopleId - int
				          @imt , -- MemberTypeId - int
				          @att , -- AttendancePercentage - real
				          @ipending,  -- Pending - bit
				          @ienrolldt -- EnrollmentDate
				        )
			END
			IF (@ienrolldt <> @denrolldt) -- need to update the most recent enrollment transactino
			BEGIN
				UPDATE dbo.EnrollmentTransaction
				SET EnrollmentDate = @ienrolldt, TransactionDate = @ienrolldt
				WHERE TransactionId = (SELECT MAX(TransactionId) FROM dbo.EnrollmentTransaction WHERE PeopleId = @pid AND OrganizationId = @oid AND TransactionTypeId = 1)
			END
			IF (@dmt = @lmt OR @imt = @lmt OR @ipending <> @dpending)
			BEGIN
				UPDATE dbo.Organizations
				SET LeaderId = dbo.OrganizationLeaderId(OrganizationId),
				LeaderName = dbo.OrganizationLeaderName(OrganizationId)
				WHERE OrganizationId = @oid
				
				UPDATE dbo.People
				SET BibleFellowshipClassId = dbo.BibleFellowshipClassId(p.PeopleId)
				FROM dbo.People p
				JOIN dbo.OrganizationMembers om ON p.PeopleId = om.PeopleId
				WHERE om.OrganizationId = @oid
			END
			FETCH NEXT FROM c INTO @oid, @dmt, @imt, @lmt, @pid, @ipending, @dpending, @denrolldt, @ienrolldt;
		END;
		CLOSE c;
		DEALLOCATE c;
	END

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
