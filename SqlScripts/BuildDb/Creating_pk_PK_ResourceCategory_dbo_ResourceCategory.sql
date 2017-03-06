ALTER TABLE [dbo].[ResourceCategory] ADD CONSTRAINT [PK_ResourceCategory] PRIMARY KEY CLUSTERED  ([ResourceCategoryId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
