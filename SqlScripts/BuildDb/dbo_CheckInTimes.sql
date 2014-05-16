CREATE TABLE [dbo].[CheckInTimes]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NULL,
[CheckInTime] [datetime] NULL,
[GuestOfId] [int] NULL,
[location] [nvarchar] (50) NULL,
[GuestOfPersonID] [int] NOT NULL CONSTRAINT [DF_CheckInTimes_GuestOfPersonID] DEFAULT ((0)),
[AccessTypeID] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
