IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'DidNotMeet'
          AND Object_ID = Object_ID(N'dbo.Meetings'))
BEGIN
    ALTER TABLE dbo.Meetings
    ADD DidNotMeet bit NOT NULL CONSTRAINT DF_Meetings_DidNotMeet DEFAULT 0
END
GO

IF NOT EXISTS(select 1 from dbo.SettingMetadata where SettingId = 'AttendanceReminderAuthenticatedLinks')
BEGIN
    DECLARE @Administration INT = 1, @dataBool INT = 1;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('AttendanceReminderAuthenticatedLinks', 'Set this to <i>true</i> to enable automatic sign in with links for group leader attendance reminder emails and SMS', @dataBool, @Administration)
END
GO
