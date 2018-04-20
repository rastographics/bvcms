CREATE PROCEDURE [dbo].[PurgePerson](@pid INT) 
AS 
BEGIN 
	BEGIN TRY  
	  IF(@pid > 1) 
	  BEGIN 
		BEGIN TRANSACTION  
		DECLARE @fid INT, @pic INT 
		DELETE dbo.OrgMemMemTags WHERE PeopleId = @pid 
		DELETE dbo.OrgMemberExtra WHERE PeopleId = @pid 
		DELETE dbo.OrganizationMembers WHERE PeopleId = @pid 
		DELETE dbo.EnrollmentTransaction WHERE PeopleId = @pid 
		DELETE dbo.CardIdentifiers WHERE PeopleId = @pid 
		 
		DELETE dbo.CheckInActivity 
		FROM dbo.CheckInActivity a 
		JOIN dbo.CheckInTimes t ON a.Id = t.Id 
		WHERE t.PeopleId = @pid 
		DELETE FROM dbo.CheckInTimes WHERE PeopleId = @pid 
		 
		DELETE dbo.PeopleExtra WHERE PeopleId = @pid 
		DELETE dbo.TransactionPeople WHERE PeopleId = @pid 
		DELETE dbo.EmailOptOut WHERE ToPeopleId = @pid 
		DELETE dbo.OrgMemberExtra WHERE PeopleId = @pid 
		DELETE dbo.PrevOrgMemberExtra WHERE PeopleId = @pid 
		UPDATE dbo.[Transaction] SET LoginPeopleId = NULL WHERE LoginPeopleId = @pid
 
		DELETE dbo.EmailResponses  
		FROM dbo.EmailResponses r 
		JOIN dbo.EmailQueue e ON e.Id = r.EmailQueueId 
		WHERE QueuedBy = @pid 
 
		DELETE dbo.EmailResponses WHERE PeopleId = @pid 
		DELETE dbo.EmailQueueTo WHERE PeopleId = @pid; 
		 
		--DELETE dbo.EmailQueueTo FROM dbo.EmailQueueTo et  
		--JOIN EmailQueue e ON e.Id = et.Id 
		--WHERE QueuedBy = @pid 

		UPDATE dbo.EmailQueue SET QueuedBy = NULL WHERE QueuedBy = @pid
		--DELETE dbo.EmailQueue WHERE QueuedBy = @pid; 
 
		DELETE dbo.GoerSupporter WHERE SupporterId = @pid OR GoerId = @pid 
		DELETE dbo.GoerSenderAmounts WHERE SupporterId = @pid OR GoerId = @pid 
		 
		UPDATE dbo.ActivityLog 
		SET PeopleId = NULL 
		WHERE PeopleId = @pid 
		 
		DELETE dbo.FamilyCheckinLock 
		FROM dbo.Families f 
		JOIN dbo.People p ON f.FamilyId = p.FamilyId 
		WHERE p.PeopleId = @pid 
 
		DECLARE @t TABLE(id int) 
		INSERT INTO @t(id) SELECT MeetingId FROM dbo.Attend a WHERE a.PeopleId = @pid 
		 
		DELETE dbo.SubRequest WHERE RequestorId = @pid 
		DELETE dbo.SubRequest WHERE SubstituteId = @pid 
		DELETE dbo.Attend WHERE PeopleId = @pid 
		 
		DECLARE cur CURSOR FOR SELECT id FROM @t 
		OPEN cur 
		DECLARE @mid int 
		FETCH NEXT FROM cur INTO @mid 
		WHILE @@FETCH_STATUS = 0 
		BEGIN 
			EXECUTE dbo.UpdateMeetingCounters @mid 
			FETCH NEXT FROM cur INTO @mid 
		END 
		CLOSE cur 
		DEALLOCATE cur 
		 
		UPDATE dbo.Contribution SET PeopleId = NULL WHERE PeopleId = @pid 
		UPDATE dbo.Families SET HeadOfHouseholdId = NULL WHERE HeadOfHouseholdId = @pid 
		UPDATE dbo.Families SET HeadOfHouseholdSpouseId = NULL WHERE HeadOfHouseholdSpouseId = @pid 
		 
		DELETE dbo.VolunteerForm WHERE PeopleId = @pid 
		DELETE dbo.VoluteerApprovalIds WHERE PeopleId = @pid 
		DELETE dbo.Volunteer WHERE PeopleId = @pid 
		DELETE dbo.VolRequest WHERE RequestorId = @pid 
		DELETE dbo.VolRequest WHERE VolunteerId = @pid 
		DELETE dbo.Contactees WHERE PeopleId = @pid 
		DELETE dbo.Contactors WHERE PeopleId = @pid 
		DELETE dbo.TagPerson WHERE PeopleId = @pid 
		DELETE dbo.Task WHERE WhoId = @pid 
		DELETE dbo.Task WHERE OwnerId = @pid 
		DELETE dbo.Task WHERE CoOwnerId = @pid 
		DELETE dbo.TaskListOwners WHERE PeopleId = @pid 
		DELETE dbo.RecurringAmounts WHERE PeopleId = @pid 
		DELETE dbo.ManagedGiving WHERE PeopleId = @pid 
		DELETE dbo.PaymentInfo WHERE PeopleId = @pid 
		DELETE dbo.MemberDocForm WHERE PeopleId = @pid 
		DELETE dbo.MobileAppPushRegistrations WHERE PeopleId = @pid
		DELETE dbo.MobileAppDevices WHERE peopleID = @pid
		 
		DELETE dbo.Preferences WHERE UserId IN (SELECT UserId FROM dbo.Users WHERE PeopleId = @pid) 
		DELETE dbo.ActivityLog WHERE UserId IN (SELECT UserId FROM dbo.Users WHERE PeopleId = @pid) 
		DELETE dbo.UserRole WHERE UserId IN (SELECT UserId FROM dbo.Users WHERE PeopleId = @pid) 
		DELETE dbo.PeopleCanEmailFor WHERE OnBehalfOf = @pid 
		DELETE dbo.PeopleCanEmailFor WHERE CanEmail = @pid 
		 
		UPDATE dbo.VolunteerForm  
		SET UploaderId = NULL  
		WHERE UploaderId IN (SELECT UserId FROM dbo.Users WHERE PeopleId = @pid) 
		 
		DELETE dbo.Coupons 
		FROM Coupons c 
		JOIN users u ON c.UserId = u.UserId 
		WHERE u.PeopleId = @pid 
		 
		DELETE dbo.Coupons WHERE PeopleId = @pid 
		 
		DELETE dbo.Users WHERE UserId IN (SELECT UserId FROM dbo.Users WHERE PeopleId = @pid) 
		 
		DELETE dbo.TagPerson WHERE id IN (SELECT Id FROM dbo.Tag WHERE PeopleId = @pid) 
		DELETE dbo.TagShare WHERE TagId IN (SELECT Id FROM dbo.Tag WHERE PeopleId = @pid) 
		DELETE dbo.TagShare WHERE PeopleId = @pid 
		DELETE dbo.Tag WHERE PeopleId = @pid 
		 
		DELETE dbo.RecReg WHERE PeopleId = @pid 
		DELETE dbo.EmailQueueTo WHERE PeopleId = @pid 
		DELETE dbo.SMSItems WHERE PeopleID = @pid 
		 
		DELETE dbo.VolInterestInterestCodes 
		FROM dbo.VolInterestInterestCodes vc 
		WHERE vc.PeopleId = @pid 

		DELETE dbo.MobileAppDevices WHERE peopleID = @pid
		 
		SELECT @fid = FamilyId, @pic = PictureId FROM dbo.People WHERE PeopleId = @pid 
		DELETE dbo.FamilyExtra WHERE FamilyId = @fid 
		 
		UPDATE dbo.Families 
		SET HeadOfHouseholdId = NULL, HeadOfHouseholdSpouseId = NULL 
		WHERE FamilyId = @fid AND HeadOfHouseholdId = @pid 
		OR FamilyId = @fid AND HeadOfHouseholdSpouseId = @pid 
 
		DELETE dbo.SubRequest 
		WHERE RequestorId = @pid OR SubstituteId = @pid 
		 
		DELETE dbo.People WHERE PeopleId = @pid 
		 
		IF (SELECT COUNT(*) FROM dbo.People WHERE FamilyId = @fid) = 0 
		BEGIN 
			DELETE dbo.RelatedFamilies WHERE FamilyId = @fid OR RelatedFamilyId = @fid 
			DELETE dbo.Families WHERE FamilyId = @fid			 
		END 
		DELETE dbo.Picture WHERE PictureId = @pic 
		 
		COMMIT 
	  END 
	END TRY  
	BEGIN CATCH  
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION; 
		DECLARE @ErrorMessage NVARCHAR(4000); 
		DECLARE @ErrorSeverity INT; 
		DECLARE @ErrorState INT; 
		SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE(); 
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState); 
	END CATCH  
END 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
