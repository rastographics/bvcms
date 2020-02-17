IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'AutoSyncBatchDates')
    AND NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'AutoSyncBatchDatesWindow')
BEGIN
    DECLARE @contributionsCategoryId INT = 3, @dataBool INT = 1,  @dataInt INT = 5;

    SET IDENTITY_INSERT dbo.SettingCategory ON;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId, DefaultValue) VALUES
    ('AutoSyncBatchDates', 'Set this to <i>true</i> to automatically retrieve batch transaction data nightly.', @dataBool, @contributionsCategoryId, 1),
    ('AutoSyncBatchDatesWindow', 'The range of days in which to search for transactions that have not yet been retrieved (prior to the current date).', @dataInt, @contributionsCategoryId, 7)
END
GO
