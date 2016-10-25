CREATE NONCLUSTERED INDEX [IX_Date_Type] ON [dbo].[EnrollmentTransaction] ([TransactionDate], [TransactionTypeId]) INCLUDE ([OrganizationId], [PeopleId], [TransactionId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
