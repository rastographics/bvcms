CREATE TRIGGER [dbo].[delOrg] 
   ON  [dbo].[Organizations] 
   AFTER DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT NULL FROM dbo.Organizations)
	BEGIN
		SET IDENTITY_INSERT [dbo].[Organizations] ON
		INSERT INTO [dbo].[Organizations] ([OrganizationId], [CreatedBy], [CreatedDate], [OrganizationStatusId], [DivisionId], [LeaderMemberTypeId], [GradeAgeStart], [GradeAgeEnd], [RollSheetVisitorWks], [SecurityTypeId], [FirstMeetingDate], [LastMeetingDate], [OrganizationClosedDate], [Location], [OrganizationName], [ModifiedBy], [ModifiedDate], [EntryPointId], [ParentOrgId], [AllowAttendOverlap], [MemberCount], [LeaderId], [LeaderName], [ClassFilled], [OnLineCatalogSort], [PendingLoc], [CanSelfCheckin], [NumCheckInLabels], [CampusId], [AllowNonCampusCheckIn], [NumWorkerCheckInLabels], [ShowOnlyRegisteredAtCheckIn], [Limit], [GenderId], [Description], [BirthDayStart], [BirthDayEnd], [LastDayBeforeExtra], [RegistrationTypeId], [ValidateOrgs], [PhoneNumber], [RegistrationClosed], [AllowKioskRegister], [WorshipGroupCodes], [IsBibleFellowshipOrg], [NoSecurityLabel], [AlwaysSecurityLabel], [DaysToIgnoreHistory], [NotifyIds], [lat], [long], [RegSetting], [OrgPickList], [Offsite]) VALUES (1, 1, '2009-05-05T22:46:43.983', 30, 1, NULL, 0, 0, NULL, 0, NULL, NULL, NULL, '', 'First Organization', NULL, NULL, 0, NULL, 0, 2, NULL, NULL, 0, NULL, NULL, 0, 0, NULL, 0, 0, NULL, NULL, 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
	END
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
