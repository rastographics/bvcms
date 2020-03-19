IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'BundleEntryCampusCode')
BEGIN
    DECLARE @Contributions INT = 3, @dataBool INT = 1;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('BundleEntryCampusCode', 'Set this to <i>true</i> to enable display of person''s CampusCode when searching in the name field.', @dataBool, @Contributions)
END
GO
