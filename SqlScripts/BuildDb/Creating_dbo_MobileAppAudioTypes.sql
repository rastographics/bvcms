CREATE TABLE [dbo].[MobileAppAudioTypes]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppAudioTypes_name] DEFAULT ('')
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
