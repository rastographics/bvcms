-- add status flags widget
IF (select count(*) from DashboardWidgets where [Name] like 'Vital Stats' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
       [Body],
       [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetVitalStatsHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs" style="border:0;">
        <h5>{{WidgetName}}</h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5><i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}</h5>
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        {{#ifGT results.Count 0}}
            <ul class="list-group bordered">
                {{#each results}}
                    {{#ifEqual name "Total"}}
                        <li class="list-group-item"><strong>{{name}}</strong><a href="{{url}}" class="badge badge-primary" style="float:right;font-weight:bold;">{{count}}</a></li>
                    {{else}}
                        <li class="list-group-item"><a href="{{url}}">{{name}}</a><a href="{{url}}" style="float:right;">{{count}}</a></li>
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
           ('WidgetVitalStatsPython','Edit Python Script',
           'import re
# You can report summary numbers for any status flag or saved search you have set up with this widget
# Example data is below, to use put the name you want to appear in the widget, followed by the status flag code or saved search name on each line
# You can also use multiple status flag codes in a line by separating them by a comma - this will show the number of people that have ALL of the specified flags
# You can link the number to any url as well with a third parameter

statusFlags = [
    ( "Community Group", "F8" ),
    ( "Leaders", "F16"),
    ( "Volunteers", "Volunteers", "/Query/f9449433-f972-46b0-95ab-ce142b5d54cb"),
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
        if re.search(''^F\d+(,F\d+)*$'',item[1]) is None:
            flag.count = q.QueryCount(item[1])
        else:
            flag.count = q.StatusCount(item[1])
        if len(item) > 2:
            flag.url = item[2]
        else:
            flag.url = ''#''
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

IF (select count(*) from DashboardWidgets where [Name] like 'Vital Stats' and [System] = 1) = 0
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
           ('Vital Stats'
           ,'Reports current counts of status flags and search queries'
           ,(select max(Id) from Content where [Name] like 'WidgetVitalStatsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetVitalStatsPython')
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