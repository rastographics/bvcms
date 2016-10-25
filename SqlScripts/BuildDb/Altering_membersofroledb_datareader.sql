EXEC sp_addrolemember N'db_datareader', N'ro-CMS_StarterDb'
GO
EXEC sp_addrolemember N'db_datareader', N'ro-CMS_StarterDb-finance'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
