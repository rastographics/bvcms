CREATE TABLE [dbo].[MobileAppDevices]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[created] [datetime] NOT NULL CONSTRAINT [DF_MobileAppDevices_created] DEFAULT (((0)-(0))-(0)),
[lastSeen] [datetime] NOT NULL CONSTRAINT [DF_MobileAppDevices_lastSeen] DEFAULT (((0)-(0))-(0)),
[deviceTypeID] [int] NOT NULL CONSTRAINT [DF_MobileAppDevices_deviceTypeID] DEFAULT ((0)),
[instanceID] [varchar] (255) NOT NULL CONSTRAINT [DF_Table_1_instance] DEFAULT (''),
[notificationID] [varchar] (max) NOT NULL CONSTRAINT [DF_MobileAppDevices_notificationID] DEFAULT (''),
[userID] [int] NULL,
[peopleID] [int] NULL,
[authentication] [varchar] (64) NOT NULL CONSTRAINT [DF_MobileAppDevices_authentication] DEFAULT (''),
[code] [varchar] (64) NOT NULL CONSTRAINT [DF_MobileAppDevices_code] DEFAULT (''),
[codeExpires] [datetime] NOT NULL CONSTRAINT [DF_MobileAppDevices_codeExpires] DEFAULT ('1970-01-01 00:00:00'),
[codeEmail] [varchar] (255) NOT NULL CONSTRAINT [DF_MobileAppDevices_codeEmail] DEFAULT ('')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
