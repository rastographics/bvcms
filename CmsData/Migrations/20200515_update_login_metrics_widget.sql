update Content 
set Body = 'from System.DateTime import Now
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
        select UserId from Users where Username in (
        select distinct REPLACE(REPLACE(Activity, '' logged in'', ''''), ''User '', '''') as Username from dbo.ActivityLog al
            WHERE ActivityDate > ''2020-04-14''
            AND Activity like ''%logged in''
        )
    )
    SELECT count(distinct lu.userid) as Count
    FROM loginUsers lu INNER JOIN dbo.Users u ON lu.Userid = u.UserId
    INNER JOIN dbo.People p ON u.PeopleId = p.PeopleId
    ''''''
    
    combinedquery = ''''''
    WITH loginUsers (userid) AS
    (
        select UserId from Users where Username in (
        select distinct REPLACE(REPLACE(Activity, '' logged in'', ''''), ''User '', '''') as Username from dbo.ActivityLog al
            WHERE ActivityDate > ''2020-04-14''
            AND Activity like ''%logged in''
        )
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
    
Get()'
where HASHBYTES('MD5', Body) like 0xD50ED08489B2964407DBF1E6F8C6AD35
and Name like 'WidgetLoginMetricsPython'

update Content
set Body = '<div class="box">
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
    var {{WidgetId}} = function() {
        var data = {{{counts}}};
        data = google.visualization.arrayToDataTable([
            [''Item'', ''Logins'', { role: ''style'' }],
            [''Web'', {{counts.web}}, ''#9575cd''],
            [''Mobile'', {{counts.mobile}}, ''#f75590''],
            [''Total'', {{counts.total}}, ''#76A7FA'']
        ]);
        var options = {
            vAxis: {
                title: ''Unique logins'',
                minValue: 0
            },
            legend: {
                position: ''none''
            }
        };
        var chart = new google.visualization.ColumnChart(document.querySelector(''#{{WidgetId}}-section .chart''));
        chart.draw(data, options);
    };
    // load and register the chart
    google.charts.load("current", {packages:["corechart"]});
    google.charts.setOnLoadCallback({{WidgetId}});
    WidgetCharts.{{WidgetId}} = {{WidgetId}};
</script>'
where HASHBYTES('MD5', Body) like 0x8332089DC47A8F62C2E84E7998975C8C
and Name like 'WidgetLoginMetricsHTML'