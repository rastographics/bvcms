model.Header = 'Test Chart'

data = q.SqlNameCountArray("test",
'''
	SELECT ms.Description Name, COUNT(*) Cnt
	FROM dbo.People p
	JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
	GROUP BY ms.Description
''')

model.Script = 
'''
    <script type='text/javascript' src='https://www.google.com/jsapi'></script>
    <script type='text/javascript'>
      google.load('visualization', '1', {packages:['corechart']});
      google.setOnLoadCallback(drawChart);
      function drawChart() {
        var data = google.visualization.arrayToDataTable(@data);

        var options = {
          title: 'Member Status',
          legend: 'none',
          pieSliceText: 'label',
          slices: {  4: {offset: 0.2},
                    12: {offset: 0.3},
                    14: {offset: 0.4},
                    15: {offset: 0.5}
          },
        };

        var chart = new google.visualization.PieChart(document.getElementById('piechart'));
        chart.draw(data, options);
      }
    </script>
'''.replace("@data", data)

print "<div id='piechart' style='width: 900px; height: 500px;'></div>"