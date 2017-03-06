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
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
