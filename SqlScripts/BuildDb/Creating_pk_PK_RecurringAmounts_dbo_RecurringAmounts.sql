ALTER TABLE [dbo].[RecurringAmounts] ADD CONSTRAINT [PK_RecurringAmounts] PRIMARY KEY CLUSTERED  ([PeopleId], [FundId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
