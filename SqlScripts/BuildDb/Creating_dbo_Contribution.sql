CREATE TABLE [dbo].[Contribution]
(
[ContributionId] [int] NOT NULL IDENTITY(1, 1),
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[FundId] [int] NOT NULL,
[ContributionTypeId] [int] NOT NULL,
[PeopleId] [int] NULL,
[ContributionDate] [datetime] NULL,
[ContributionAmount] [numeric] (11, 2) NULL,
[ContributionDesc] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ContributionStatusId] [int] NULL,
[PledgeFlag] [bit] NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[PostingDate] [datetime] NULL,
[BankAccount] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ExtraDataId] [int] NULL,
[CheckNo] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[QBSyncID] [int] NULL,
[TranId] [int] NULL,
[Source] [int] NULL,
[CampusId] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
