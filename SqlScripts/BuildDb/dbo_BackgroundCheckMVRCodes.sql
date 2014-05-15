CREATE TABLE [dbo].[BackgroundCheckMVRCodes]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Code] [nvarchar] (10) NOT NULL,
[Description] [nvarchar] (100) NOT NULL,
[StateAbbr] [nvarchar] (3) NOT NULL CONSTRAINT [DF_BackgroundCheckMVRCodes_StateCode] DEFAULT ('')
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
