-- add Giving Type Breakdown widget
IF (select count(*) from DashboardWidgets where [Name] like 'Giving Type Breakdown' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetGivingTypeBreakdownHTML','Edit Text Content',
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
            <div class="chart"></div>
        </div>
    </div>
</div>
<script type="text/javascript">
    var myData = [];
    var weekCount = [];
    var myResultsArrayofArrays = [];
    var {{WidgetId}} = function() {
        myData = {{{results}}};
        var data = new google.visualization.DataTable();
        data.addColumn(''string'', ''Week'');
        {{{addcolumns}}}
        
        var i = 0;
        $.each(myData,function(index, value){
            var myResultsArray = [];
            for(let j = 0; j < value.results.length; j++){
                myResultsArray[j] = value.results[j].amount;
                if(i == 0){
                    weekCount[j] = value.results[j].w.toString();
                }
            }
            myResultsArrayofArrays[i] = myResultsArray;
            i++;
        });
        
        switch(myResultsArrayofArrays.length)
        {
            case 1:
                for(let x = 0; x < 8; x++){
                    data.addRow([weekCount[x], myResultsArrayofArrays[0][x]]);
                }
                break;
            case 2:
                for(let x = 0; x < 8; x++){
                    data.addRow([weekCount[x], myResultsArrayofArrays[0][x], myResultsArrayofArrays[1][x]]);
                }
                break;
            case 3:
                for(let x = 0; x < 8; x++){
                    data.addRow([weekCount[x], myResultsArrayofArrays[0][x], myResultsArrayofArrays[1][x], myResultsArrayofArrays[2][x]]);
                }
                break;
            case 4:
                for(let x = 0; x < 8; x++){
                    data.addRow([weekCount[x], myResultsArrayofArrays[0][x], myResultsArrayofArrays[1][x], myResultsArrayofArrays[2][x], myResultsArrayofArrays[3][x]]);
                }
                break;
            case 5:
                for(let x = 0; x < 8; x++){
                    data.addRow([weekCount[x], myResultsArrayofArrays[0][x], myResultsArrayofArrays[1][x], myResultsArrayofArrays[2][x], myResultsArrayofArrays[3][x], myResultsArrayofArrays[4][x]]);
                }
                break;
            case 6:
                for(let x = 0; x < 8; x++){
                    data.addRow([weekCount[x], myResultsArrayofArrays[0][x], myResultsArrayofArrays[1][x], myResultsArrayofArrays[2][x], myResultsArrayofArrays[3][x], myResultsArrayofArrays[4][x], myResultsArrayofArrays[5][x]]);
                }
                break;
            default:
                break;
        }
        
        var options = {
            colors: [''#CC0300'', ''#5CA12B'', ''#345CAD'', ''#F9B710'', ''#9E4EBC'', ''#74746D''],
            seriesType: ''bars'',
            series: {5: {type: ''line''}},
            legend: {
                position: "bottom",
                textStyle: {fontSize: 8},
                alignment: ''center''
            },
            isStacked: true,
            vAxis: {format: ''currency''}
        };
        var chart = new google.visualization.ComboChart(document.querySelector(''#{{WidgetId}}-section .chart''));
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
           ('WidgetGivingTypeBreakdownPython','Edit Python Script',
           '# You can change FundIds to select the Fund you wish to view data for
FundIds = ''9''




from System import DateTime
# Initialize global data
MaxDate = DateTime.Today
GivingTypesData = model.DynamicData()
Priority = 0
Days = 56
selectedTags = []

Base = model.DynamicData()
Base.MinDate = MaxDate.AddDays(-Days).ToString("d")
Base.NonTaxDed = -1 # both tax and non tax
Base.MaxDate = MaxDate.ToString("d")
Base.FundIds = FundIds
    
def NextPriority():
    global Priority 
    Priority += 1
    return Priority

def GetResults(tagName):
    results = q.QuerySql(Data.SQLContent, { ''Tag'': tagName })
    return results
    
def CreateTag(fundSet, typeCategory, searchParameters):
    TagName = ''Gt{}-{}''.format(fundSet, typeCategory)
    NamePriority = ''{}-{}''.format(TagName, Priority)
    dd = model.DynamicData()
    dd.params = searchParameters
    dd.name = typeCategory
    selectedTags.append(typeCategory)
    dd.link = ''/ContributionsJsonSearch/{}/{}''.format(''GivingTypesData'', NamePriority)
    tag = model.CreateContributionTag(TagName, searchParameters)
    dd.results = GetResults(TagName)
    GivingTypesData.AddValue(NamePriority, dd)

def CreateTags(fundSet):
    RowType = ''Online''                              # Name this set of data
    SearchParameters = model.DynamicData(Base)      # don''t change
    SearchParameters.Priority = NextPriority()      # don''t change
    SearchParameters.BundleTypes = ''Online''         # name of type you want to retrieve data for
    CreateTag(fundSet, RowType, SearchParameters)   # don''t change

    RowType = ''Cash''                                        # Name this set of data
    SearchParameters = model.DynamicData(Base)              # don''t change
    SearchParameters.Priority = NextPriority()              # don''t change
    SearchParameters.BundleTypes = ''Loose Checks and Cash''  # name of type you want to retrieve data for
    CreateTag(fundSet, RowType, SearchParameters)           # don''t change
    
    RowType = ''Envelopes''                                   # Name this set of data
    SearchParameters = model.DynamicData(Base)              # don''t change
    SearchParameters.Priority = NextPriority()              # don''t change
    SearchParameters.BundleTypes = ''Preprinted Envelopes''   # name of type you want to retrieve data for
    CreateTag(fundSet, RowType, SearchParameters)           # don''t change
    
Priority=100 
CreateTags(''MainFund'')
Data.days = Days
Data.results = model.FormatJson(GivingTypesData)
addcolumn = "data.addColumn(''number'', ''{}'');"
alist = [addcolumn.format(x) for x in selectedTags]
Data.addcolumns = ''\n''.join(alist)
print model.RenderTemplate(Data.HTMLContent)',
           GETDATE(),5,0,0,0,'admin')

INSERT INTO [dbo].[ContentKeyWords]
           ([Id],[Word])
     VALUES
           ((select SCOPE_IDENTITY()),'widget')
INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetGivingTypeBreakdownSQL','Edit Sql Script',
           'set datefirst 1;
with weeks as (
select DATEPART(week, getdate()) w UNION ALL
select DATEPART(week, getdate() - 7) w UNION ALL
select DATEPART(week, getdate() - 14) w UNION ALL
select DATEPART(week, getdate() - 21) w UNION ALL
select DATEPART(week, getdate() - 28) w UNION ALL
select DATEPART(week, getdate() - 35) w UNION ALL
select DATEPART(week, getdate() - 42) w UNION ALL
select DATEPART(week, getdate() - 49) w 
),
tags as (
select weeks.w, SUM(coalesce(c.ContributionAmount,0)) as amount, TagName from weeks
cross join ContributionTag ct 
left join Contribution c on datepart(week, c.ContributionDate) = weeks.w and c.ContributionId = ct.ContributionId and c.ContributionTypeId NOT IN (6,7,8)
where TagName = @Tag
group by weeks.w, TagName
)
select * from tags
order by w',
           GETDATE(),4,0,0,0,'admin')

INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Giving Type Breakdown' and [System] = 1) = 0
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
           ('Giving Type Breakdown'
           ,'Displays breakdown of giving for a specific fund based on giving types'
           ,(select max(Id) from Content where [Name] like 'WidgetGivingTypeBreakdownHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingTypeBreakdownPython')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingTypeBreakdownSQL')
           ,0
           ,(select isnull(max([Order]), 0)+1 from DashboardWidgets)
           ,1
           ,1
           ,6)

INSERT INTO [dbo].[DashboardWidgetRoles]
           ([WidgetId]
           ,[RoleId])
    SELECT SCOPE_IDENTITY() [WidgetId], RoleId FROM dbo.Roles WHERE RoleName in ('Finance', 'FinanceAdmin', 'Admin')
END
GO
