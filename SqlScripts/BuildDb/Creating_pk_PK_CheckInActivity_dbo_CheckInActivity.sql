ALTER TABLE [dbo].[CheckInActivity] ADD CONSTRAINT [PK_CheckInActivity] PRIMARY KEY CLUSTERED  ([Id], [Activity])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
