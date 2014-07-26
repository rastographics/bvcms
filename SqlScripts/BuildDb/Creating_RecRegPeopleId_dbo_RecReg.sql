CREATE NONCLUSTERED INDEX [RecRegPeopleId] ON [dbo].[RecReg] ([PeopleId]) INCLUDE ([ActiveInAnotherChurch], [emcontact], [emphone], [fname], [MedicalDescription], [mname])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
