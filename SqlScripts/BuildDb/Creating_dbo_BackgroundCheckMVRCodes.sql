CREATE TABLE [dbo].[BackgroundCheckMVRCodes]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Code] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Description] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[StateAbbr] [nvarchar] (3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_BackgroundCheckMVRCodes_StateCode] DEFAULT ('')
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
