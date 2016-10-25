CREATE NONCLUSTERED INDEX [RecRegPeopleId] ON [dbo].[RecReg] ([PeopleId]) INCLUDE ([ActiveInAnotherChurch], [emcontact], [emphone], [fname], [MedicalDescription], [mname])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
