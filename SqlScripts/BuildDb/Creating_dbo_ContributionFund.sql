CREATE TABLE [dbo].[ContributionFund]
(
[FundId] [int] NOT NULL,
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[FundName] [nvarchar] (256) NOT NULL,
[FundDescription] [nvarchar] (256) NULL,
[FundStatusId] [int] NOT NULL,
[FundTypeId] [int] NOT NULL,
[FundPledgeFlag] [bit] NOT NULL,
[FundAccountCode] [int] NULL,
[FundIncomeDept] [nvarchar] (25) NULL,
[FundIncomeAccount] [nvarchar] (25) NULL,
[FundIncomeFund] [nvarchar] (25) NULL,
[FundCashDept] [nvarchar] (25) NULL,
[FundCashAccount] [nvarchar] (25) NULL,
[FundCashFund] [nvarchar] (25) NULL,
[OnlineSort] [int] NULL,
[NonTaxDeductible] [bit] NULL,
[QBIncomeAccount] [int] NOT NULL CONSTRAINT [DF_ContributionFund_QBIncomeAccount] DEFAULT ((0)),
[QBAssetAccount] [int] NOT NULL CONSTRAINT [DF_ContributionFund_QBAssetAccount] DEFAULT ((0))
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
