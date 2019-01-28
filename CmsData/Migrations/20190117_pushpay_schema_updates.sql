IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BatchPage'
          AND Object_ID = Object_ID(N'dbo.PushPayLog'))
BEGIN
	ALTER TABLE dbo.PushPayLog
	DROP COLUMN BatchPage

    ALTER TABLE dbo.PushPayLog
    ADD BundleHeaderId int NULL,
	ContributionId int NULL,
    OrganizationKey [nvarchar](100) NULL,
	SettlementKey [nvarchar](100) NULL

    EXEC sp_rename 'PushPayLog.BatchDate', 'TransactionDate', 'COLUMN'

    ALTER TABLE dbo.BundleHeader
    ADD ReferenceId [nvarchar](100) NULL,
    ReferenceIdType int NULL
END
GO
