CREATE NONCLUSTERED INDEX [RecRegPeopleId] ON [dbo].[RecReg] ([PeopleId]) INCLUDE ([ActiveInAnotherChurch], [emcontact], [emphone], [fname], [MedicalDescription], [mname]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
