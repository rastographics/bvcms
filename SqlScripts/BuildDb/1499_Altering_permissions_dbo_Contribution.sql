DENY SELECT ON  [dbo].[Contribution] TO [ro]
DENY INSERT ON  [dbo].[Contribution] TO [ro]
DENY DELETE ON  [dbo].[Contribution] TO [ro]
DENY UPDATE ON  [dbo].[Contribution] TO [ro]
GO
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO
