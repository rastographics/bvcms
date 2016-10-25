CREATE TABLE [dbo].[TaskListOwners]
(
[TaskListId] [int] NOT NULL,
[PeopleId] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
