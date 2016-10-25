ALTER TABLE [dbo].[GoerSupporter] ADD CONSTRAINT [FK_Supporters__Goer] FOREIGN KEY ([GoerId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[GoerSupporter] ADD CONSTRAINT [FK_Goers__Supporter] FOREIGN KEY ([SupporterId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
