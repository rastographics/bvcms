CREATE TABLE [dbo].[MobileAppFloor]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[campus] [int] NOT NULL CONSTRAINT [DF_MobileAppFloors_campus] DEFAULT ((0)),
[name] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppFloors_name] DEFAULT (''),
[image] [nvarchar] (250) NOT NULL CONSTRAINT [DF_MobileAppFloors_image] DEFAULT (''),
[order] [int] NOT NULL CONSTRAINT [DF_MobileAppFloors_order] DEFAULT ((0)),
[enabled] [bit] NOT NULL CONSTRAINT [DF_MobileAppFloors_enabled] DEFAULT ((1))
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
