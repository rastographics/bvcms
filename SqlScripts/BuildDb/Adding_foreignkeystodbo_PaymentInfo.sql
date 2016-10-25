ALTER TABLE [dbo].[PaymentInfo] ADD CONSTRAINT [FK_PaymentInfo_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
