-- add Giving to Budget Comparison widget
IF (select count(*) from DashboardWidgets where [Name] like 'Giving to Budget Comparison' and [System] = 1) = 0
BEGIN
  INSERT INTO [dbo].[Content]
           ([Name],[Title],[Body],[DateCreated],[TypeID],[ThumbID],[RoleID],[OwnerID],[CreatedBy])
     VALUES
           ('WidgetGivingToBudgetComparisonHTML','Edit Text Content',
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
        <div class="box-content" style="text-align: -webkit-center;">
            <div class="chart" style="display: inline-block;width:200px;"></div>
            <div class="chartSummary" style="display: inline-block;width:200px;vertical-align: top;margin-top: 50px;"></div>
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
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p1);
        
        let p2 = document.createElement("p");
        p2.innerHTML = "Budget YTD:   $" + Math.round(budgetYTD);
        p2.className = "text-center";
        p2.style = "margin:0px;";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p2);
        
        let p3 = document.createElement("p");
        p3.innerHTML = "Over/Under YTD:   $" + Math.round(overUnderYTD);
        if(overUnderYTD < 0){
            p3.style = "color:red;margin-top:10px;";
        } else{
            p3.style = "margin:0px;";
        }
        p3.className = "text-center";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p3);
        
        let p4 = document.createElement("p");
        p4.innerHTML = "Weekly Average YTD:   $" + Math.round(weeklyAverageYTD);
        p4.className = "text-center";
        p4.style = "margin:0px;";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p4);
        
        let p5 = document.createElement("p");
        p5.innerHTML = "Last 7 Days:   $" + Math.round(givingLastSevenDays);
        p5.className = "text-center";
        p5.style = "margin:0px;";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p5);
        
        var YTDgiving = Math.round(givingTotalYTD);
        var YTDbudget = Math.round(budgetYTD);
        
        
        var data = google.visualization.arrayToDataTable([
          [''Label'', ''Value''],
          ['''', 0]
        ]);
        var formatter = new google.visualization.NumberFormat({
            prefix: ''$'',
            fractionDigits: 1
        });
        formatter.format(data, 1);
        
        var oneThird = (budgetYTD * 0.33)/1000;
        var oneThirdString = ''$'' + oneThird.toFixed(1);
        var twoThird = (budgetYTD * 0.66)/1000;
        var twoThirdString = ''$'' + twoThird.toFixed(1);
        var threeThird = budgetYTD/1000;
        var threeThirdString = ''$'' + threeThird.toFixed(1);
        var options = {
          height: 200, max: (Math.round(YTDbudget*1.33)/1000),
          yellowFrom: (Math.round(YTDbudget*0.665)/1000), yellowTo: (Math.round(YTDbudget*0.85)/1000), yellowColor: ''#F5D762'',
          greenFrom: (Math.round(YTDbudget*0.85)/1000), greenTo: (Math.round(YTDbudget)/1000), greenColor: ''#37968D'',
          redFrom: (Math.round(YTDbudget)/1000), redTo: (Math.round(YTDbudget*1.33)/1000), redColor: ''#757375'',
          majorTicks: ['''',oneThirdString,twoThirdString,threeThirdString,''''],
          animation:{
            duration: 2000,
            easing: ''out'',
          }
        };
        
        var chart = new google.visualization.Gauge(document.querySelector(''#{{WidgetId}}-section .chart''));
        chart.draw(data, options);
        
        givingTotalYTD = givingTotalYTD/1000;
        var data2 = google.visualization.arrayToDataTable([
          [''Label'', ''Value''],
          ['''', givingTotalYTD]
        ]);
        formatter.format(data2, 1);
        
        setInterval(function() {
          chart.draw(data2, options);
        }, 2000);
        
        let p6 = document.createElement("p");
        p6.innerHTML = "(Dollars In Thousands)";
        p6.className = "text-center";
        p6.style = "margin:0px;";
        document.querySelector(''#{{WidgetId}}-section .chart'').appendChild(p6);
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
           ('WidgetGivingToBudgetComparisonPython','Edit Python Script',
           '# Change FundId to select the fund you wish to use for this widget. Don''t delete the quotes.
FundId = ''9''
# Change AnnualBudget to the target budget for this fund for this time frame.
AnnualBudget = 240000 # in dollars
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
           ('WidgetGivingToBudgetComparisonSQL','Edit Sql Script',
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

IF (select count(*) from DashboardWidgets where [Name] like 'Giving to Budget Comparison' and [System] = 1) = 0
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
           ('Giving to Budget Comparison'
           ,'Displays progress of giving against budget by month to date'
           ,(select max(Id) from Content where [Name] like 'WidgetGivingToBudgetComparisonHTML')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingToBudgetComparisonPython')
           ,(select max(Id) from Content where [Name] like 'WidgetGivingToBudgetComparisonSQL')
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
