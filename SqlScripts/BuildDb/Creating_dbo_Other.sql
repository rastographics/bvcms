CREATE TABLE [dbo].[Other]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[created] [datetime] NOT NULL CONSTRAINT [DF_Extra_created] DEFAULT (getdate()),
[userID] [int] NOT NULL CONSTRAINT [DF_Other_userID] DEFAULT ((0)),
[first] [varbinary] (max) NULL,
[second] [varbinary] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
