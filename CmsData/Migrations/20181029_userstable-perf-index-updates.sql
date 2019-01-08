IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Users_Username' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Username]
    ON [dbo].[Users] ([Username])
END