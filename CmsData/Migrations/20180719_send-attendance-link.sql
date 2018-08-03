ALTER TABLE dbo.Organizations ADD SendAttendanceLink bit NOT NULL CONSTRAINT DF_Organizations_SendAttendanceLink DEFAULT 0
GO
