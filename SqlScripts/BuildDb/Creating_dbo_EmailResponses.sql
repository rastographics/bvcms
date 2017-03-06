CREATE TABLE [dbo].[EmailResponses]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[EmailQueueId] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[Type] [char] (1) NOT NULL,
[Dt] [datetime] NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
