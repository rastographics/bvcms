ALTER TABLE [dbo].[VoluteerApprovalIds] ADD CONSTRAINT [PK_VoluteerAppoval] PRIMARY KEY CLUSTERED  ([PeopleId], [ApprovalId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
