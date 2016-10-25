ALTER TABLE [lookup].[MemberType] ADD CONSTRAINT [FK_MemberType_AttendType] FOREIGN KEY ([AttendanceTypeId]) REFERENCES [lookup].[AttendType] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
