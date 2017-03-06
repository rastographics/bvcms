CREATE TABLE [dbo].[CheckInTimes]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NULL,
[CheckInTime] [datetime] NULL,
[GuestOfId] [int] NULL,
[location] [nvarchar] (50) NULL,
[GuestOfPersonID] [int] NOT NULL CONSTRAINT [DF_CheckInTimes_GuestOfPersonID] DEFAULT ((0)),
[AccessTypeID] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
