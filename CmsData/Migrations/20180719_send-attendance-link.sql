IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SendAttendanceLink'
          AND Object_ID = Object_ID(N'dbo.Organizations'))
BEGIN
    ALTER TABLE dbo.Organizations ADD SendAttendanceLink bit NOT NULL CONSTRAINT DF_Organizations_SendAttendanceLink DEFAULT 0
END
