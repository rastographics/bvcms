IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'RemoteDepositCaptureEnabled')
BEGIN
    DECLARE @Contributions INT = 3, @dataBool INT = 1;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('RemoteDepositCaptureEnabled', 'Set this to <i>true</i> to enable remote deposit capture features.  This feature may require additional setup to comply with your bank. Refer to <a href="https://docs.touchpointsoftware.com/Finance/Bundle_Index.html#remote-deposit-capture" target="_blank">Remote Deposit Capture</a> documentation for more information.', @dataBool, @Contributions)
END
GO