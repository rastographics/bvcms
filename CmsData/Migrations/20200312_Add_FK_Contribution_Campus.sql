IF NOT EXISTS ( SELECT  name FROM sys.foreign_keys where name = 'FK_Contribution_Campus' ) 
    alter TABLE [dbo].[Contribution] add constraint [FK_Contribution_Campus] foreign key ([CampusId]) references [lookup].[Campus] ([Id])
GO
