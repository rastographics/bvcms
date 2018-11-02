CREATE TABLE [dbo].[ContributionTag]
(
[ContributionId] [int] NOT NULL,
[TagName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Priority] [int] NULL
)
GO
ALTER TABLE [dbo].[ContributionTag] ADD CONSTRAINT [PK_ContributionTag] PRIMARY KEY CLUSTERED  ([ContributionId], [TagName])
GO
CREATE NONCLUSTERED INDEX [IX_ContributionTagName] ON [dbo].[ContributionTag] ([TagName])
GO
ALTER TABLE [dbo].[ContributionTag] ADD CONSTRAINT [FK_ContributionTag_Contribution] FOREIGN KEY ([ContributionId]) REFERENCES [dbo].[Contribution] ([ContributionId])
GO
