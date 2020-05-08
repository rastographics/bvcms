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

    UPDATE dbo.SettingMetadata
    SET Description = 'Enter the number of hours (plus or minus) offset from the Central Time zone. If you are in the Central Time zone, you can leave this blank. If, for example, you are in the Eastern Time zone, enter <i>1</i> for the value. Note this is only necessary for Legacy Check In.'
    WHERE SettingId = 'TZOffset'

END
GO