ALTER TABLE [dbo].[CheckInTimes] ADD CONSTRAINT [Guests__GuestOf] FOREIGN KEY ([GuestOfId]) REFERENCES [dbo].[CheckInTimes] ([Id])
GO
ALTER TABLE [dbo].[CheckInTimes] ADD CONSTRAINT [FK_CheckInTimes_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
