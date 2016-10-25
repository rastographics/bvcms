CREATE TABLE [dbo].[EmailLinks]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Created] [datetime] NULL,
[EmailID] [int] NULL,
[Hash] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Link] [nvarchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Count] [int] NOT NULL CONSTRAINT [DF_EmailLinks_Count] DEFAULT ((0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
