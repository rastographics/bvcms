CREATE TABLE [dbo].[MobileAppVideoTypes]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppVideoTypes_name] DEFAULT ('')
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
