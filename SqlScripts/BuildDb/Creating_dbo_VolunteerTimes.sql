CREATE VIEW [dbo].[VolunteerTimes] AS
	SELECT
		OrganizationId,
		slot.value('(@Time)[1]', 'varchar(50)') AS [Time],
		slot.value('(@DayOfWeek)[1]', 'int') AS [DayOfWeek]
	FROM  
	      dbo.Organizations o 
		  CROSS APPLY RegSettingXml.nodes('/Settings/TimeSlots/Slot') AS TimeSlot(slot)
	WHERE o.RegistrationTypeId = 6 -- Choose Volunteer Times

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
