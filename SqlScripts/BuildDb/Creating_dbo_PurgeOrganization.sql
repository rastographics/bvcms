CREATE PROCEDURE [dbo].[PurgeOrganization](@oid INT)
AS
BEGIN
	BEGIN TRY 
		BEGIN TRANSACTION 
		DECLARE @fid INT, @pic INT
		DELETE FROM dbo.OrgMemMemTags WHERE OrgId = @oid
		DELETE FROM dbo.OrganizationMembers WHERE OrganizationId = @oid
		DELETE FROM dbo.PrevOrgMemberExtra WHERE OrganizationId = @oid
		DELETE FROM dbo.EnrollmentTransaction WHERE OrganizationId = @oid

		delete from dbo.Attend where OrganizationId = @oid
		delete from dbo.DivOrg where OrgId = @oid
        
		delete dbo.VolRequest
		from dbo.VolRequest vr
		join dbo.Meetings m on m.MeetingId = vr.MeetingId
		where m.OrganizationId = @oid

		DELETE FROM dbo.Meetings WHERE OrganizationId = @oid
		DELETE FROM dbo.MemberTags WHERE OrgId = @oid
		DELETE FROM dbo.Coupons WHERE OrgId = @oid
		DELETE FROM dbo.OrgSchedule WHERE OrganizationId = @oid
		DELETE FROM dbo.ResourceOrganization WHERE OrganizationId = @oid
		DELETE FROM dbo.OrganizationExtra WHERE OrganizationId = @oid
		UPDATE dbo.Organizations SET ParentOrgId = NULL WHERE ParentOrgId = @oid
		UPDATE dbo.ActivityLog
		SET OrgId = NULL
		WHERE OrgId = @oid
		DELETE FROM dbo.Organizations WHERE OrganizationId = @oid
		COMMIT
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
