model.Header = 'Morning Worship'

data = q.SqlNameCountArray("Sunday", '''
	SELECT CONVERT(VARCHAR, ss, 111) Name, SUM(HeadCount) Cnt
	FROM (
		SELECT 
			HeadCount, 
			dbo.SundayForDate(MeetingDate) ss, 
			DATEDIFF(HOUR, dbo.SundayForDate(MeetingDate), MeetingDate) hh
		FROM dbo.Meetings m
		WHERE EXISTS(	SELECT NULL 
						FROM dbo.DivOrg d 
						JOIN dbo.ProgDiv p ON p.DivId = d.DivId 
						WHERE d.OrgId = m.OrganizationId AND p.ProgId = 1106 )
	) tt
	WHERE tt.hh > 1 AND tt.hh <= 12 AND ss >= '1/1/06' AND tt.HeadCount > 0
	GROUP BY ss
	ORDER BY ss
''')

model.Script = '''
    <script type='text/javascript' src='https://www.google.com/jsapi'></script>
    <script type='text/javascript'>
      google.load('visualization', '1', {packages:['corechart']});
      google.setOnLoadCallback(drawChart);
      function drawChart() {
        var data = google.visualization.arrayToDataTable(@data);

        var options = {
          title: 'Sunday Morning Worship',
          legend: 'none',
          pieSliceText: 'label',
          slices: {  4: {offset: 0.2},
                    12: {offset: 0.3},
                    14: {offset: 0.4},
                    15: {offset: 0.5}
          },
        };

        var chart = new google.visualization.LineChart(document.getElementById('chart'));
        chart.draw(data, options);
      }
    </script>
'''.replace("@data", data)

print "<div id='chart' style='width: 900px; height: 500px;'></div>"