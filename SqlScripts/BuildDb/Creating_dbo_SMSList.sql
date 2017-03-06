CREATE TABLE [dbo].[SMSList]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Created] [datetime] NOT NULL CONSTRAINT [DF_SMSList1_slCreated] DEFAULT ((0)),
[SenderID] [int] NOT NULL CONSTRAINT [DF_SMSList1_slSenderID] DEFAULT ((0)),
[SendAt] [datetime] NOT NULL CONSTRAINT [DF_SMSList1_slSendAt] DEFAULT ((0)),
[SendGroupID] [int] NOT NULL CONSTRAINT [DF_SMSList_SendGroupID] DEFAULT ((0)),
[Message] [nvarchar] (160) NOT NULL CONSTRAINT [DF_SMSList1_slMessage] DEFAULT (''),
[SentSMS] [int] NOT NULL CONSTRAINT [DF_SMSList1_slSentSMS] DEFAULT ((0)),
[SentNone] [int] NOT NULL CONSTRAINT [DF_SMSList1_slSentNone] DEFAULT ((0)),
[Title] [nvarchar] (150) NOT NULL CONSTRAINT [DF_SMSList_Title] DEFAULT ('')
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
