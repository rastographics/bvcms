CREATE TABLE [dbo].[ApiSession]
(
[UserId] [int] NOT NULL,
[SessionToken] [uniqueidentifier] NOT NULL,
[PIN] [int] NULL,
[LastAccessedDate] [datetime] NOT NULL,
[CreatedDate] [datetime] NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
