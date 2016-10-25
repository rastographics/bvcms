ALTER TABLE [dbo].[EmailQueue] ADD CONSTRAINT [FK_EmailQueue_People] FOREIGN KEY ([QueuedBy]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
