CREATE TABLE [dbo].[CustomColumns]
(
[Ord] [int] NOT NULL,
[Column] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Select] [varchar] (300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[JoinTable] [varchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
