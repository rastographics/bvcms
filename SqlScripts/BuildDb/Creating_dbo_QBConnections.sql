CREATE TABLE [dbo].[QBConnections]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Creation] [datetime] NOT NULL CONSTRAINT [DF_QBConnections_Creation] DEFAULT ((0)),
[UserID] [int] NOT NULL CONSTRAINT [DF_QBConnections_UserID] DEFAULT ((0)),
[Active] [tinyint] NOT NULL CONSTRAINT [DF_QBConnections_Active] DEFAULT ((0)),
[DataSource] [char] (3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_QBConnections_DataSource] DEFAULT (''),
[Token] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_QBConnections_Token] DEFAULT (''),
[Secret] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_QBConnections_Secret] DEFAULT (''),
[RealmID] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_QBConnections_RealmID] DEFAULT ('')
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
