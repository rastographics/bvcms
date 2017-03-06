ALTER TABLE [dbo].[Meetings] ADD CONSTRAINT [MeetingDateOrgId] UNIQUE NONCLUSTERED  ([MeetingDate], [OrganizationId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
