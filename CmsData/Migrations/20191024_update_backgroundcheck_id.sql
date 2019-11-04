IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BillingRefId'
          AND Object_ID = Object_ID(N'dbo.BackgroundChecks'))
BEGIN
    ALTER TABLE dbo.BackgroundChecks ADD BillingRefId NVARCHAR(50) DEFAULT '' NOT NULL
END
GO

UPDATE dbo.BackgroundChecks 
SET BillingRefId = CONVERT(varchar(50), ID)
WHERE BillingRefId = ''
