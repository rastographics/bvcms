CREATE TABLE [dbo].[Picture]
(
[PictureId] [int] NOT NULL IDENTITY(1, 1),
[CreatedDate] [datetime] NULL,
[CreatedBy] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LargeId] [int] NULL,
[MediumId] [int] NULL,
[SmallId] [int] NULL,
[ThumbId] [int] NULL,
[X] [int] NULL,
[Y] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
