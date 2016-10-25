ALTER TABLE [dbo].[ResourceAttachment] ADD CONSTRAINT [PK_ResourceAttachment] PRIMARY KEY CLUSTERED  ([ResourceAttachmentId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
