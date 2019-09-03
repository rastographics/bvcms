IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'JsonData'
          AND Object_ID = Object_ID(N'dbo.PrintJob'))
BEGIN
    ALTER TABLE dbo.PrintJob
    ADD JsonData nvarchar(max) NULL
END
GO
