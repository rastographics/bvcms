ALTER TABLE [dbo].[Division] ADD CONSTRAINT [FK_Division_Program] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Program] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
