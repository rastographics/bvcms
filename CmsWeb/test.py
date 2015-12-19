query = '''
    AttendTypeAsOf( Prog=101[Life Groups], Div={}, StartDate='{}', EndDate='{}' ) = 60[New Guest]
    AND IncludeDeceased = 1[True]
'''
# The above query has 3 placeholders: {}
# which will be replaced with different values for each row/column
# using the pattern: query2 = query.format(div, startdt, enddt)
# where query2 has ready to execute string using QueryCodeCount(query2)

# the following can be retrieved on your database from /DivisionCodes
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
# this is a list of starting dates for each column
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

# a row object has a name and a list of columns
class Row:
    def __init__(self, name):
        self.name = name
        self.cols = []

# Data is a built-in dynamic object
Data.rows = [] # create a list of rows

# the Row constructor is passed the name when creating an instance
Data.header = Row("Divisions")
Data.footer = Row("Totals")

# initialize the header and footer rows
for startdt in dates:
    Data.header.cols.append(startdt)
    Data.footer.cols.append(0) # initialize the total to 0

# build all the rows between header and footer
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

# Data is passed by default to RenderTemplate
print model.RenderTemplate(template)