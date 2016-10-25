ALTER TABLE [dbo].[AuditValues] ADD CONSTRAINT [FK_AuditValues_Audits] FOREIGN KEY ([AuditId]) REFERENCES [dbo].[Audits] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
