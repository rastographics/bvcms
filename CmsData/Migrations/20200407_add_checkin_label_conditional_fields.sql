IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'orgEV'
          AND Object_ID = Object_ID(N'dbo.CheckInLabelEntry'))
BEGIN
	ALTER TABLE dbo.CheckInLabelEntry
    ADD orgEV [nvarchar](50) CONSTRAINT orgEV_default DEFAULT NULL,
        personFlag [nvarchar](10) CONSTRAINT personFlag_default DEFAULT NULL
END
GO
