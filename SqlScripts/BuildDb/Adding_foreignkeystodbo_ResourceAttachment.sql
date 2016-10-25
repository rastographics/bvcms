ALTER TABLE [dbo].[ResourceAttachment] ADD CONSTRAINT [FK_ResourceAttachment_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
