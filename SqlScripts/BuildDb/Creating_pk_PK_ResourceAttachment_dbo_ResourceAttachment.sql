ALTER TABLE [dbo].[ResourceAttachment] ADD CONSTRAINT [PK_ResourceAttachment] PRIMARY KEY CLUSTERED  ([ResourceAttachmentId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
