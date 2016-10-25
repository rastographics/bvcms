CREATE TABLE [dbo].[GeoCodes]
(
[Address] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Latitude] [float] NOT NULL CONSTRAINT [DF_GeoCodes_Latitude] DEFAULT ((0)),
[Longitude] [float] NOT NULL CONSTRAINT [DF_GeoCodes_Longitude] DEFAULT ((0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
