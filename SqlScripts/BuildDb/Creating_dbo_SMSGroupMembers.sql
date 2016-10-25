CREATE TABLE [dbo].[SMSGroupMembers]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[GroupID] [int] NOT NULL,
[UserID] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
