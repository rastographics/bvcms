ALTER TABLE [dbo].[Meetings] WITH NOCHECK  ADD CONSTRAINT [FK_MEETINGS_TBL_ORGANIZATIONS_TBL] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[Meetings] WITH NOCHECK  ADD CONSTRAINT [FK_Meetings_AttendCredit] FOREIGN KEY ([AttendCreditId]) REFERENCES [lookup].[AttendCredit] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
