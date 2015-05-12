ALTER TABLE [dbo].[VoluteerApprovalIds] ADD CONSTRAINT [FK_VoluteerApprovalIds_Volunteer] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[Volunteer] ([PeopleId])
ALTER TABLE [dbo].[VoluteerApprovalIds] ADD CONSTRAINT [FK_VoluteerApprovalIds_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
