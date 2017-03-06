CREATE TABLE [dbo].[VolInterestCodes]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Description] [nvarchar] (180) NULL,
[Code] [nvarchar] (100) NULL,
[Org] [nvarchar] (150) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
