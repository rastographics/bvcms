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
        <h5><a href="/TaskSearch">My Tasks</a></h5>
    </div>
    <a class="visible-xs-block" id="tasks-collapse" data-toggle="collapse" href="#tasks-section" aria-expanded="true" aria-controls="tasks-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;My Tasks
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="tasks-section">
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
           'import datetime

def Get():
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
        <h5><a href="/Tags">My Tags</a></h5>
    </div>
    <a class="visible-xs-block" id="tags-collapse" data-toggle="collapse" href="#tags-section" aria-expanded="true" aria-controls="tags-section">
        <div class="box-title">
            <h5><i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;My Tags</h5>
        </div>
    </a>
    <div class="collapse in" id="tags-section">
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
        <h5><a href="{{blogurl}}" target="_blank">{{blogtitle}}</a></h5>
    </div>
    <a class="visible-xs-block" id="news-collapse" data-toggle="collapse" href="#news-section" aria-expanded="true" aria-controls="news-section">
        <div class="box-title">
            <h5><i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;{{blogtitle}}</h5>
        </div>
    </a>
    <div class="collapse in" id="news-section">
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
    blogtitle = ''TouchPoint News''
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
    Data.blogtitle = blogtitle
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
    blogtitle = ''Church News''
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
    Data.blogtitle = blogtitle
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
        <h5><a href="/SavedQueryList">My Searches</a></h5>
    </div>
    <a class="visible-xs-block" id="searches-collapse" data-toggle="collapse" href="#searches-section" aria-expanded="true" aria-controls="searches-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;My Searches
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="searches-section">
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
        <h5><a href="/Person2/{{CurrentPerson.PeopleId}}#enrollment">My Involvement</a></h5>
    </div>
    <a class="visible-xs-block" id="involvements-collapse" data-toggle="collapse" href="#involvements-section" aria-expanded="true" aria-controls="involvements-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;My Involvement
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="involvements-section">
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
        <h5><a href="/Tags?tag=TrackBirthdays">Birthdays</a></h5>
    </div>
    <a class="visible-xs-block" id="birthdays-collapse" data-toggle="collapse" href="#birthdays-section" aria-expanded="true" aria-controls="birthdays-section">
        <div class="box-title">
            <h5>
                <i class="fa fa-chevron-circle-right"></i>&nbsp;&nbsp;Birthdays
            </h5>
            {{#ifGT results.Count 0}}
                <div class="pull-right">
                    <span class="badge badge-primary">{{results.Count}}</span>
                </div>
            {{/ifGT}}
        </div>
    </a>
    <div class="collapse in" id="birthdays-section">
        {{#ifGT results.Count 0}}
            <ul class="list-group">
                {{#each results}}
                    <li class="list-group-item"><a href="/Person2/{{PeopleId}}" class="target">
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
