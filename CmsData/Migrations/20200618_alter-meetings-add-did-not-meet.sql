IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'DidNotMeet'
          AND Object_ID = Object_ID(N'dbo.Meetings'))
BEGIN
    ALTER TABLE dbo.Meetings
    ADD DidNotMeet bit NOT NULL CONSTRAINT DF_Meetings_DidNotMeet DEFAULT 0
END
GO
