CREATE TABLE [dbo].[GeoCodes]
(
[Address] [nvarchar] (80) NOT NULL,
[Latitude] [float] NOT NULL CONSTRAINT [DF_GeoCodes_Latitude] DEFAULT ((0)),
[Longitude] [float] NOT NULL CONSTRAINT [DF_GeoCodes_Longitude] DEFAULT ((0))
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
