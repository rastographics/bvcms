-- add ratio line chart widget
IF (select count(*) from DashboardWidgets where [Name] like 'Current Ratio by Week' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetCurrentRatioByWeekHTML','Edit Text Content',
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
    var divOneData = {{{divonedata}}};
    var divTwoData = {{{divtwodata}}};
    var adjustedDivOneData = [];
    var adjustedDivTwoData = [];
    const startingYear = {{{startingyear}}};
    const startingWeek = {{{startingweek}}};
    const fixedNumber = {{{fixednumber}}};
    const fixedName = ''{{{fixedname}}}'';
    const DisplayAttendanceVsAttendance = {{{displayattendancevsattendance}}};
    const numYears = {{{numyears}}};
    const numWeeks = {{{numweeks}}};
    var weeklySundays = [];
    
    var {{WidgetId}} = function() {
        var data = new google.visualization.DataTable();
        data.addColumn(''string'', ''Name'');
        {{{addcolumns}}}
        
        weeklySundays = GetThisSunday();
        
        if(DisplayAttendanceVsAttendance == true){
            for(let i = 0; i < 3; i++){
                for(let j = 0; j < 12; j++){
                    let tempHeadCount = 0;
                    let tempWeekCount = startingWeek[0][0] + j;
                    let tempYearCount = startingYear + i;
                    for(let k = 0; k < divOneData.length; k++){
                        if(divOneData[k][0] == tempWeekCount && divOneData[k][2] == tempYearCount){
                            tempHeadCount = divOneData[k][1];
                            break;
                        }
                    }
                    tempHeadCount = tempHeadCount / fixedNumber;
                    var tempObject = [weeklySundays[j], tempHeadCount, tempYearCount];
                    adjustedDivOneData.push(tempObject);
                }
                for(let j = 0; j < 12; j++){
                    let tempHeadCount = 0;
                    let tempWeekCount = startingWeek[0][0] + j;
                    let tempYearCount = startingYear + i;
                    for(let k = 0; k < divTwoData.length; k++){
                        if(divTwoData[k][0] == tempWeekCount && divTwoData[k][2] == tempYearCount){
                            tempHeadCount = divTwoData[k][1];
                            break;
                        }
                    }
                    tempHeadCount = tempHeadCount / fixedNumber;
                    var tempObject = [weeklySundays[j], tempHeadCount, tempYearCount];
                    adjustedDivTwoData.push(tempObject);
                }
                for(let i = 0; i < adjustedDivOneData.length; i++){
                    if(adjustedDivTwoData[i][1] != 0){
                        adjustedDivOneData[i][1] = adjustedDivOneData[i][1] / adjustedDivTwoData[i][1];
                    } else{
                        adjustedDivOneData[i][1] = 0;
                    }
                }
            }
        } else{
            for(let i = 0; i < 3; i++){
                for(let j = 0; j < 12; j++){
                    let tempHeadCount = 0;
                    let tempWeekCount = startingWeek[0][0] + j;
                    let tempYearCount = startingYear + i;
                    for(let k = 0; k < divOneData.length; k++){
                        if(divOneData[k][0] == tempWeekCount && divOneData[k][2] == tempYearCount){
                            tempHeadCount = divOneData[k][1];
                            break;
                        }
                    }
                    tempHeadCount = tempHeadCount / fixedNumber;
                    var tempObject = [weeklySundays[j], tempHeadCount, tempYearCount];
                    adjustedDivOneData.push(tempObject);
                }
            }
        }
        
        switch(numYears){
            case 1:
                for(let i = 0; i < numWeeks; i++){
                    let year1 = i;
                    data.addRow([adjustedDivOneData[i][0].toString(),adjustedDivOneData[year1][1]]);
                }
                break;
            case 2:
                for(let i = 0; i < numWeeks; i++){
                    let year1 = i;
                    let year2 = i + numWeeks;
                    data.addRow([adjustedDivOneData[i][0].toString(),adjustedDivOneData[year1][1], adjustedDivOneData[year2][1]]);
                }
                break;
            case 3:
                for(let i = 0; i < numWeeks; i++){
                    let year1 = i;
                    let year2 = i + numWeeks;
                    let year3 = i + (numWeeks * 2);
                    data.addRow([adjustedDivOneData[i][0].toString(),adjustedDivOneData[year1][1], adjustedDivOneData[year2][1], adjustedDivOneData[year3][1]]);
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
                slantedText: false,
                textStyle: {
                    fontSize: 10
                }
            },
            legend: {
                position: "top",
                textStyle: {fontSize: 10},
                alignment: ''center''
            },
        };
        
        var chart = new google.visualization.LineChart(document.querySelector(''#{{WidgetId}}-section .chart''));
        chart.draw(data, options);
    }
    
    function GetThisSunday(){
        var thisSunday = new Date();
        for(let i = 0; i < 7; i++){
            if(thisSunday.getDay() != 0){
                thisSunday.setDate(thisSunday.getDate() + 1);
            } else{
                break;
            }
        }
        var sundays = [];
        thisSunday.setDate(thisSunday.getDate() + 7);
        for(let i = 11; i >= 0; i--){
            thisSunday.setDate(thisSunday.getDate() - 7);
            let day = thisSunday.getDate().toString();
            let month = thisSunday.getMonth() + 1;
            month = month.toString();
            sundays[i] = month + ''/'' + day;
        }
        return sundays;
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
           ('WidgetCurrentRatioByWeekPython','Edit Python Script',
           '# Users can add divisions to include in the ratio chart for this widget inside this divisions array.
# Only two divisions can be selected. More than that will cause issues.
# This array is made up of two objects. Each object is a division. Each object contains a name and a division id number.
# The first item in the object is the name you wish to display in the widget for the selected division.
# The second item in the object is the division id you wish to select for this widget.
divisions = [
    [''Young Couples'', 210],
    [''Young Marrieds'', 211]
]
# You can change startyear to the date you wish to start the chart for your widget. Notice, the furthest back you can select is 3 years.
startyear = ''2018''

# Here you can set a fixed number and a name for this fixed number. This will be included in the ratio line graph for each year selected.
fixedNumber = 500
fixedName = ''Church Seating Capacity''

# The ratio for this graph will be based on a comparison between the first division in the array above and the fixed number chosen above.
# To change this to a comparison between two divisions in the array above, change this setting to true
displayAttendanceVsAttendance = ''false''

# This will allow you to adjust the number of weeks to display on the graph, maximum of 12 weeks.
numWeeks = 12






import datetime
currentDate = datetime.datetime.now()

def GetData(divid, startingYear):
    sql = Data.SQLContent
    sql = sql.replace(''@startYear'', str(startingYear))
    sql = sql.replace(''@divId'', str(divid))
    return q.QuerySqlJsonArray(sql)
    
def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
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
    Data.startingweek = q.QuerySqlJsonArray(''select DATEPART(WEEK, GETDATE() - 77) w'')
    Data.numyears = numYears
    Data.numweeks = numWeeks
    Data.divonedata = GetData(divisions[0][1], startingYear)
    divLength = len(divisions)
    if divLength > 1:
        Data.divtwodata = GetData(divisions[1][1], startingYear)
    else:
        Data.divtwodata = 0
    Data.displayattendancevsattendance = displayAttendanceVsAttendance.lower()
    Data.fixednumber = fixedNumber
    Data.fixedname = fixedName
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
           ('WidgetCurrentRatioByWeekSQL','Edit Sql Script',
           'set datefirst 1;
with weeks as (
select DATEPART(WEEK, GETDATE()) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 7) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 14) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 21) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 28) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 35) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 42) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 49) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 56) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 63) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 70) w UNION ALL 
select DATEPART(WEEK, GETDATE() - 77) w 
),
attendance as (
select weeks.w, SUM(COALESCE(HeadCOUNT,0)) as HeadCount, YEAR(m.MeetingDate) as MyYear from weeks
cross join dbo.Organizations o
left join dbo.Meetings m on DATEPART(WEEK, m.MeetingDate) = weeks.w and o.OrganizationId = m.OrganizationId and YEAR(m.MeetingDate) >= YEAR(''@startYear'')
where o.DivisionId in (@divId) 
group by weeks.w, YEAR(m.MeetingDate)
)
select * from attendance
order by w;',
           GETDATE(),4,0,0,0,'admin')

INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Current Ratio by Week' and [System] = 1) = 0
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
           ('Current Ratio by Week'
           ,'Shows a line chart comparison of attendance vs attendance by division, over the past three years.'
           ,(select max(Id) from Content where [Name] like 'WidgetCurrentRatioByWeekHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetCurrentRatioByWeekPython')
           ,(select max(Id) from Content where [Name] like 'WidgetCurrentRatioByWeekSQL')
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
