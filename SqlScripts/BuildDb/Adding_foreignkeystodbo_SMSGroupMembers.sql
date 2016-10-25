ALTER TABLE [dbo].[SMSGroupMembers] ADD CONSTRAINT [FK_SMSGroupMembers_SMSGroups] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[SMSGroups] ([ID])
GO
ALTER TABLE [dbo].[SMSGroupMembers] ADD CONSTRAINT [FK_SMSGroupMembers_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
