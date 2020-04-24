-- add tasks widget
IF (select count(*) from DashboardWidgets where [Name] like 'My Tasks' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetTasksHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="/TaskSearch">{{WidgetName}}</a></h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        {{#ifGT results.Count 0}}
            <ul class="list-group">
                {{#each results}}
                    <li class="list-group-item"><a href="/Task/{{TaskId}}">{{Description}}</a> (<a href="/Person2/{{PeopleId}}">{{Who}}</a>)</li>
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
           ('WidgetTasksPython','Edit Python Script',
           'def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    params = { ''pid'': Data.CurrentPerson.PeopleId }
    Data.results = q.QuerySql(sql, params)
    print model.RenderTemplate(template)

Get()',
           GETDATE(),5,0,0,0,'admin')
          
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
           ('WidgetTasksSQL','Edit Sql Script',
           'select t.WhoId, t.Id as TaskId, p.PeopleId, p.[Name] as Who, t.Description from Task t
join People p on p.PeopleId = t.WhoId
where Archive = 0
and (OwnerId = @pid or CoOwnerId = @pid)
and WhoId is not null
and StatusId != (select Id from lookup.TaskStatus where Code = ''C'')
and not (OwnerId = @pid and CoOwnerId is not null)
order by CreatedOn',
           GETDATE(),4,0,0,0,'admin')

INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'My Tasks' and [System] = 1) = 0
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
           ('My Tasks'
           ,'Shows a list of tasks for the current user'
           ,(select max(Id) from Content where [Name] like 'WidgetTasksHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetTasksPython')
           ,(select max(Id) from Content where [Name] like 'WidgetTasksSQL')
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,2
           ,1)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))
END
GO

-- add tags widget
IF (select count(*) from DashboardWidgets where [Name] like 'My Tags' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetTagsHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="/Tags">{{WidgetName}}</a></h5>
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
                    <li class="list-group-item"><a href="/Tags?tag={{Id}}%2c{{PeopleId}}!{{Name}}">{{DisplayName}}</a></li>
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
           ('WidgetTagsPython','Edit Python Script',
           'def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    params = { ''pid'': Data.CurrentPerson.PeopleId }
    Data.results = q.QuerySql(sql, params)
    print model.RenderTemplate(template)

Get()',
           GETDATE(),5,0,0,0,'admin')
           
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
           ('WidgetTagsSQL','Edit Sql Script',
           'select * from (select top 100 t.Id, t.PeopleId, t.Name, t.Name as DisplayName, 1 as [Order] from Tag t
where t.PeopleId = @pid
and t.TypeId = 1
order by Name) as set1
union all
select * from (select top 100 t.Id, t.PeopleId, t.Name, t.OwnerName + ''!'' + t.Name as DisplayName, 2 as [Order] from Tag t
join TagShare ts on t.Id = ts.TagId
where t.PeopleId != @pid
and t.TypeId = 1
and ts.PeopleId = @pid
order by Name) as set2',
           GETDATE(),4,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'My Tags' and [System] = 1) = 0
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
           ('My Tags'
           ,'Shows a list of tags for the current user'
           ,(select max(Id) from Content where [Name] like 'WidgetTagsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetTagsPython')
           ,(select max(Id) from Content where [Name] like 'WidgetTagsSQL')
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,2
           ,1)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))

END
GO

