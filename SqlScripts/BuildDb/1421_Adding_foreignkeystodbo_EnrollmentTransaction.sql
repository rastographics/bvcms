ALTER TABLE [dbo].[EnrollmentTransaction] ADD CONSTRAINT [DescTransactions__FirstTransaction] FOREIGN KEY ([EnrollmentTransactionId]) REFERENCES [dbo].[EnrollmentTransaction] ([TransactionId])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
