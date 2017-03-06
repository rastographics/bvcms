ALTER TABLE [lookup].[BackgroundCheckLabels] ADD CONSTRAINT [PK_BackgroundCheckLabels] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
