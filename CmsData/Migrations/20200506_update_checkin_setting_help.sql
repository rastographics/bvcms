IF EXISTS(SELECT 1 FROM SettingMetadata
          WHERE SettingId = N'EarlyCheckin'
          AND Description like '%Attendance%')
BEGIN
    UPDATE dbo.SettingMetadata
    SET Description = 'The number of minutes before a meeting starts when people can start checking in. You can override this per organization in its Check In settings.'
    WHERE SettingId = 'EarlyCheckin'


    UPDATE dbo.SettingMetadata
    SET Description = 'The number of minutes after a meeting starts when check in will be unavailable. You can override this per organization in its Check In settings.'
    WHERE SettingId = 'LateCheckin'
END
GO