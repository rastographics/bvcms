CREATE TABLE [dbo].[FamilyCheckinLock]
(
[FamilyId] [int] NOT NULL,
[Locked] [bit] NOT NULL,
[Created] [datetime] NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
