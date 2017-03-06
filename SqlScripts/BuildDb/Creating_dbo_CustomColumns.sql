CREATE TABLE [dbo].[CustomColumns]
(
[Ord] [int] NOT NULL,
[Column] [varchar] (50) NOT NULL,
[Select] [varchar] (300) NULL,
[JoinTable] [varchar] (200) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
