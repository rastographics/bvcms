IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'ShowSyncBatchDatesOption')
BEGIN
    DECLARE @contributionsCategoryId INT = 3, @dataBool INT = 1;

    SET IDENTITY_INSERT dbo.SettingCategory ON;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId, DefaultValue) VALUES
    ('ShowSyncBatchDatesOption', 'Set this to <i>true</i> to enable options to retrieve batch transaction data on demand.', @dataBool, @contributionsCategoryId, NULL)
END
GO
