CREATE TABLE [dbo].[QueryStats]
(
[RunId] [int] NOT NULL,
[StatId] [nvarchar] (5) NOT NULL,
[Runtime] [datetime] NOT NULL,
[Description] [nvarchar] (75) NOT NULL,
[Count] [int] NOT NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
