ALTER TABLE [dbo].[CheckInActivity] ADD CONSTRAINT [FK_CheckInActivity_CheckInTimes] FOREIGN KEY ([Id]) REFERENCES [dbo].[CheckInTimes] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
