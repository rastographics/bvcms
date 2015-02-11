CREATE TABLE [dbo].[MobileAppFloor]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[campus] [int] NOT NULL CONSTRAINT [DF_MobileAppFloors_campus] DEFAULT ((0)),
[name] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppFloors_name] DEFAULT (''),
[image] [nvarchar] (250) NOT NULL CONSTRAINT [DF_MobileAppFloors_image] DEFAULT (''),
[order] [int] NOT NULL CONSTRAINT [DF_MobileAppFloors_order] DEFAULT ((0)),
[enabled] [bit] NOT NULL CONSTRAINT [DF_MobileAppFloors_enabled] DEFAULT ((1))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
