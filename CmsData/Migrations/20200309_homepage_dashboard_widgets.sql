IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = 'DashboardWidgets')
BEGIN
    CREATE TABLE [dbo].[DashboardWidgets](
        Id INT IDENTITY NOT NULL PRIMARY KEY,
        Name NVARCHAR(140) NOT NULL,
        Description VARCHAR(400),
        HTMLContentId INT CONSTRAINT [FK_Dashboard_HTMLContent] FOREIGN KEY REFERENCES [dbo].[Content]([Id]),
        PythonContentId INT CONSTRAINT [FK_Dashboard_PythonContent] FOREIGN KEY REFERENCES [dbo].[Content]([Id]),
        SQLContentId INT CONSTRAINT [FK_Dashboard_SQLContent] FOREIGN KEY REFERENCES [dbo].[Content]([Id]),
        Enabled BIT NOT NULL DEFAULT 1,
        [Order] INT NOT NULL DEFAULT 0,
        System BIT NOT NULL DEFAULT 0,
        CachePolicy INT NOT NULL DEFAULT 0,
        CacheHours INT NOT NULL DEFAULT 0,
        CacheExpires DATETIME DEFAULT NULL,
        CachedContent NVARCHAR(MAX) DEFAULT NULL
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = 'DashboardWidgetRoles')
BEGIN
    CREATE TABLE [dbo].[DashboardWidgetRoles](
        WidgetId INT NOT NULL,
        RoleId INT NOT NULL,
        CONSTRAINT [PK_WidgetRole] PRIMARY KEY CLUSTERED 
        (
	        [WidgetId] ASC,
	        [RoleId] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
    )

    ALTER TABLE [dbo].[DashboardWidgetRoles]  WITH CHECK ADD  CONSTRAINT [FK_WigetRole_Widgets] FOREIGN KEY([WidgetId])
    REFERENCES [dbo].[DashboardWidgets]([Id])

    ALTER TABLE [dbo].[DashboardWidgetRoles]  WITH CHECK ADD  CONSTRAINT [FK_WidgetRole_Roles] FOREIGN KEY([RoleId])
    REFERENCES [dbo].[Roles]([RoleId])
END
GO
