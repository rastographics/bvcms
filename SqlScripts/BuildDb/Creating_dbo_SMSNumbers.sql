CREATE TABLE [dbo].[SMSNumbers]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[GroupID] [int] NOT NULL CONSTRAINT [DF_SMSNumber_GroupID] DEFAULT ((0)),
[Number] [nvarchar] (50) NOT NULL CONSTRAINT [DF_SMSNumbers_Number] DEFAULT (''),
[LastUpdated] [datetime] NOT NULL CONSTRAINT [DF_SMSNumbers_LastUpdated] DEFAULT ((0))
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
