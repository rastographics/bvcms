IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = 'ContributionTag')
BEGIN
    CREATE TABLE [dbo].[ContributionTag]
    (
    [ContributionId] [int] NOT NULL,
    [TagName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [Priority] [int] NULL
    )
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.objects WHERE type = 'PK' AND  parent_object_id = OBJECT_ID ('ContributionTag'))
BEGIN
    ALTER TABLE [dbo].[ContributionTag] ADD CONSTRAINT [PK_ContributionTag] PRIMARY KEY CLUSTERED  ([ContributionId], [TagName])
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ContributionTagName' AND object_id = OBJECT_ID('ContributionTag'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ContributionTagName] ON [dbo].[ContributionTag] ([TagName])
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.object_id = object_id(N'[dbo].[FK_ContributionTag_Contribution]') AND OBJECTPROPERTY(o.object_id, N'IsForeignKey') = 1)
BEGIN
    ALTER TABLE [dbo].[ContributionTag] ADD CONSTRAINT [FK_ContributionTag_Contribution] FOREIGN KEY ([ContributionId]) REFERENCES [dbo].[Contribution] ([ContributionId])
END
GO
