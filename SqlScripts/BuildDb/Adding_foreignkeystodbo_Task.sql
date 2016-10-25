ALTER TABLE [dbo].[Task] WITH NOCHECK  ADD CONSTRAINT [CoTasks__CoTaskList] FOREIGN KEY ([CoListId]) REFERENCES [dbo].[TaskList] ([Id])
GO
ALTER TABLE [dbo].[Task] WITH NOCHECK  ADD CONSTRAINT [Tasks__TaskList] FOREIGN KEY ([ListId]) REFERENCES [dbo].[TaskList] ([Id])
GO
ALTER TABLE [dbo].[Task] WITH NOCHECK  ADD CONSTRAINT [FK_Task_TaskStatus] FOREIGN KEY ([StatusId]) REFERENCES [lookup].[TaskStatus] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
