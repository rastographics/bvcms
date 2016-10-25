ALTER TABLE [dbo].[Attend] WITH NOCHECK  ADD CONSTRAINT [FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[Attend] WITH NOCHECK  ADD CONSTRAINT [FK_AttendWithAbsents_TBL_AttendType] FOREIGN KEY ([AttendanceTypeId]) REFERENCES [lookup].[AttendType] ([Id])
GO
ALTER TABLE [dbo].[Attend] WITH NOCHECK  ADD CONSTRAINT [FK_Attend_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
