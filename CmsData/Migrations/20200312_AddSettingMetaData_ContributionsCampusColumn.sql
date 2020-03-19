IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'ContributionsCampusColumn')
BEGIN
    DECLARE @Contributions INT = 3, @dataBool INT = 1;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('ContributionsCampusColumn', 'Set this to <i>true</i> to enable display of Contribution CampusCode on Contribution Search Results.', @dataBool, @Contributions)
END
GO
