IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'ContributionFilterDefault')
BEGIN
    DECLARE @contributionsCategoryId INT = 3, @dataString INT = 3;

    SET IDENTITY_INSERT dbo.SettingCategory ON;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId, DefaultValue) VALUES
    ('ContributionFilterDefault', 'Determines the default filter when viewing Contributions. Available options: "YearToDate", "PreviousAndCurrent", & "AllYears".', @dataString, @contributionsCategoryId, 'YearToDate');
    
    SET IDENTITY_INSERT dbo.SettingCategory OFF;
END
GO
