ALTER TABLE [dbo].[VoluteerApprovalIds] WITH NOCHECK  ADD CONSTRAINT [FK_VoluteerApprovalIds_VolunteerCodes] FOREIGN KEY ([ApprovalId]) REFERENCES [lookup].[VolunteerCodes] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
