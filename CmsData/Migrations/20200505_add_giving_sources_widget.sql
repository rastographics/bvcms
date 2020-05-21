-- add giving sources widget
IF (select count(*) from DashboardWidgets where [Name] like 'Giving Sources' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],
           [Body],
           [DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetGivingSourcesHTML','Edit Text Content',
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
            <h4 class="text-center">{{fund}}</h4>
            <div class="chart">
            </div>
            <p class="text-center" style="margin-top:10px;">Giving past {{days}} days</p>
        </div>
    </div>
</div>
<script type="text/javascript">
    var {{WidgetId}} = function() {
        var data = {{{results}}};
        var totalOnline = 0;
        data = Object.values(data).map(function(item) {
            // add up online giving to find the "Other" amount
            if (item.name != ''All'') totalOnline += item.results[0].Total;
            return [item.name, item.results[0].Total]
        }).filter(function(item) {
            return item[1] > 0
        });
        // convert All row to Other
        for(var i = 0; i < data.length; i++) {
            if (data[i][0] == ''All'') {
                data[i][0] = ''Other'';
                data[i][1] -= totalOnline;
                break;
            }
        }
        data = [[''Item'', ''Dollars'']].concat(data);
        data = google.visualization.arrayToDataTable(data);
        var formatter = new google.visualization.NumberFormat({
            prefix: ''$'',
            fractionDigits: 0
        });
        formatter.format(data, 1);
        var options = {
            pieSliceText: ''percentage'',
            legend: {
                position: ''bottom''
            },
            chartArea: {
                left: 0,
                top: 20,
                bottom: 20,
                width: ''100%'',
                height: ''100%''
            }
        };
        
        var chart = new google.visualization.PieChart(document.querySelector(''#{{WidgetId}}-section .chart''));
        chart.draw(data, options);
    }
    // load and register the chart
    google.charts.load("current", {packages:["corechart"]});
    google.charts.setOnLoadCallback({{WidgetId}});
    WidgetCharts.{{WidgetId}} = {{WidgetId}};
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
           ('WidgetGivingSourcesPython','Edit Python Script',
           'from System import DateTime
FundIds = ''1''
FundName = ''General Fund''
Days = 30

# Initialize global data
MaxDate = DateTime.Today
GivingTypesData = model.DynamicData()
Priority = 0
Base = model.DynamicData()
Base.MinDate = MaxDate.AddDays(-Days).ToString("d")
# Base.MinDate = DateTime(MaxDate.Year, 1, 1).ToString("d") # YTD
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
    dd.link = ''/ContributionsJsonSearch/{}/{}''.format(''GivingTypesData'', NamePriority)
    tag = model.CreateContributionTag(TagName, searchParameters)
    dd.results = GetResults(TagName)
    GivingTypesData.AddValue(TagName, dd)
    
def CreateTags(fundSet):
    RowType = ''Mobile'' 
    SearchParameters = model.DynamicData(Base)
    SearchParameters.Priority = NextPriority()
    SearchParameters.Source = 1 # Mobile
    CreateTag(fundSet, RowType, SearchParameters)
    RowType = ''One Time''
    SearchParameters = model.DynamicData(Base)
    SearchParameters.Priority = NextPriority()
    SearchParameters.BundleTypes = ''Online''
    SearchParameters.TransactionDesc = ''<>Recurring Giving''
    SearchParameters.Source = 0 # Not Mobile
    CreateTag(fundSet, RowType, SearchParameters)
    RowType = ''Recurring''
    SearchParameters = model.DynamicData(Base)
    SearchParameters.BundleTypes = ''Online''
    SearchParameters.Priority = NextPriority()
    SearchParameters.Source = 0 # Not Mobile
    SearchParameters.TransactionDesc = ''Recurring Giving''
    CreateTag(fundSet, RowType, SearchParameters)
    RowType = ''All'' # row used to determine the non online gifts later
    SearchParameters = model.DynamicData(Base)
    SearchParameters.Priority = NextPriority()
    CreateTag(fundSet, RowType, SearchParameters)
    
Priority=100 
CreateTags(''MainFund'')
Data.days = Days
Data.fund = FundName
Data.results = model.FormatJson(GivingTypesData)
print model.RenderTemplate(Data.HTMLContent)',
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
           ('WidgetGivingSourcesSQL','Edit Sql Script',
           'select isnull(sum(ContributionAmount),0) as Total, count(*) as Contributions
from ContributionTag ct
join Contribution c on ct.ContributionId = c.ContributionId
where TagName = @Tag',
           GETDATE(),4,0,0,0,'admin')
           
INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Giving Sources' and [System] = 1) = 0
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
           ('Giving Sources'
           ,'Shows a pie chart with online giving sources and their share of overall online giving'
           ,(select max(Id) from Content where [Name] like 'WidgetGivingSourcesHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingSourcesPython')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingSourcesSQL')
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
