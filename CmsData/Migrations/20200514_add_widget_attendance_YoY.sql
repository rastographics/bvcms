-- add Attendance Year Over Year widget
IF (select count(*) from DashboardWidgets where [Name] like 'Attendance Year Over Year' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetAttendanceYearOverYearHTML','Edit Text Content',
           '<div class="box">
    <div class="box-title hidden-xs">
        <h5>{{WidgetName}}</h5>
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
        <div id="tester"></div>
            <div class="chart">
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    var inputList = [];
    var outputList = [];
    var year1 = [];
    var year2 = [];
    var year3 = [];
    var dates = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var {{WidgetId}} = function() {
        var data = new google.visualization.DataTable();
        data.addColumn(''string'', ''Name'');
        {{{addcolumns}}}
        inputList = {{{rowdata}}};
        
        let intervalYear = {{{startingyear}}};
        for(let i = 1; i <= {{{numyears}}}; i++)
        {
            for(let j = 1; j <= 12; j++)
            {
                if(intervalYear == {{{currentyear}}} && j > {{{currentmonth}}})
                {
                    break;
                }
                else
                {
                    //count++;
                    outputList.push([0, j, intervalYear]);
                }
            }
            intervalYear++;
        }
        
        for(let i = 0; i < outputList.length; i++)
        {
            var match = false;
            for(let j = 0; j < inputList.length; j++)
            {
                if(outputList[i][1] == inputList[j][1] && outputList[i][2] == inputList[j][2])
                {
                    outputList[i][0] += inputList[j][0];
                    break;
                }
            }
        }
        
        for(let i = 0; i < outputList.length; i++)
        {
            if(outputList[i][2] == {{{startingyear}}})
            {
                year1.push(outputList[i]);
            }
            if(outputList[i][2] == {{{startingyear}}} +1)
            {
                year2.push(outputList[i]);
            }
            if(outputList[i][2] == {{{startingyear}}} +2)
            {
                year3.push(outputList[i]);
            }
        }
        
        for(let i = 0; i < year1.length; i++)
        {
            switch(year1[i][1])
            {
                case 1:
                    year1[i][1] = ''Jan'';
                    break;
                case 2:
                    year1[i][1] = ''Feb'';
                    break;
                case 3:
                    year1[i][1] = ''Mar'';
                    break;
                case 4:
                    year1[i][1] = ''Apr'';
                    break;
                case 5:
                    year1[i][1] = ''May'';
                    break;
                case 6:
                    year1[i][1] = ''Jun'';
                    break;
                case 7:
                    year1[i][1] = ''Jul'';
                    break;
                case 8:
                    year1[i][1] = ''Aug'';
                    break;
                case 9:
                    year1[i][1] = ''Sep'';
                    break;
                case 10:
                    year1[i][1] = ''Oct'';
                    break;
                case 11:
                    year1[i][1] = ''Nov'';
                    break;
                case 12:
                    year1[i][1] = ''Dec'';
                    break;
                default:
                    break;
            }
        }
        for(let i = 0; i < year2.length; i++)
        {
            switch(year2[i][1])
            {
                case 1:
                    year2[i][1] = ''Jan'';
                    break;
                case 2:
                    year2[i][1] = ''Feb'';
                    break;
                case 3:
                    year2[i][1] = ''Mar'';
                    break;
                case 4:
                    year2[i][1] = ''Apr'';
                    break;
                case 5:
                    year2[i][1] = ''May'';
                    break;
                case 6:
                    year2[i][1] = ''Jun'';
                    break;
                case 7:
                    year2[i][1] = ''Jul'';
                    break;
                case 8:
                    year2[i][1] = ''Aug'';
                    break;
                case 9:
                    year2[i][1] = ''Sep'';
                    break;
                case 10:
                    year2[i][1] = ''Oct'';
                    break;
                case 11:
                    year2[i][1] = ''Nov'';
                    break;
                case 12:
                    year2[i][1] = ''Dec'';
                    break;
                default:
                    break;
            }
        }
        for(let i = 0; i < year3.length; i++)
        {
            switch(year3[i][1])
            {
                case 1:
                    year3[i][1] = ''Jan'';
                    break;
                case 2:
                    year3[i][1] = ''Feb'';
                    break;
                case 3:
                    year3[i][1] = ''Mar'';
                    break;
                case 4:
                    year3[i][1] = ''Apr'';
                    break;
                case 5:
                    year3[i][1] = ''May'';
                    break;
                case 6:
                    year3[i][1] = ''Jun'';
                    break;
                case 7:
                    year3[i][1] = ''Jul'';
                    break;
                case 8:
                    year3[i][1] = ''Aug'';
                    break;
                case 9:
                    year3[i][1] = ''Sep'';
                    break;
                case 10:
                    year3[i][1] = ''Oct'';
                    break;
                case 11:
                    year3[i][1] = ''Nov'';
                    break;
                case 12:
                    year3[i][1] = ''Dec'';
                    break;
                default:
                    break;
            }
        }
        
        switch({{{numyears}}})
        {
            case 1:
                for (let i = 0; i < 12; i++) 
                {
                    if(year1[i] == null)
                    {
                        data.addRow([dates[i], null]);
                    }
                    else
                    {
                        data.addRow([dates[i], year1[i][0]]);
                    }
                }
                break;
            case 2:
                for (let i = 0; i < 12; i++) 
                {
                    if(year2[i] == null)
                    {
                        data.addRow([dates[i], year1[i][0], null]);
                    }
                    else
                    {
                        data.addRow([dates[i], year1[i][0], year2[i][0]]);
                    }
                }
                break;
            case 3:
                for (let i = 0; i < 12; i++) 
                {
                    if(year3[i] == null)
                    {
                        data.addRow([dates[i], year1[i][0], year2[i][0], null]);
                    }
                    else
                    {
                        data.addRow([dates[i], year1[i][0], year2[i][0], year3[i][0]]);
                    }
                }
                break;
            default:
            break;
        }
        
        
        var options = {
            hAxis: {
            title: ''Dates''
            },
            colors: [''#CC0300'', ''#5CA12B'', ''#345CAD'', ''#F9B710'', ''#9E4EBC'', ''#74746D''],
            hAxis: {
                slantedText: true,
                slantedTextAngle: 30,
                textStyle: {
                    fontSize: 8
                },
                viewWindow: {
                    min: 0,
                    max: 12
                }
            },
            legend: {
                position: "top",
                textStyle: {fontSize: 8},
                alignment: ''center''
            }
        };
        
        var chart = new google.visualization.LineChart(document.querySelector(''#{{WidgetId}}-section .chart''));
        chart.draw(data, options);
    }
    // load and register the chart
    google.charts.load("current", {packages:["corechart"]});
    google.charts.setOnLoadCallback({{WidgetId}});
    WidgetCharts.{{WidgetId}} = {{WidgetId}};
</script>',
           GETDATE(),1,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id],[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')

INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetAttendanceYearOverYearPython','Edit Python Script',
           '# Users can add divisions to include in the line chart for this widget inside this divisions array.
# This array is made up of objects. Each object contains a name and a division id number.
# The first item in the object is the name you wish to display in the widget for the selected division.
# The second item in the object is the division id you wish to display in the widget.
divisions = [
    [''Young Couples'', 210],
    [''Young Marrieds'', 211],
    [''Adults 1'', 213]
]
# You can change startyear to the date you wish to start the chart for your widget.
# Notice, the furthest back you can select is 3 years.
startyear = ''2018''





import datetime
currentDate = datetime.datetime.now()

def GetData(divisions, startyear):
    sql = model.Content(''WidgetAttendanceYearOverYearSQL'')
    divids = [row[1] for row in divisions]
    sql = sql.replace(''@startyear'', startyear)
    sql = sql.replace(''@divs'', str(divids)[1:-1])
    return q.QuerySqlJsonArray(sql)
    
def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    Data.rowdata = GetData(divisions, startyear)
    startingYear = int(startyear)
    maxYears = currentDate.year - 2
    if startingYear < maxYears:
        startingYear = maxYears
    numYears = currentDate.year - startingYear + 1
    selectedStartingYear = startingYear
    selectedYears = []
    while selectedStartingYear <= currentDate.year:
        selectedYears.append(selectedStartingYear)
        selectedStartingYear += 1
    addcolumn = "data.addColumn(''number'', ''{}'');"
    alist = [addcolumn.format(x) for x in selectedYears]
    Data.addcolumns = ''\n''.join(alist)
    Data.startingyear = startingYear
    Data.numyears = numYears
    Data.currentyear = currentDate.year
    Data.currentmonth = currentDate.month
    print model.RenderTemplate(template)
    
Get()',
           GETDATE(),5,0,0,0,'admin')
          
INSERT INTO [dbo].[ContentKeyWords]
           ([Id],[Word])
     VALUES
           ((select SCOPE_IDENTITY()),'widget')
INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetAttendanceYearOverYearSQL','Edit Sql Script',
           'select MaxCount, dd.DivId DivisionId, m.MeetingDate into #meetingdata
from dbo.Meetings m
join dbo.Organizations o on o.OrganizationId = m.OrganizationId
join dbo.DivOrg dd on dd.orgid = o.OrganizationId
join dbo.Division d on d.Id = dd.DivId
where dd.DivId in (@divs) and YEAR(MeetingDate) >= YEAR(''@startyear'');
with data as ( select MaxCount, MeetingDate from #meetingdata )
select 
SUM(MaxCount) as MaxCount, 
Month(MeetingDate) as MyMonth, 
Year(MeetingDate) as MyYear 
from #meetingdata 
group by Month(MeetingDate), Year(MeetingDate)
order by Year(MeetingDate), Month(MeetingDate)
drop table #meetingdata',
           GETDATE(),4,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Attendance Year Over Year' and [System] = 1) = 0
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
           ('Attendance Year Over Year'
           ,'Shows a line chart comparing attendance year over year by division'
           ,(select max(Id) from Content where [Name] like 'WidgetAttendanceYearOverYearHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetAttendanceYearOverYearPython')
           ,(select max(Id) from Content where [Name] like 'WidgetAttendanceYearOverYearSQL')
           ,0
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,1
           ,6)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
    SELECT SCOPE_IDENTITY() [WidgetId], RoleId FROM dbo.Roles WHERE RoleName in ('Edit')
END
GO
