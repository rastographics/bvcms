CREATE TABLE [dbo].[EmailLinks]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Created] [datetime] NULL,
[EmailID] [int] NULL,
[Hash] [nvarchar] (50) NULL,
[Link] [nvarchar] (2000) NULL,
[Count] [int] NOT NULL CONSTRAINT [DF_EmailLinks_Count] DEFAULT ((0))
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
