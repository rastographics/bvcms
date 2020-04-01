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
                    <li class="list-group-item"><a href="/Task/{{TaskId}}">{{Description}}</a> (<a href="{{Url}}">{{Who}}</a>)</li>
                {{/each}}
            </ul>
        {{else}}
            <div class="box-content"></div>
        {{/ifGT}}
    </div>
</div>',
           GETDATE(),1,0,0,0,'admin')

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
           ,[System])
     VALUES
           ('My Tasks'
           ,'Shows a list of tasks for the current user'
           ,(select max(Id) from Content where [Name] like 'WidgetTasksHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetTasksPython')
           ,(select max(Id) from Content where [Name] like 'WidgetTasksSQL')
           ,1
           ,(select max([Order])+1 from DashboardWidgets)
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
                    <li class="list-group-item"><a href="/Tags?tag={{Id}}">{{Name}}</a></li>
                {{/each}}
            </ul>
        {{else}}
            <div class="box-content"></div>
        {{/ifGT}}
    </div>
</div>',
           GETDATE(),1,0,0,0,'admin')

INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetTagsPython','Edit Python Script',
           'import datetime

def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    params = { ''pid'': Data.CurrentPerson.PeopleId }
    Data.results = q.QuerySql(sql, params)
    print model.RenderTemplate(template)

Get()',
           GETDATE(),5,0,0,0,'admin')
           
INSERT INTO [dbo].[Content]
           ([Name],[Title],
		   [Body],
		   [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetTagsSQL','Edit Sql Script',
           'select * from Tag t
where t.PeopleId = @pid
and t.TypeId = 1
union all
select t.* from Tag t
join TagShare ts on t.Id = ts.TagId
where t.PeopleId != @pid
and t.TypeId = 1
and ts.PeopleId = @pid
order by t.Name',
           GETDATE(),4,0,0,0,'admin')

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
           ,[System])
     VALUES
           ('My Tags'
           ,'Shows a list of tags for the current user'
           ,(select max(Id) from Content where [Name] like 'WidgetTagsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetTagsPython')
           ,(select max(Id) from Content where [Name] like 'WidgetTagsSQL')
           ,1
           ,(select max([Order])+1 from DashboardWidgets)
           ,1)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
     VALUES
           ((select SCOPE_IDENTITY())
           ,(select min(RoleId) from Roles where RoleName like 'Access'))

END
GO


