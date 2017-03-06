CREATE TABLE [dbo].[BackgroundCheckMVRCodes]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Code] [nvarchar] (10) NOT NULL,
[Description] [nvarchar] (100) NOT NULL,
[StateAbbr] [nvarchar] (3) NOT NULL CONSTRAINT [DF_BackgroundCheckMVRCodes_StateCode] DEFAULT ('')
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
