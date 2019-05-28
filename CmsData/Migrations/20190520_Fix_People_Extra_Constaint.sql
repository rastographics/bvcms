BEGIN TRAN FixConstraint
	BEGIN TRY  
		ALTER TABLE [PeopleExtra]
		DROP CONSTRAINT [PK_PeopleExtra_1]

		ALTER TABLE [PeopleExtra]
		ADD  CONSTRAINT [PK_PeopleExtra_1] PRIMARY KEY CLUSTERED 
		(
			[PeopleId] ASC,
			[TransactionTime] ASC,
			[Field] ASC,
			[Instance] ASC
		)
		COMMIT
	END TRY  
	BEGIN CATCH  
		ROLLBACK
		SELECT   
			ERROR_NUMBER() AS ErrorNumber  
		   ,ERROR_MESSAGE() AS ErrorMessage;  
	END CATCH
GO