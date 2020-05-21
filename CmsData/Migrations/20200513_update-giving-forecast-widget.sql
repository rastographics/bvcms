UPDATE [dbo].[Content]
SET [Body] = 
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
        <div class="box-content center">
            <h4 class="text-center">{{fund}}</h4>
            <div class="chart">
            </div>
            <p class="text-center" style="margin-top:10px;">Based on annual budget of ${{FmtNumber budget}}</p>
        </div>
    </div>
</div>
<script type="text/javascript">
    var {{WidgetId}} = function() {
        var data = {{{results}}};
        var budget = {{budget}};
        var projected = Math.round(data.Combined.MonthlyAmt * 12);
        data = google.visualization.arrayToDataTable([
            [''Item'', ''Dollars''],
            [''Projected Monthly Recurring'', projected],
            [''Remaining Budget'', budget - projected]
        ]);
        var formatter = new google.visualization.NumberFormat({
            prefix: ''$'',
            fractionDigits: 0
        });
        formatter.format(data, 1);
        var options = {
            pieHole: 0.4,
            legend: ''none'',
            pieSliceText: ''percentage'',
            colors: [''#3366CC'',''#DC3912''],
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
</script>'
WHERE HASHBYTES('MD5', [Body]) = 0x7FA9A01573E6600A2AF6C78FF96B7AE0
AND [Name] = 'WidgetGivingForecastHTML'