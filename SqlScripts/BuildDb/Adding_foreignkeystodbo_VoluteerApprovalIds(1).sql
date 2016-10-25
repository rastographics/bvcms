ALTER TABLE [dbo].[VoluteerApprovalIds] ADD CONSTRAINT [FK_VoluteerApprovalIds_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[VoluteerApprovalIds] ADD CONSTRAINT [FK_VoluteerApprovalIds_Volunteer] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[Volunteer] ([PeopleId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
