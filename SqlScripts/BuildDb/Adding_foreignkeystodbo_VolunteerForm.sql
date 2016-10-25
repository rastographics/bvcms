ALTER TABLE [dbo].[VolunteerForm] ADD CONSTRAINT [FK_VolunteerForm_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[VolunteerForm] ADD CONSTRAINT [VolunteerFormsUploaded__Uploader] FOREIGN KEY ([UploaderId]) REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[VolunteerForm] ADD CONSTRAINT [FK_VolunteerForm_Volunteer1] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[Volunteer] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
