-- add Giving Metrics widget
IF (select count(*) from DashboardWidgets where [Name] like 'Giving Metrics' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetGivingMetricsHTML','Edit Text Content',
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
        <div class="box-content">
            <div class="chart" style="display: inline-block;">
            </div>
            <div id="chartSummary" style="display: inline-block;width: 250px;vertical-align: top;margin-top: 50px;"></div>
        </div>
    </div>
</div>
<script type="text/javascript">
    var myData = [];
    var {{WidgetId}} = function() {
        var data = {{{results}}};
        myData = data;
        var budget = {{{annualbudget}}};
        var weekOfYear = Math.round({{{dayofyear}}} / 7);
        var givingTotalYTD = 0;
        var budgetYTD = ((budget/52)*weekOfYear);
        var sevenDayaAgo = new Date();
        sevenDayaAgo.setDate(sevenDayaAgo.getDate() - 7);
        var givingLastSevenDays = 0;
        
        for(let i = 0; i < data.length; i++){
            givingTotalYTD += data[i][0];
            var d = new Date(data[i][1]);
            if(d > sevenDayaAgo){
                givingLastSevenDays += data[i][0];
            }
        }
        var overUnderYTD = budgetYTD - givingTotalYTD;
        var weeklyAverageYTD = (givingTotalYTD/weekOfYear)
        
        let p1 = document.createElement("p");
        p1.innerHTML = "Total Given YTD:   $" + Math.round(givingTotalYTD);
        p1.className = "text-center";
        p1.style = "margin:0px;";
        document.getElementById("chartSummary").appendChild(p1);
        
        let p2 = document.createElement("p");
        p2.innerHTML = "Budget YTD:   $" + Math.round(budgetYTD);
        p2.className = "text-center";
        p2.style = "margin:0px;";
        document.getElementById("chartSummary").appendChild(p2);
        
        let p3 = document.createElement("p");
        p3.innerHTML = "Over/Under YTD:   $" + Math.round(overUnderYTD);
        if(overUnderYTD < 0){
            p3.style = "color:red;margin-top:10px;";
        } else{
            p3.style = "margin:0px;";
        }
        p3.className = "text-center";
        document.getElementById("chartSummary").appendChild(p3);
        
        let p4 = document.createElement("p");
        p4.innerHTML = "Weekly Average YTD:   $" + Math.round(weeklyAverageYTD);
        p4.className = "text-center";
        p4.style = "margin:0px;";
        document.getElementById("chartSummary").appendChild(p4);
        
        let p5 = document.createElement("p");
        p5.innerHTML = "Last 7 Days:   $" + Math.round(givingLastSevenDays);
        p5.className = "text-center";
        p5.style = "margin:0px;";
        document.getElementById("chartSummary").appendChild(p5);
        
        var YTDgiving = Math.round(givingTotalYTD);
        var YTDbudget = Math.round(budgetYTD);
        
        var data = google.visualization.arrayToDataTable([
          [''Label'', ''Value''],
          [''Giving'', 0]
        ]);

        var options = {
          height: 200, max: Math.round(budgetYTD),
          redFrom: 0, redTo: Math.round(budgetYTD*0.33),
          yellowFrom:Math.round(budgetYTD*0.33), yellowTo: Math.round(budgetYTD*0.67),
          greenFrom: Math.round(budgetYTD*0.67), greenTo: Math.round(budgetYTD),
          animation:{
            duration: 20000,
            easing: ''out'',
          }
        };
        
        var chart = new google.visualization.Gauge(document.querySelector(''#{{WidgetId}}-section .chart''));
        chart.draw(data, options);
        
        setInterval(function() {
          data.setValue(0, 1, Math.round(givingTotalYTD));
          chart.draw(data, options);
        }, 2000);
    }
    // load and register the chart
    google.charts.load("current", {packages:["gauge"]});
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
           ('WidgetGivingMetricsPython','Edit Python Script',
           '# Change FundId to select the fund you wish to use for this widget. Don''t delete the quotes.
FundId = ''9''
# Change AnnualBudget to the target budget for this fund for this time frame.
AnnualBudget = 24000 # in dollars
# Change StartDate to the beginning of time frame you wish to keep track of.
StartDate = ''01/01/2020''


from datetime import datetime

def GetData(FundId, StartDate):
    sql = Data.SQLContent
    sql = sql.replace(''@FundId'', FundId)
    sql = sql.replace(''@StartDate'', StartDate)
    return q.QuerySqlJsonArray(sql)
    
def Get():
    sql = Data.SQLContent
    template = Data.HTMLContent
    Data.annualbudget = AnnualBudget
    Data.results = GetData(FundId, StartDate)
    Data.dayofyear = datetime.now().timetuple().tm_yday
    last7days = 0
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
           ('WidgetGivingMetricsSQL','Edit Sql Script',
           'select ContributionAmount, ContributionDate from Contribution where FundId = @FundId and ContributionDate >= ''@StartDate'';',
           GETDATE(),4,0,0,0,'admin')

INSERT INTO [dbo].[ContentKeyWords]
           ([Id]
           ,[Word])
     VALUES
           ((select SCOPE_IDENTITY())
           ,'widget')           
END
GO

IF (select count(*) from DashboardWidgets where [Name] like 'Giving Metrics' and [System] = 1) = 0
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
           ('Giving Metrics'
           ,'Displays progress of giving against budget by month to date'
           ,(select max(Id) from Content where [Name] like 'WidgetGivingMetricsHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingMetricsPython')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingMetricsSQL')
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
