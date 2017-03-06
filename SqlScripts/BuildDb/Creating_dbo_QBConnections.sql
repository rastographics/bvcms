CREATE TABLE [dbo].[QBConnections]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Creation] [datetime] NOT NULL CONSTRAINT [DF_QBConnections_Creation] DEFAULT ((0)),
[UserID] [int] NOT NULL CONSTRAINT [DF_QBConnections_UserID] DEFAULT ((0)),
[Active] [tinyint] NOT NULL CONSTRAINT [DF_QBConnections_Active] DEFAULT ((0)),
[DataSource] [char] (3) NOT NULL CONSTRAINT [DF_QBConnections_DataSource] DEFAULT (''),
[Token] [nvarchar] (max) NOT NULL CONSTRAINT [DF_QBConnections_Token] DEFAULT (''),
[Secret] [nvarchar] (max) NOT NULL CONSTRAINT [DF_QBConnections_Secret] DEFAULT (''),
[RealmID] [nvarchar] (max) NOT NULL CONSTRAINT [DF_QBConnections_RealmID] DEFAULT ('')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
