-- add status flags widget
IF (select count(*) from DashboardWidgets where [Name] like 'Status Flags' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
       [Body],
       [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetStatusFlagsHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5>{{WidgetName}}</h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5><i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}</h5>
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        {{#ifGT results.Count 0}}
            <ul class="list-group">
                {{#each results}}
                    {{#ifEqual name "Total"}}
                        <li class="list-group-item" title="{{flag}}"><strong>{{name}}</strong><span style="float:right;font-weight:bold;">{{count}}</span></li>
                    {{else}}
                        <li class="list-group-item" title="{{flag}}">{{name}}<span style="float:right;">{{count}}</span></li>
                    {{/ifEqual}}
                {{/each}}
            </ul>
        {{else}}
            <div class="box-content"></div>
        {{/ifGT}}
    </div>
</div>',
           GETDATE(),1,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')

INSERT INTO [dbo].[Content]
           ([Name],[Title],
       [Body],
       [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetStatusFlagsPython','Edit Python Script',
           '# You can report summary numbers for any status flag you have set up with this widget
# Example data is below, to use put the name of each status flag you want to report on, followed by the code on each line
# You can also use multiple codes in a line by separating them by a comma - this will show the number of people that have ALL of the specified flags

statusFlags = [
    ( "Community Group", "F8" ),
    ( "Leaders", "F16" ),
    ( "Volunteers", "F5,F20" ),
    ( "New Visitors", "F31" ),
    ( "Total", "F20" ),
]

def Get():
    results = []
    template = Data.HTMLContent
    
    for item in statusFlags:
        flag = model.DynamicData()
        flag.name = item[0]
        flag.flag = item[1]
        flag.count = q.StatusCount(item[1])
        results.append(flag)
    
    Data.results = results
    print model.RenderTemplate(template)
Get()',
           GETDATE(),5,0,0,0,'admin')
          
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')
           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Status Flags' and [System] = 1) = 0
BEGIN
INSERT INTO [dbo].[DashboardWidgets]
           ([Name]
           ,[Description]
           ,[HTMLContentId]
           ,[PythonContentId]
           ,[SQLContentId]
           ,[Enabled]
           ,[Order]
           ,[System]
           ,[CachePolicy]
           ,[CacheHours])
     VALUES
           ('Status Flags'
           ,'Displays vital stats about your church'
           ,(select max(Id) from Content where [Name] like 'WidgetStatusFlagsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetStatusFlagsPython')
           ,NULL
           ,0
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,1
           ,12)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
    SELECT SCOPE_IDENTITY() [WidgetId], RoleId FROM dbo.Roles WHERE RoleName in ('Edit')
END
GO