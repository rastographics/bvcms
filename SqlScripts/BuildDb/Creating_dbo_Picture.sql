CREATE TABLE [dbo].[Picture]
(
[PictureId] [int] NOT NULL IDENTITY(1, 1),
[CreatedDate] [datetime] NULL,
[CreatedBy] [nvarchar] (50) NULL,
[LargeId] [int] NULL,
[MediumId] [int] NULL,
[SmallId] [int] NULL,
[ThumbId] [int] NULL,
[X] [int] NULL,
[Y] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
