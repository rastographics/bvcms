CREATE TABLE [dbo].[BundleHeader]
(
[BundleHeaderId] [int] NOT NULL IDENTITY(1, 1),
[ChurchId] [int] NOT NULL,
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[RecordStatus] [bit] NOT NULL,
[BundleStatusId] [int] NOT NULL,
[ContributionDate] [datetime] NOT NULL,
[BundleHeaderTypeId] [int] NOT NULL,
[DepositDate] [datetime] NULL,
[BundleTotal] [numeric] (10, 2) NULL,
[TotalCash] [numeric] (10, 2) NULL,
[TotalChecks] [numeric] (10, 2) NULL,
[TotalEnvelopes] [numeric] (10, 2) NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[FundId] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
