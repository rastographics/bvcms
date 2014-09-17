ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Families] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([FamilyId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DropType] FOREIGN KEY ([DropCodeId]) REFERENCES [lookup].[DropType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Gender] FOREIGN KEY ([GenderId]) REFERENCES [lookup].[Gender] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MaritalStatus] FOREIGN KEY ([MaritalStatusId]) REFERENCES [lookup].[MaritalStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_FamilyPosition] FOREIGN KEY ([PositionInFamilyId]) REFERENCES [lookup].[FamilyPosition] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MemberStatus] FOREIGN KEY ([MemberStatusId]) REFERENCES [lookup].[MemberStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Origin] FOREIGN KEY ([OriginId]) REFERENCES [lookup].[Origin] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_EntryPoint] FOREIGN KEY ([EntryPointId]) REFERENCES [lookup].[EntryPoint] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_InterestPoint] FOREIGN KEY ([InterestPointId]) REFERENCES [lookup].[InterestPoint] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_BaptismType] FOREIGN KEY ([BaptismTypeId]) REFERENCES [lookup].[BaptismType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_BaptismStatus] FOREIGN KEY ([BaptismStatusId]) REFERENCES [lookup].[BaptismStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DecisionType] FOREIGN KEY ([DecisionTypeId]) REFERENCES [lookup].[DecisionType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_DiscoveryClassStatus] FOREIGN KEY ([NewMemberClassStatusId]) REFERENCES [lookup].[NewMemberClassStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_MemberLetterStatus] FOREIGN KEY ([LetterStatusId]) REFERENCES [lookup].[MemberLetterStatus] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_JoinType] FOREIGN KEY ([JoinCodeId]) REFERENCES [lookup].[JoinType] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [EnvPeople__EnvelopeOption] FOREIGN KEY ([EnvelopeOptionsId]) REFERENCES [lookup].[EnvelopeOption] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [ResCodePeople__ResidentCode] FOREIGN KEY ([ResCodeId]) REFERENCES [lookup].[ResidentCode] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_PEOPLE_TBL_Picture] FOREIGN KEY ([PictureId]) REFERENCES [dbo].[Picture] ([PictureId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [StmtPeople__ContributionStatementOption] FOREIGN KEY ([ContributionOptionsId]) REFERENCES [lookup].[EnvelopeOption] ([Id])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [BFMembers__BFClass] FOREIGN KEY ([BibleFellowshipClassId]) REFERENCES [dbo].[Organizations] ([OrganizationId])
ALTER TABLE [dbo].[People] ADD CONSTRAINT [FK_People_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
