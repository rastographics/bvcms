ALTER TABLE [dbo].[LongRunningOp] ADD CONSTRAINT [PK_LongRunningOp] PRIMARY KEY CLUSTERED  ([id], [operation])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
