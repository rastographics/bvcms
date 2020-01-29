IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BarcodeId'
          AND Object_ID = Object_ID(N'dbo.People'))
BEGIN
    ALTER TABLE dbo.People
    ADD BarcodeId UNIQUEIDENTIFIER DEFAULT NULL,
    BarcodeExpires DATETIME DEFAULT NULL
END
GO
