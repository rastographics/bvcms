ALTER TABLE [dbo].[TaskListOwners] WITH NOCHECK  ADD CONSTRAINT [FK_TaskListOwners_TaskList] FOREIGN KEY ([TaskListId]) REFERENCES [dbo].[TaskList] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
