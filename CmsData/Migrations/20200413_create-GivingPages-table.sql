IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'GivingPages' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN	
		    CREATE TABLE [dbo].[GivingPages](
			[GivingPageId] [INT] PRIMARY KEY IDENTITY (1,1),
			[PageName] [NVARCHAR](max) NOT NULL,
			[PageTitle] [NVARCHAR](max) NOT NULL,
			[PageType] [INT] NOT NULL,
			[FundId] [INT] NOT NULL CONSTRAINT FK_GivingPages_ContributionFund FOREIGN KEY REFERENCES ContributionFund(FundId),
            [Enabled] [bit] NOT NULL,
            [DisabledReDirect] [NVARCHAR](max) NOT NULL,
            [SkinFile] [NVARCHAR](max),
            [TopText] [NVARCHAR](max),
            [ThankYouText] [NVARCHAR](max),
            [ConfirmationEmail_Pledge] [NVARCHAR](max),
            [ConfirmationEmail_OneTime] [NVARCHAR](max),
            [ConfirmationEmail_Recurring] [NVARCHAR](max),

			[CampusId] [INT] CONSTRAINT FK_GivingPages_Campus FOREIGN KEY REFERENCES [lookup].[Campus](Id),
            [EntryPointId] [INT] CONSTRAINT FK_GivingPages_EntryPoint FOREIGN KEY REFERENCES [lookup].[EntryPoint](Id)
			)

            CREATE TABLE [dbo].[GivingPageFunds](
			[GivingPageFundId] [INT] PRIMARY KEY IDENTITY (1,1),
			[GivingPageId] [INT] NOT NULL CONSTRAINT FK_GivingPageFunds_GivingPages FOREIGN KEY REFERENCES [dbo].[GivingPages](GivingPageId),
            [FundId] [INT] NOT NULL CONSTRAINT FK_GivingPageFunds_ContributionFund FOREIGN KEY REFERENCES [dbo].[ContributionFund](FundId)
			)
	END
GO
