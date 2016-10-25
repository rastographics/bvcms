ALTER TABLE [dbo].[Volunteer] WITH NOCHECK  ADD CONSTRAINT [FK_Volunteer_VolApplicationStatus] FOREIGN KEY ([StatusId]) REFERENCES [lookup].[VolApplicationStatus] ([Id])
GO
ALTER TABLE [dbo].[Volunteer] WITH NOCHECK  ADD CONSTRAINT [StatusMvrId__StatusMvr] FOREIGN KEY ([MVRStatusId]) REFERENCES [lookup].[VolApplicationStatus] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
