ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [FK_UserRole_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [FK_UserRole_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
