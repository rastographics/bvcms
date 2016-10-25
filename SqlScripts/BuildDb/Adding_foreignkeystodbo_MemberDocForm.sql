ALTER TABLE [dbo].[MemberDocForm] ADD CONSTRAINT [FK_MemberDocForm_PEOPLE_TBL] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
