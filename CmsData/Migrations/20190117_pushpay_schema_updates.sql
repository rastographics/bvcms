IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BatchPage'
          AND Object_ID = Object_ID(N'dbo.PushPayLog'))
BEGIN
	ALTER TABLE dbo.PushPayLog
	DROP COLUMN BatchPage, MerchantKey

    ALTER TABLE dbo.PushPayLog
    ADD PeopleId int NULL,
	BundleHeaderId int NULL,
	ContributionId [nvarchar](100) NULL,
	SettlementKey [nvarchar](100) NULL

    ALTER TABLE dbo.BundleHeader
    ADD ReferenceId int NULL,
    ReferenceIdType int NULL
END
GO
