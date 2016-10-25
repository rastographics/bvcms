CREATE TABLE [dbo].[SMSItems]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[ListID] [int] NOT NULL CONSTRAINT [DF_SMSItems_siListID] DEFAULT ((0)),
[PeopleID] [int] NOT NULL CONSTRAINT [DF_SMSItems_siSentToID] DEFAULT ((0)),
[Sent] [bit] NOT NULL CONSTRAINT [DF_SMSItems_Sent] DEFAULT ((0)),
[Number] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_SMSItems_SendAddress] DEFAULT (''),
[NoNumber] [bit] NOT NULL CONSTRAINT [DF_SMSItems_NoNumber] DEFAULT ((0)),
[NoOptIn] [bit] NOT NULL CONSTRAINT [DF_SMSItems_NoOptIn] DEFAULT ((0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
