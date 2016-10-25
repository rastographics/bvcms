ALTER TABLE [dbo].[EnrollmentTransaction] ADD CONSTRAINT [DescTransactions__FirstTransaction] FOREIGN KEY ([EnrollmentTransactionId]) REFERENCES [dbo].[EnrollmentTransaction] ([TransactionId])
GO
ALTER TABLE [dbo].[EnrollmentTransaction] ADD CONSTRAINT [ENROLLMENT_TRANSACTION_ORG_FK] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
GO
ALTER TABLE [dbo].[EnrollmentTransaction] ADD CONSTRAINT [ENROLLMENT_TRANSACTION_PPL_FK] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([PeopleId])
GO
ALTER TABLE [dbo].[EnrollmentTransaction] ADD CONSTRAINT [FK_ENROLLMENT_TRANSACTION_TBL_MemberType] FOREIGN KEY ([MemberTypeId]) REFERENCES [lookup].[MemberType] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
