CREATE TABLE [dbo].[CheckInTimes]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NULL,
[CheckInTime] [datetime] NULL,
[GuestOfId] [int] NULL,
[location] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[GuestOfPersonID] [int] NOT NULL CONSTRAINT [DF_CheckInTimes_GuestOfPersonID] DEFAULT ((0)),
[AccessTypeID] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
