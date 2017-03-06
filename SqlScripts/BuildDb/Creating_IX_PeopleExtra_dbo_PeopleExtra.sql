CREATE NONCLUSTERED INDEX [IX_PeopleExtra] ON [dbo].[PeopleExtra] ([Field], [StrValue]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
