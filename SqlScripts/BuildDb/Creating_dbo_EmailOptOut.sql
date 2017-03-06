CREATE TABLE [dbo].[EmailOptOut]
(
[ToPeopleId] [int] NOT NULL,
[FromEmail] [nvarchar] (50) NOT NULL,
[Date] [datetime] NULL CONSTRAINT [DF_EmailOptOut_Date] DEFAULT (getdate())
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
