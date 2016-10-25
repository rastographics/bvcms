CREATE TABLE [dbo].[MobileAppRoom]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[floor] [int] NOT NULL CONSTRAINT [DF_MobileAppRooms_floor] DEFAULT ((0)),
[name] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_MobileAppRooms_name] DEFAULT (''),
[room] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_MobileAppRooms_room] DEFAULT (''),
[x] [int] NOT NULL CONSTRAINT [DF_MobileAppRooms_x] DEFAULT ((0)),
[y] [int] NOT NULL CONSTRAINT [DF_MobileAppRooms_y] DEFAULT ((0)),
[enabled] [bit] NOT NULL CONSTRAINT [DF_MobileAppRooms_enabled] DEFAULT ((1))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
