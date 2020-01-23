DECLARE @dataBool INT = 1, @dataDate INT = 2, @dataText INT = 3, @dataPassword INT = 4, @dataInt INT = 5;
DECLARE
@Administration INT = 1,
@ChurchInfo INT = 2,
@Contributions INT = 3,
@Security INT = 4,
@CheckIn INT = 5,
@Contacts INT = 6,
@MobileApp INT = 7,
@Registrations INT = 8,
@Resources INT = 9,
@SmallGroupFinder INT = 10,
@ProtectMyMinistry INT = 11,
@Pushpay INT = 12,
@Rackspace INT = 13,
@Twilio INT = 14;
IF NOT EXISTS(SELECT 1 FROM SettingMetadata
          WHERE SettingId = N'EarlyCheckin')
BEGIN
    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES 
    ('EarlyCheckin', 'The number of minutes before a meeting starts when people can start checking in. You can override this per organization in Attendance settings.', @dataInt, @CheckIn),
    ('LateCheckin', 'The number of minutes after a meeting starts when check in will be unavailable. You can override this per organization in Attendance settings.', @dataInt, @CheckIn)
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'EarlyCheckIn'
          AND Object_ID = Object_ID(N'dbo.Organizations'))
BEGIN
    ALTER TABLE dbo.Organizations
    ADD EarlyCheckin INT NULL,
    LateCheckin INT NULL
END
GO