-- add news widget
IF (select count(*) from DashboardWidgets where [Name] like 'TouchPoint News' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetNewsHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="{{blogurl}}" target="_blank">{{WidgetName}}</a></h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5><i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}</h5>
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        {{#ifGT news.Count 0}}
            <ul class="list-group">
                {{#each news}}
                    <li class="list-group-item">
                        {{#ifEqual new "New"}}
                            <span class="label label-danger">New</span>
                        {{/ifEqual}}
                        <a target="_blank" href="{{link}}">{{title}}</a>
                    </li>
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
           ('WidgetNewsPython','Edit Python Script',
           'from datetime import datetime
from datetime import timedelta
import xml.etree.ElementTree as ET 

def Get():
    feedurl = ''https://www.touchpointsoftware.com/feed/''
    blogurl = ''http://blog.touchpointsoftware.com''
    highlightNew = 7 # days to show new badge on article
    
    headers = { ''content-type'': ''application/json'' }
    template = Data.HTMLContent
    response = model.RestGet(feedurl, headers)
    
    tree = ET.fromstring(response) 
  
    newsitems = list()
  
    for item in tree.findall(''./channel/item''): 
        news = {}
        for child in item: 
            if ''{'' not in child.tag: 
                news[child.tag] = child.text.encode(''utf8'') 
        
        published = datetime.strptime(news[''pubDate''][0:17], "%a, %d %b %Y")
        present = datetime.now()
        if published.date() > (present - timedelta(days=highlightNew+1)).date():
            news[''new''] = ''New''
            
        newsitems.append(model.DynamicData(news)) 
    Data.news = newsitems
    Data.blogurl = blogurl
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

IF (select count(*) from DashboardWidgets where [Name] like 'TouchPoint News' and [System] = 1) = 0
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
           ('TouchPoint News'
           ,'Displays a list of recent posts from the TouchPoint blog.'
           ,(select max(Id) from Content where [Name] like 'WidgetNewsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetNewsPython')
           ,NULL
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,1
           ,24)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))
END
GO

-- add blog widget
IF (select count(*) from DashboardWidgets where [Name] like 'Church News' and [System] = 1) = 0
BEGIN

INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetBlogPython','Edit Python Script',
           'from datetime import datetime
from datetime import timedelta
import xml.etree.ElementTree as ET 

def Get():
    feedurl = model.Setting(''ChurchFeedUrl'')
    blogurl = model.Setting(''ChurchBlogUrl'')
    highlightNew = 7 # days to show new badge on article
    
    headers = { ''content-type'': ''application/json'' }
    template = Data.HTMLContent
    response = model.RestGet(feedurl, headers)
    
    tree = ET.fromstring(response) 
  
    newsitems = list()
  
    for item in tree.findall(''./channel/item''): 
        news = {}
        for child in item: 
            if ''{'' not in child.tag: 
                news[child.tag] = child.text.encode(''utf8'') 
        
        published = datetime.strptime(news[''pubDate''][0:17], "%a, %d %b %Y")
        present = datetime.now()
        if published.date() > (present - timedelta(days=highlightNew+1)).date():
            news[''new''] = ''New''
            
        newsitems.append(model.DynamicData(news)) 
    Data.news = newsitems
    Data.blogurl = blogurl
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

IF (select count(*) from DashboardWidgets where [Name] like 'Church News' and [System] = 1) = 0
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
           ('Church News'
           ,'Displays a list of recent posts from your church blog. Configure your feed and blog URLs in Settings > System > Church Info'
           ,(select max(Id) from Content where [Name] like 'WidgetNewsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetBlogPython')
           ,NULL
           ,0
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,1
           ,24)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))
END
GO

-- add searches widget
IF (select count(*) from DashboardWidgets where [Name] like 'My Searches' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetSearchesHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="/SavedQueryList">{{WidgetName}}</a></h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        {{#ifGT results.Count 0}}
            <ul class="list-group">
                {{#each results}}
                    <li class="list-group-item"><a href="/Query/{{QueryId}}">{{Name}}</a></li>
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
           ('WidgetSearchesPython','Edit Python Script',
           'def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    params = { ''uname'': Data.CurrentUser.Username }
    Data.results = q.QuerySql(sql, params)
    print model.RenderTemplate(template)

Get()',
           GETDATE(),5,0,0,0,'admin')
          
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
           ('WidgetSearchesSQL','Edit Sql Script',
           'select QueryId, Name from Query
where owner like @uname
and Name != ''scratchpad''
order by Name',
           GETDATE(),4,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'My Searches' and [System] = 1) = 0
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
           ('My Searches'
           ,'Shows a list of searches for the current user'
           ,(select max(Id) from Content where [Name] like 'WidgetSearchesHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetSearchesPython')
           ,(select max(Id) from Content where [Name] like 'WidgetSearchesSQL')
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,2
           ,1)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))
END
GO

-- add welcome widget
IF (select count(*) from DashboardWidgets where [Name] like 'Welcome to Touchpoint') = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetWelcomeHTML','Edit Text Content',
           '<div class="box" style="background-color:white;">
    <div class="center">
        <h2 style="color:#003f72;text-align:center;">Welcome to TouchPoint!</h2>
        <div style="margin-bottom: 15px;">
            <div class="col-xs-4" style="text-align:center;display: flex;flex-direction: column;">
                <i class="fa fa-video-camera fa-2x" style="margin-bottom: 8px;"></i>
                <a href="https://www.touchpointsoftware.com/blog/#on-demand-webinars">Webinars</a>
            </div>
            <div class="col-xs-4" style="text-align:center;display: flex;flex-direction: column;">
                <i class="fa fa-book fa-2x" style="margin-bottom: 8px;"></i>
                <a href="https://docs.touchpointsoftware.com/">Documentation</a>
            </div>
            <div class="col-xs-4" style="text-align:center;display: flex;flex-direction: column;">
                <i class="fa fa-question-circle fa-2x" style="margin-bottom: 8px;"></i>
                <a href="https://www.touchpointsoftware.com/training/">Training</a>
            </div>
        </div>
    </div>
</div>',
           GETDATE(),1,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')

END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Welcome to Touchpoint') = 0
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
           ('Welcome to Touchpoint'
           ,'Helpful links to resources on Touchpoint.'
           ,(select max(Id) from Content where [Name] like 'WidgetWelcomeHTML')
           ,NULL
           ,NULL
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,0
           ,1
           ,24)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))
END
GO

-- add involvement widget
IF (select count(*) from DashboardWidgets where [Name] like 'My Involvement' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetInvolvementHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="/Person2/{{CurrentPerson.PeopleId}}#enrollment">{{WidgetName}}</a></h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        {{#ifGT results.Count 0}}
            <ul class="list-group">
                {{#each results}}
                    {{#ifEqual New "New"}}
                        <li class="list-group-item section">{{Description}}</li>
                    {{/ifEqual}}
                    <li class="list-group-item indent"><a href="/Org/{{OrganizationId}}">{{OrganizationName}}</a></li>
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
           ('WidgetInvolvementPython','Edit Python Script',
           'def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    orgleadersonly = False
    
    for item in Data.CurrentUser.UserRoles:
        if item.Role.RoleName == ''OrgLeadersOnly'':
            orgleadersonly = True

    params = { ''olo'': orgleadersonly, ''pid'': Data.CurrentPerson.PeopleId }
    results = q.QuerySql(sql, params)
    currentOrgType = "Other"
    for item in results:
        if item.Description != currentOrgType:
            item.New = ''New''
            currentOrgType = item.Description
        
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
INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetInvolvementSQL','Edit Sql Script',
           'select org.OrganizationName, om.OrganizationId, ISNULL(ot.Description, ''Other'') as Description from OrganizationMembers om
join Organizations org on om.OrganizationId = org.OrganizationId
left join lookup.OrganizationType ot on org.OrganizationTypeId = ot.Id
left join lookup.MemberType mt on om.MemberTypeId = mt.Id
where om.PeopleId = @pid
and om.Pending != 1
and org.SecurityTypeId != 3
order by ISNULL(ot.Description, ''ZZ''), org.OrganizationName',
           GETDATE(),4,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'My Involvement' and [System] = 1) = 0
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
           ('My Involvement'
           ,'Shows a list of organizations the current user is a member of'
           ,(select max(Id) from Content where [Name] like 'WidgetInvolvementHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetInvolvementPython')
           ,(select max(Id) from Content where [Name] like 'WidgetInvolvementSQL')
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,2
           ,1)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))
END
GO

-- add birthdays widget
IF (select count(*) from DashboardWidgets where [Name] like 'Birthdays' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetBirthdaysHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="/Tags?tag=TrackBirthdays">{{WidgetName}}</a></h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        {{#ifGT results.Count 0}}
            <ul class="list-group">
                {{#each results}}
                    <li class="list-group-item"><a target="_blank" href="/Person2/{{PeopleId}}">
                        {{Name}} ({{Birthday}})
                    </a></li>
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
           ('WidgetBirthdaysPython','Edit Python Script',
           'def Get():
    template = Data.HTMLContent
    Data.results = model.BirthdayList()
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

IF (select count(*) from DashboardWidgets where [Name] like 'Birthdays' and [System] = 1) = 0
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
           ('Birthdays'
           ,'Displays a list of upcoming birthdays for the current user'
           ,(select max(Id) from Content where [Name] like 'WidgetBirthdaysHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetBirthdaysPython')
           ,NULL
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,2
           ,4)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))
END
GO
-- add recurring giving forecast widget
IF (select count(*) from DashboardWidgets where [Name] like 'Recurring Giving Forecast' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
       [Body],
       [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetGivingForecastHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="#">{{WidgetName}}</a></h5>
    </div>
    <a class="visible-xs-block" id="{{WidgetId}}-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}
            </h5>
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        <div class="box-content center">
            <h4 class="text-center">{{fund}}</h4>
            <div class="chart">
            </div>
            <p class="text-center" style="margin-top:10px;">Based on annual budget of ${{FmtNumber budget}}</p>
        </div>
    </div>
</div>
<script type="text/javascript">
    var data = {{{results}}};
    var budget = {{budget}};
    var projected = Math.round(data.Combined.MonthlyAmt * 12);
    google.charts.load("current", {packages:["corechart"]});
    google.charts.setOnLoadCallback(function() {
    data = google.visualization.arrayToDataTable([
        [''Item'', ''Dollars''],
        [''Projected Monthly Recurring'', projected],
        [''Remaining Budget'', budget - projected]
    ]);
    
    var options = {
        pieHole: 0.4,
        legend: ''none'',
        chartArea: {
            left: 0,
            top: 0,
            width: ''100%'',
            height: ''100%''
        }
    };
    
    var chart = new google.visualization.PieChart(document.querySelector(''#{{WidgetId}}-section .chart''));
    chart.draw(data, options);
    });
</script>',
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
           ('WidgetGivingForecastPython','Edit Python Script',
           'FundId = 9
FundName = ''Benevolence''
AnnualBudget = 48000 # in dollars
TagMembers = ''GivingForecast-Members''
TagNonMembers = ''GivingForecast-ActiveNonMembers''

def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    params = { ''FundId'': FundId, ''TagMembers'': TagMembers, ''TagNonMembers'': TagNonMembers }
    
    model.CreateQueryTag(TagMembers, 
        ''FamHasPrimAdultChurchMemb = 1 AND IncludeDeceased = 1'')
    
    model.CreateQueryTag(TagNonMembers, 
    ''''''
        ( 
            RecentFamilyAdultLastAttend( Days=365 ) = 1 
            OR IsFamilyGiver( Days=365 ) = 1
        ) 
        AND FamHasPrimAdultChurchMemb = 0 
        AND IncludeDeceased = 1
    '''''')

    results = q.SqlFirstColumnRowKey(sql, params)
    Data.results = model.FormatJson(results)
    Data.fund = FundName
    Data.budget = AnnualBudget
    print model.RenderTemplate(template)
    
Get()',
           GETDATE(),5,0,0,0,'admin')
          
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
           ('WidgetGivingForecastSQL','Edit Sql Script',
           'with Members as (
  select tp.PeopleId, p.FamilyId
  from dbo.TagPerson tp
  join dbo.Tag t on t.Id = tp.Id
  join dbo.People p on p.PeopleId = tp.PeopleId
  where t.Name = @TagMembers
  and t.TypeId = 101 -- QueryTag
),
NonMembers as (
  select tp.PeopleId, p.FamilyId
  from dbo.TagPerson tp
  join dbo.Tag t on t.Id = tp.Id
  join dbo.People p on p.PeopleId = tp.PeopleId
  where t.Name = @TagNonMembers
  and t.TypeId = 101 -- QueryTag
),
Both as (
  select PeopleId, FamilyId, ''Member'' MemberStatus
  from Members
  union
  select PeopleId, FamilyId, ''NonMember''
  from NonMembers
),
FamilyCounts as (
  select 
    MemberStatus = ''Member'',
    FamilyCnt = count(distinct FamilyId)
  from Members

  union all

  select 
    MemberStatus = ''NonMember'',
    FamilyCnt = count(distinct FamilyId)
  from NonMembers
),
RecurTable as(
    select PeopleId
    from dbo.RecurringAmounts
    where (FundID = @FundId or @FundId is null)
    and Amt > 0
),
RecurringCounts as (
  select 
    MemberStatus = ''Member'',
    RecurringCnt = count(distinct FamilyId)
  from Members m
  where exists(select null from RecurTable where PeopleId =  m.PeopleId)

  union all

  select 
    MemberStatus = ''NonMember'',
    RecurringCnt = count(distinct FamilyId)
  from NonMembers n
  where exists(select null from RecurTable where PeopleId =  n.PeopleId)
),
RecurringAmts as (
  select 
    RecurringType = ''Monthly'',
    b.MemberStatus,
    MonthlyAmt = isnull(sum(Amt),0)
  from dbo.RecurringAmounts a
  join dbo.ManagedGiving mg on a.peopleid = mg.peopleid
  join Both b on b.PeopleId = mg.PeopleId
  where (a.fundid = @FundId or @FundId is null)
  and SemiEvery=''E'' and EveryN=1 and Period=''M''
  group by b.MemberStatus

  union all

  select 
    RecurringType = ''Weekly'',
    b.MemberStatus,
    MonthlyAmt = 4.345 * isnull(sum(Amt),0)
  from dbo.RecurringAmounts a
  join dbo.ManagedGiving mg on a.peopleid = mg.peopleid
  join Both b on b.PeopleId = mg.PeopleId
  where (a.fundid = @FundId or @FundId is null)
  and SemiEvery=''E'' and EveryN=1 and Period=''W''
  group by b.MemberStatus

  union all

  select 
    RecurringType = ''BiWeeky'',
    b.MemberStatus,
    MonthlyAmt = 2.18 * isnull(sum(Amt),0)
  from dbo.RecurringAmounts a
  join dbo.ManagedGiving mg on a.peopleid = mg.peopleid
  join Both b on b.PeopleId = mg.PeopleId
  where (a.fundid = @FundId or @FundId is null)
  and SemiEvery=''E'' and EveryN=2 and Period=''W''
  group by b.MemberStatus

  union all

  select 
    RecurringType = ''SemiMonthly'',
    b.MemberStatus,
    MonthlyAmt = 2 * isnull(sum(Amt),0)
  from dbo.RecurringAmounts a
  join dbo.ManagedGiving mg on a.peopleid = mg.peopleid
  join Both b on b.PeopleId = mg.PeopleId
  where (a.fundid = @FundId or @FundId is null)
  and SemiEvery=''S''
  group by b.MemberStatus
),
Totals as (
  select 
    MemberStatus, 
    MonthlyAmt = sum(MonthlyAmt)
  from RecurringAmts
  group by MemberStatus
)
select
  RowType = isnull(t.MemberStatus, ''Combined''),
  FamilyCnt = sum(FamilyCnt),
  RecurringCnt = sum(RecurringCnt),
  MonthlyAmt = sum(MonthlyAmt)
from Totals t
join FamilyCounts c on c.MemberStatus = t.MemberStatus
join RecurringCounts r on r.MemberStatus = t.MemberStatus
group by rollup(t.MemberStatus)',
           GETDATE(),4,0,0,0,'admin')

INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Recurring Giving Forecast' and [System] = 1) = 0
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
           ('Recurring Giving Forecast'
           ,'Shows a pie chart with projected monthly recurring giving as a percentage of total budget'
           ,(select max(Id) from Content where [Name] like 'WidgetGivingForecastHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingForecastPython')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingForecastSQL')
           ,0
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,1
           ,24)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Finance')),
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'FinanceAdmin')),
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'FinanceViewOnly')),
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'FinanceDataEntry')),
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Admin'))
END
GO

-- add login metrics widget
IF (select count(*) from DashboardWidgets where [Name] like 'Login Metrics' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
       [Body],
       [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetLoginMetricsHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5><a href="#">{{WidgetName}}</a></h5>
    </div>
    <a class="visible-xs-block" id="giving-fc-collapse" data-toggle="collapse" href="#{{WidgetId}}-section" aria-expanded="true" aria-controls="{{WidgetId}}-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{WidgetName}}
            </h5>
        </div>
    </a>
    <div class="collapse in" id="{{WidgetId}}-section">
        <div class="box-content center">
            <h4 class="text-center">Login Metrics - Last {{days}} days</h4>
            <div class="chart">
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    var data = {{{counts}}};
    google.charts.load("current", {packages:["corechart"]});
    google.charts.setOnLoadCallback(function() {
        data = google.visualization.arrayToDataTable([
            [''Item'', ''Logins''],
            [''Web'', {{counts.web}}],
            [''Mobile'', {{counts.mobile}}],
            [''Total'', {{counts.web}}]
        ]);
        var options = {
            colors: [''#9575cd''],
            vAxis: {
                title: ''Unique logins''
            },
            legend: {
                position: ''none''
            }
        };
        var chart = new google.visualization.ColumnChart(document.querySelector(''#{{WidgetId}}-section .chart''));
        chart.draw(data, options);
    });
</script>',
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
           ('WidgetLoginMetricsPython','Edit Python Script',
           'from System.DateTime import Now
Days = 30 # Report on logins for the past # of days entered here

def Get():
    today = Now
    StartDate = today.AddDays(-Days).Date
    strToday = today.ToString(''d'')
    strStartDate = StartDate.ToString(''d'')
    template = Data.HTMLContent
    
    appquery = ''''''
    SELECT count(distinct p.peopleid) as Count
    FROM dbo.MobileAppDevices mad
    INNER JOIN dbo.People p on mad.PeopleId = p.PeopleId
    WHERE mad.lastseen > ''{}''
    ''''''
    
    webquery = ''''''
    WITH loginUsers (userid) AS
    (
        SELECT distinct userid
        FROM dbo.ActivityLog log
        WHERE ActivityDate > ''{}''
        AND Activity like ''%logged in''
    )
    SELECT count(distinct lu.userid) as Count
    FROM loginUsers lu INNER JOIN dbo.Users u ON lu.Userid = u.UserId
    INNER JOIN dbo.People p ON u.PeopleId = p.PeopleId
    ''''''
    
    combinedquery = ''''''
    WITH loginUsers (userid) AS
    (
        SELECT distinct userid
        FROM dbo.ActivityLog log
        WHERE ActivityDate > ''{}''
        AND Activity like ''%logged in''
    )
    ,webUsers (web_userid) AS
    (
        SELECT lu.UserId
        FROM loginUsers lu INNER JOIN dbo.Users u ON lu.Userid = u.UserId
        INNER JOIN dbo.People p ON u.PeopleId = p.PeopleId
    )
    ,appUsers (app_userid) AS
        (SELECT distinct userid
        FROM dbo.MobileAppDevices mad
        INNER JOIN dbo.People p on mad.PeopleId = p.PeopleId
        WHERE mad.lastseen > ''{}''
    )
    SELECT count(*) as Count
    FROM webUsers FULL OUTER JOIN appUsers ON webUsers.web_userid = appUsers.app_userId
    ''''''
    
    counts = {}
    
    # Mobile app logins
    rowcolquery = appquery.format(strStartDate)
    data = q.QuerySql(rowcolquery)
    for i in data:
        counts["mobile"] =  int(i.Count or 0)
    
    # Web user logins
    rowcolquery = webquery.format(strStartDate)
    data = q.QuerySql(rowcolquery)
    for i in data:
        counts["web"] =  int(i.Count or 0)
    
    # Total unique login users
    rowcolquery = combinedquery.format(strStartDate, strStartDate)
    data = q.QuerySql(rowcolquery)
    
    for i in data:
        counts["total"] =  int(i.Count or 0)
        
    Data.days = Days
    Data.counts = model.DynamicData(counts)
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

IF (select count(*) from DashboardWidgets where [Name] like 'Login Metrics' and [System] = 1) = 0
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
           ('Login Metrics'
           ,'Shows a bar chart with login stats for the time period chosen'
           ,(select max(Id) from Content where [Name] like 'WidgetLoginMetricsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetLoginMetricsPython')
           ,NULL
           ,1
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,1
           ,12)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Edit')),
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Admin'))
END
GO