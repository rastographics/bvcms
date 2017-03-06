CREATE TABLE [dbo].[RecurringAmounts]
(
[PeopleId] [int] NOT NULL,
[FundId] [int] NOT NULL,
[Amt] [money] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
