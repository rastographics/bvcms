IF NOT EXISTS(SELECT 1 FROM dbo.SettingMetadata WHERE SettingId = 'UseQuarterlyRecurring')
BEGIN

DECLARE @Contributions INT = 3, @dataBool INT = 1;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('UseQuarterlyRecurring', 'This setting allows you to set quarterly as one of the recurring giving options.', @dataBool, @Contributions),
    ('UseAnnualRecurring', 'This setting allows you to set annually as one of the recurring giving options.', @dataBool, @Contributions),
    ('HideBiWeeklyRecurring', 'This setting allows you to remove the biweekly recurring giving option.', @dataBool, @Contributions),
    ('HideSemiMonthlyRecurring', 'This setting allows you to remove the semi-monthly recurring giving option.', @dataBool, @Contributions)
END
GO