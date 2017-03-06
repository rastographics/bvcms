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
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
