IF NOT EXISTS(select 1 from dbo.SettingCategory where Name = 'eSpace')
BEGIN
    DECLARE @Integrations INT = 3, @eSpace INT = 15, @dataBool INT = 1, @dataPassword INT = 4, @dataInt INT = 5;

    SET IDENTITY_INSERT dbo.SettingCategory ON;
    
    INSERT INTO dbo.SettingCategory
    (SettingCategoryId, Name, SettingTypeId, DisplayOrder) VALUES
    (@eSpace, 'eSpace', @Integrations, 5);

    SET IDENTITY_INSERT dbo.SettingCategory OFF;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('eSpaceEnabled', 'Set this to <i>true</i> to enable eSpace integration features', @dataBool, @eSpace),
    ('eSpaceUserName', 'The eSpace user name (email) to use for the integration', @dataPassword, @eSpace),
    ('eSpacePassword', 'The eSpace password to use for the integration', @dataPassword, @eSpace),
    ('eSpaceDaysToSync', 'The number of days in the future to search for data from eSpace that will be synced (The default is 60 days)', @dataInt, @eSpace)
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name = 'ESpaceEventId' AND object_id = OBJECT_ID('dbo.Organizations'))
ALTER TABLE dbo.Organizations ADD
	ESpaceEventId bigint NULL,
	ESpaceEventName nvarchar(200) NULL
GO