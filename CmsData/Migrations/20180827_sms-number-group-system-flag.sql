IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SystemFlag'
          AND Object_ID = Object_ID(N'dbo.SMSGroups'))
BEGIN
    ALTER TABLE dbo.SMSGroups ADD SystemFlag bit NOT NULL CONSTRAINT DF_SMSGroups_SystemFlag DEFAULT 0
END
GO
IF OBJECT_ID(N'[dbo].[updSMSGroups]') IS NULL 
BEGIN
    EXEC ('
CREATE TRIGGER [dbo].[updSMSGroups] 
   ON  [dbo].[SMSGroups] 
   FOR UPDATE, INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF UPDATE(SystemFlag)
	BEGIN
		-- Only one group can be designated as SystemFlag = 1
		UPDATE dbo.SMSGroups SET SystemFlag = 0
		WHERE ID NOT IN (SELECT ID FROM INSERTED)
	END
END')
END
GO

ALTER TABLE [dbo].[SMSGroups] ENABLE TRIGGER [updSMSGroups]
GO


