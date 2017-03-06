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
[ContributionDesc] [nvarchar] (256) NULL,
[ContributionStatusId] [int] NULL,
[PledgeFlag] [bit] NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[PostingDate] [datetime] NULL,
[BankAccount] [nvarchar] (250) NULL,
[ExtraDataId] [int] NULL,
[CheckNo] [nvarchar] (20) NULL,
[QBSyncID] [int] NULL,
[TranId] [int] NULL,
[Source] [int] NULL,
[CampusId] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
