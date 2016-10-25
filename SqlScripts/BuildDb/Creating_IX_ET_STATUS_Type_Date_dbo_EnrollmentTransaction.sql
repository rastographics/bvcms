CREATE NONCLUSTERED INDEX [IX_ET_STATUS_Type_Date] ON [dbo].[EnrollmentTransaction] ([TransactionStatus], [TransactionTypeId]) INCLUDE ([OrganizationId], [PeopleId], [TransactionDate])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
