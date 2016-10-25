CREATE TABLE [dbo].[QueryStats]
(
[RunId] [int] NOT NULL,
[StatId] [nvarchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Runtime] [datetime] NOT NULL,
[Description] [nvarchar] (75) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Count] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
