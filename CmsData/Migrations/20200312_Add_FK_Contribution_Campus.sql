IF NOT EXISTS ( SELECT  name FROM sys.foreign_keys where name = 'FK_Contribution_Campus' ) 
BEGIN
    UPDATE dbo.Contribution SET CampusId = NULL WHERE CampusId NOT IN (SELECT Id FROM lookup.Campus);
    ALTER TABLE [dbo].[Contribution] ADD CONSTRAINT [FK_Contribution_Campus] FOREIGN KEY ([CampusId]) REFERENCES [lookup].[Campus] ([Id])
END
