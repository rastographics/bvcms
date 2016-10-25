ALTER TABLE [dbo].[ProgDiv] ADD CONSTRAINT [FK_ProgDiv_Division] FOREIGN KEY ([DivId]) REFERENCES [dbo].[Division] ([Id])
GO
ALTER TABLE [dbo].[ProgDiv] ADD CONSTRAINT [FK_ProgDiv_Program] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Program] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
