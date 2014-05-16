CREATE TABLE [dbo].[GoerSenderAmounts]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[OrgId] [int] NOT NULL,
[SupporterId] [int] NOT NULL,
[GoerId] [int] NULL,
[Amount] [money] NULL,
[Created] [datetime] NULL,
[InActive] [bit] NULL,
[NoNoticeToGoer] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
