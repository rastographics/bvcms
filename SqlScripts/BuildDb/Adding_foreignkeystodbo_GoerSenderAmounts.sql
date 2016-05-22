ALTER TABLE [dbo].[GoerSenderAmounts] ADD CONSTRAINT [FK_GoerSenderAmounts_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[GoerSenderAmounts] ADD CONSTRAINT [GoerAmounts__Goer] FOREIGN KEY ([SupporterId]) REFERENCES [dbo].[People] ([PeopleId])
ALTER TABLE [dbo].[GoerSenderAmounts] ADD CONSTRAINT [SenderAmounts__Sender] FOREIGN KEY ([GoerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
