CREATE TABLE [dbo].[ContributionFund]
(
[FundId] [int] NOT NULL,
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[FundName] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[FundDescription] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FundStatusId] [int] NOT NULL,
[FundTypeId] [int] NOT NULL,
[FundPledgeFlag] [bit] NOT NULL,
[FundAccountCode] [int] NULL,
[FundIncomeDept] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FundIncomeAccount] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FundIncomeFund] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FundCashDept] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FundCashAccount] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FundCashFund] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[OnlineSort] [int] NULL,
[NonTaxDeductible] [bit] NULL,
[QBIncomeAccount] [int] NOT NULL CONSTRAINT [DF_ContributionFund_QBIncomeAccount] DEFAULT ((0)),
[QBAssetAccount] [int] NOT NULL CONSTRAINT [DF_ContributionFund_QBAssetAccount] DEFAULT ((0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
