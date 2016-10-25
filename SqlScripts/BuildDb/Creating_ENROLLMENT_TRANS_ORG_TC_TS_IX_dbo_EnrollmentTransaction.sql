CREATE NONCLUSTERED INDEX [ENROLLMENT_TRANS_ORG_TC_TS_IX] ON [dbo].[EnrollmentTransaction] ([OrganizationId], [TransactionTypeId], [TransactionStatus])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
