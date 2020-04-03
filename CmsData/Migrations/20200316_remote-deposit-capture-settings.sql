IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'RemoteDepositFilename')
BEGIN
    DECLARE @Contributions INT = 3, @dataText INT = 3;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('RemoteDepositFilename', 'Customizes the downloaded file name for remote deposit capture export.  Refer to <a href="https://docs.touchpointsoftware.com/Finance/Bundle_Index.html#remote-deposit-capture" target="_blank">Remote Deposit Capture</a> documentation for more information.', @dataText, @Contributions)
END
GO
