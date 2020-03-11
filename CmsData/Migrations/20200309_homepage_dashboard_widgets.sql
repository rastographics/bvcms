IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = 'DashboardWidgets')
BEGIN
    CREATE TABLE [dbo].[DashboardWidgets](
        Id INT IDENTITY NOT NULL PRIMARY KEY,
        Name NVARCHAR(140) NOT NULL,
        Description VARCHAR(400),
        HTMLContent INT FOREIGN KEY REFERENCES [dbo].[Content]([Id]),
        PythonContent INT FOREIGN KEY REFERENCES [dbo].[Content]([Id]),
        SQLContent INT FOREIGN KEY REFERENCES [dbo].[Content]([Id]),
        Enabled BIT NOT NULL DEFAULT 1,
        [Order] INT NOT NULL DEFAULT 0,
        System BIT NOT NULL DEFAULT 0
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = 'DashboardWidgetRoles')
BEGIN
    CREATE TABLE [dbo].[DashboardWidgetRoles](
        WidgetId INT FOREIGN KEY REFERENCES [dbo].[DashboardWidgets]([Id]) NOT NULL,
        RoleId INT FOREIGN KEY REFERENCES [dbo].[Roles]([RoleId]) NOT NULL
    )
END
GO
