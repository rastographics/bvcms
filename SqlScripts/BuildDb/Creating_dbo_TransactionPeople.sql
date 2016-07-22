CREATE TABLE [dbo].[TransactionPeople]
(
[Id] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[Amt] [money] NULL,
[OrgId] [int] NULL,
[Donor] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
