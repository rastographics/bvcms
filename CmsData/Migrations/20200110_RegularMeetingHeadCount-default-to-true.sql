UPDATE dbo.Setting
SET Setting = 'true'
WHERE Id = 'RegularMeetingHeadCount' AND Setting = 'enable'
GO

UPDATE dbo.Setting
SET Setting = 'false'
WHERE Id = 'RegularMeetingHeadCount' AND Setting = 'disable'
GO

IF NOT EXISTS(SELECT 1 FROM dbo.Setting WHERE Id = 'RegularMeetingHeadCount')
INSERT INTO dbo.Setting (Id, Setting)
VALUES ('RegularMeetingHeadCount', 'true')
GO
