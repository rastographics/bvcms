CREATE NONCLUSTERED INDEX [IX_Attend_2] ON [dbo].[Attend] ([OrganizationId], [PeopleId], [MeetingDate] DESC) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
