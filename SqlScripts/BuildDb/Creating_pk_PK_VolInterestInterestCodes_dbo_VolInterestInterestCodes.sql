ALTER TABLE [dbo].[VolInterestInterestCodes] ADD CONSTRAINT [PK_VolInterestInterestCodes] PRIMARY KEY CLUSTERED  ([PeopleId], [InterestCodeId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
