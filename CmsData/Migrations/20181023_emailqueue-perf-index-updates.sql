IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_EmailQueueTo_PeopleId_Id' AND object_id = OBJECT_ID('dbo.EmailQueueTo'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_EmailQueueTo_PeopleId_Id]
    ON [dbo].[EmailQueueTo] ([PeopleId])
    INCLUDE ([Id])
END