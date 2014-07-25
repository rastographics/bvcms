CREATE NONCLUSTERED INDEX [IX_ET_STATUS_Type_Date] ON [dbo].[EnrollmentTransaction] ([TransactionStatus], [TransactionTypeId]) INCLUDE ([OrganizationId], [PeopleId], [TransactionDate])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
