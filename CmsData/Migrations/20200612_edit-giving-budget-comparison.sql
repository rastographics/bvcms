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
        <div class="box-content" style="text-align: -webkit-center;">
            <div class="chart" style="display: inline-block;width:200px;"></div>
            <div class="chartSummary" style="display: inline-block;width:200px;vertical-align: top;margin-top: 50px;"></div>
        </div>
    </div>
</div>
<script type="text/javascript">
    var {{WidgetId}} = function() {
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').innerHTML = "";
        var weekOfYear = getWeekNumber(new Date());
        var newDate = new Date();
        newDate.setFullYear({{startyear}});
        newDate.setDate({{startday}});
        newDate.setMonth({{startmonth}} - 1);
        var chosenWeekOfYear = getWeekNumber(newDate);
        var data = {{{results}}};
        var budget = {{{annualbudget}}};
        var givingTotalYTD = 0;
        if(newDate.getFullYear() < today.getFullYear()) {
            if(newDate.getFullYear() === (today.getFullYear() - 1)){
                var numWeeksLeftLastYear = 52 - chosenWeekOfYear[1];
                var weekMultiplier = currentWeekOfYear[1] + numWeeksLeftLastYear;
            }
        }
        else if (newDate.getFullYear() === today.getFullYear()) {
            var weekMultiplier = currentWeekOfYear[1] - chosenWeekOfYear[1];
        }
        var budgetYTD = ((budget/52)*weekMultiplier);
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
        var overUnderYTD = givingTotalYTD - budgetYTD;
        var weeklyAverageYTD = (givingTotalYTD/weekMultiplier)
        var myCurrencyFormatter = new Intl.NumberFormat(''en-US'', {
          style: ''currency'',
          currency: ''USD'',
        });
        
        let p1 = document.createElement("p");
        p1.innerHTML = "Total Given YTD: " + myCurrencyFormatter.format(givingTotalYTD);
        p1.className = "text-center";
        p1.style = "margin:0px;text-align:right;font-size:13px;";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p1);
        
        let p2 = document.createElement("p");
        p2.innerHTML = "Budget YTD: " + myCurrencyFormatter.format(budgetYTD);
        p2.className = "text-center";
        p2.style = "margin:0px;text-align:right;font-size:13px;";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p2);
        
        let p3 = document.createElement("p");
        p3.innerHTML = "Over/Under YTD: " + myCurrencyFormatter.format(overUnderYTD);
        if(overUnderYTD < 0){
            p3.style = "color:red;margin:0px;text-align:right;font-size:13px;";
        } else{
            p3.style = "margin:0px;text-align:right;font-size:13px;";
        }
        p3.className = "text-center";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p3);
        
        let p4 = document.createElement("p");
        p4.innerHTML = "Weekly Average YTD: " + myCurrencyFormatter.format(weeklyAverageYTD);
        p4.className = "text-center";
        p4.style = "margin:0px;text-align:right;font-size:13px;";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p4);
        
        let p5 = document.createElement("p");
        p5.innerHTML = "Last 7 Days: " + myCurrencyFormatter.format(givingLastSevenDays);
        p5.className = "text-center";
        p5.style = "margin:0px;text-align:right;font-size:13px;";
        document.querySelector(''#{{WidgetId}}-section .chartSummary'').appendChild(p5);
        
        var YTDgiving = Math.round(givingTotalYTD);
        var YTDbudget = Math.round(budgetYTD);
        
        
        var data = google.visualization.arrayToDataTable([
          [''Label'', ''Value''],
          ['''', 0]
        ]);
        var googleNumFormatter = new google.visualization.NumberFormat({
            prefix: ''$'',
            fractionDigits: 1
        });
        googleNumFormatter.format(data, 1);
        
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
        googleNumFormatter.format(data2, 1);
        
        setInterval(function() {
          chart.draw(data2, options);
        }, 2000);
        
        let p6 = document.createElement("p");
        p6.innerHTML = "(Dollars In Thousands)";
        p6.className = "text-center";
        p6.style = "margin:0px;";
        document.querySelector(''#{{WidgetId}}-section .chart'').appendChild(p6);
    }
    function getWeekNumber(d) {
        // Copy date so don''t modify original
        d = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate()));
        // Set to nearest Thursday: current date + 4 - current day number
        // Make Sunday''s day number 7
        d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay()||7));
        // Get first day of year
        var yearStart = new Date(Date.UTC(d.getUTCFullYear(),0,1));
        // Calculate full weeks to nearest Thursday
        var weekNo = Math.ceil(( ( (d - yearStart) / 86400000) + 1)/7);
        // Return array of year and week number
        return [d.getUTCFullYear(), weekNo];
    }
    // load and register the chart
    google.charts.load("current", {packages:["gauge"]});
    google.charts.setOnLoadCallback({{WidgetId}});
    WidgetCharts.{{WidgetId}} = {{WidgetId}};
</script>'
where Name like 'WidgetGivingToBudgetComparisonHTML'

update Content
set Body = '# Change FundId to select the fund you wish to use for this widget. Don''t delete the quotes.
FundId = ''9''
# Change AnnualBudget to the target budget for this fund for this time frame.
AnnualBudget = 240000 # in dollars
# Change these start date variables to the beginning of time frame you wish to keep track of.
# date format = month/day/year
startMonth = "01"
startDay = "01"
startYear = "2020"


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
    StartDate = startMonth + "/" + startDay + "/" + startYear
    Data.results = GetData(FundId, StartDate)
    Data.dayofyear = datetime.now().timetuple().tm_yday
    Data.startmonth = startMonth
    Data.startday = startDay
    Data.startyear = startYear
    last7days = 0
    print model.RenderTemplate(template)
    
Get()'
where Name like 'WidgetGivingToBudgetComparisonPython'

update Content
set Body = 'select ContributionAmount, ContributionDate
from Contribution
where FundId = @FundId and ContributionStatusId = 0 and ContributionDate >= ''@StartDate'' and ContributionTypeId NOT IN (6,7,8);'
where Name like 'WidgetGivingToBudgetComparisonSQL'
