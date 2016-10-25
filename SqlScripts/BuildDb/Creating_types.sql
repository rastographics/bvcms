IF @@ERROR <> 0 SET NOEXEC ON
GO
CREATE TYPE [dbo].[DonorTotalsTable] AS TABLE
(
[tot] [money] NULL,
[cnt] [int] NULL,
[attr] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
