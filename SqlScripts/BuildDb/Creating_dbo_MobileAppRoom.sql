CREATE TABLE [dbo].[MobileAppRoom]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[floor] [int] NOT NULL CONSTRAINT [DF_MobileAppRooms_floor] DEFAULT ((0)),
[name] [nvarchar] (100) NOT NULL CONSTRAINT [DF_MobileAppRooms_name] DEFAULT (''),
[room] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppRooms_room] DEFAULT (''),
[x] [int] NOT NULL CONSTRAINT [DF_MobileAppRooms_x] DEFAULT ((0)),
[y] [int] NOT NULL CONSTRAINT [DF_MobileAppRooms_y] DEFAULT ((0)),
[enabled] [bit] NOT NULL CONSTRAINT [DF_MobileAppRooms_enabled] DEFAULT ((1))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
