
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'active'
          AND Object_ID = Object_ID(N'dbo.CheckInLabel'))
BEGIN
    ALTER TABLE CheckInLabel
    ADD [active] BIT CONSTRAINT active_default DEFAULT 0 NOT NULL
END
GO

IF (select count(*) from CheckInLabel where [active] = 1) = 0
BEGIN
    UPDATE dbo.CheckInLabel
    SET [active] = 1
    WHERE id <= 12
END
GO
