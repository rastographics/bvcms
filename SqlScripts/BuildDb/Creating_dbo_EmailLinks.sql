CREATE TABLE [dbo].[EmailLinks]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Created] [datetime] NULL,
[EmailID] [int] NULL,
[Hash] [nvarchar] (50) NULL,
[Link] [nvarchar] (2000) NULL,
[Count] [int] NOT NULL CONSTRAINT [DF_EmailLinks_Count] DEFAULT ((0))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
