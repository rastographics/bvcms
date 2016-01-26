# This script will produce a report showing various divisions of Sunday School classes
# and total numbers of guests for each for a given year starting on a date.

# first we define a query that will select the New Guests for life groups for a particular division and date range.
query = '''
    AttendTypeAsOf( Prog=101[Life Groups], Div={}, StartDate='{}', EndDate='{}' ) = 60[New Guest]
    AND IncludeDeceased = 1[True]
'''
# The above query has 3 placeholders (they look like this: {} ).
# The first one is for the Division, the other two are for the start date and end date.
# They will be replaced with different values for each row/column of the report
# using the pattern: rowcolquery = query.format(div, startdt, enddt)
# where query2 has ready to execute string using QueryCodeCount(query2)

# this is a list of starting dates and each will become a column in your report
dates = [
	"7/1/2007",
	"7/1/2008",
	"7/1/2009",
	"7/1/2010",
	"7/1/2011",
	"7/1/2012",
	"7/1/2013",
	"7/1/2014",
	"7/1/2015"
]

# the following is a list of divisions you want in your report
# and can be retrieved on your database from https://mychurch.tpsdb.com/DivisionCodes.
# Each division will become a single row in your report.
divisions = [
    "201[Younger Pre-School]",
    "202[Older Pre-School]",
    "239[Special Needs]",
    "203[Children Grades 1-3]",
    "6450[Grades 4-5]",
    "6451[Middle School]",
    "6452[High School]",
    "205[College]",
    "240[Young Adult Singles]",
    "210[Young Couples]",
    "211[Young Marrieds]",
    "212[Adult 1]",
    "213[Adult 2]",
    "214[Adult 3]",
    "215[Senior Adults]",
    "6477[Off Campus Ministries]"
]

# a row object has a name and a list of columns
class Row:
    def __init__(self, name):
        self.name = name
        self.cols = []

# Data is a built-in, dynamic object
Data.rows = [] # create a list of rows

# the Row constructor is passed the name when creating an instance
# we first create the header and footer rows
Data.header = Row("Divisions")
Data.footer = Row("Totals")

# now we initialize the header and footer rows
for startdt in dates:
    Data.header.cols.append(startdt) # put the start date in the header column
    Data.footer.cols.append(0) # initialize each total row cell to 0

# now build all the rows between header and footer
for div in divisions:
    name = div.split('[')[1].strip(']'); # strip the name from this pattern: 123[this is the name]
    row = Row(name)
    for startdt in dates:
        enddt = model.DateAddDays(startdt, 365)
        rowcolquery = query.format(div, startdt, enddt) # create the query needed for this division/date
        count = q.QueryCodeCount(rowcolquery) # execute the query and get the # people
        i = len(row.cols) # this is the index of the column we are working on
        Data.footer.cols[i] += count # add to total row
        row.cols.append(count)
    Data.rows.append(row)

# This is the template for the HTML table that will hold the results.
template = '''
<h3>Life Group New Guests, FY report (starting dates)</h3>
<table class="table" style="width: auto">
    <thead>
    <tr>
        <th>{{header.name}}</th>
        {{#each header.cols}}
            <td align="right">{{this}}</td>
        {{/each}}
    </tr>
    </thead>
    <tbody>
    {{#each rows}}
    <tr>
        <th>{{name}}</th>
        {{#each cols}}
            <td align="right">{{Fmt this "N0"}}</td>
        {{/each}}
    </tr>
    {{/each}}
    </tbody>
    <tfoot>
    <tr>
        <th>{{footer.name}}</th>
        {{#each footer.cols}}
            <td align="right">{{Fmt this "N0"}}</td>
        {{/each}}
    </tr>
    </tfoot>
</table>
'''

# The dynamic, built-in Data object is passed in by default to RenderTemplate
# the report is printed on the page
print model.RenderTemplate(template)