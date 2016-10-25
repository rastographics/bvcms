ALTER TABLE [dbo].[GoerSenderAmounts] ADD CONSTRAINT [FK_GoerSenderAmounts_Organizations] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[GoerSenderAmounts] ADD CONSTRAINT [GoerAmounts__Goer] FOREIGN KEY ([SupporterId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[GoerSenderAmounts] ADD CONSTRAINT [SenderAmounts__Sender] FOREIGN KEY ([GoerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
