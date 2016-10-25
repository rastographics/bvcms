CREATE TABLE [dbo].[EmailOptOut]
(
[ToPeopleId] [int] NOT NULL,
[FromEmail] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Date] [datetime] NULL CONSTRAINT [DF_EmailOptOut_Date] DEFAULT (getdate())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
