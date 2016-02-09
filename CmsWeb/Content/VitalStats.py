Data.days = 7

Data.members = q.QueryCount(
"MemberStatusId = 10[Member]")

Data.uniqueAttends = q.QueryCount(
"RecentAttendCount( Days=7 ) > 0")

Data.newAttends = q.QueryCount(
"HasRecentNewAttend( Days=7, NumberOfDaysForNoAttendance='365' ) = 1[True]")

Data.meetings = q.MeetingCount(Data.days, 0, 0, 0)
Data.numPresent = q.NumPresent(Data.days, 0, 0, 0)

Data.decisions = q.QueryCount("""
    RecentDecisionType( Days=7 ) IN ( 
        0[Unknown]
        ,10[POF for Membership]
        ,20[POF NOT for Membership]
        ,30[Letter], 40[Statement] 
        ,50[Stmt requiring Baptism] 
        )""")

Data.contacts = q.QueryCount("""
    RecentContactType( Days=7 ) IN ( 
        4[Card Sent]
        ,5[EMail Sent]
        ,6[Info Pack Sent]
        ,3[Letter Sent]
        ,7[Other]
        ,1[Personal Visit]
        ,2[Phone Call]
        ,99[Unknown] 
        )""")

Data.registrations = q.QueryCount("""
    RecentRegistrationType( Days=7 ) IN ( 
        1[Join Organization]
        , 10[User Selects Organization]
        , 11[Compute Org By Birthday]
        , 15[Manage Subscriptions]
        , 14[Manage Recurring Giving]
        , 8[Online Giving]
        , 9[Online Pledge]
        , 16[Special Script] 
        )""")

fund = 0 # 0 is for all funds

week = 7
weeksinyear = 52
year = weeksinyear * week
oneweekago = week
twoweeksago = week * 2
fiveweeksago = week * 5
oneyearago = year + oneweekago
twoyearsago = year * 2 + oneweekago

Data.cnAmtPrev7 = q.ContributionTotals(twoweeksago, oneweekago, fund)
Data.cnCntPrev7 = q.ContributionCount(twoweeksago, oneweekago, fund)

tcount = q.ContributionCount(oneyearago, oneweekago, fund)
Data.cnAvgAmtPerDonorYear = \
    q.ContributionTotals(oneyearago, oneweekago, fund) \
    / tcount if tcount > 0 else 0

Data.cnWeekly4WeekAvg = \
    q.ContributionTotals(fiveweeksago, oneweekago, fund) / 4

Data.cnWeeklyAvgCurrYear = \
    q.ContributionTotals(oneyearago, oneweekago, fund) / weeksinyear
Data.cnWeeklyAvgPrevYear = \
    q.ContributionTotals(twoyearsago, oneyearago, fund) / weeksinyear

Data.cnDateRangeCurrYear = \
    q.DateRangeForContributionTotals(oneyearago, oneweekago)
Data.cnDateRangePrevYear = \
    q.DateRangeForContributionTotals(twoyearsago, oneyearago)

template = """
<style>
    #vitalStats { width:auto; margin-left:auto; margin-right:auto; } 
    #vitalStats td { text-align: right; }
</style>
<table id="vitalStats" class="table">
    <tr><th colspan="2">Counts for past {{days}} days</th></tr>
    <tr><td>Members</td>
        <td>{{Fmt members "N0"}}</td></tr>
    <tr><td>Decisions</td>                  
        <td>{{Fmt decisions "N0"}}</td></tr>
    <tr><td>Meetings</td>                   
        <td>{{Fmt meetings "N0"}}</td></tr>
    <tr><td>Sum of Present in Meetings</td> 
        <td>{{Fmt numPresent "N0"}}</td></tr>
    <tr><td>Unique Attends</td>             
        <td>{{Fmt uniqueAttends "N0"}}</td></tr>
    <tr><td>New Attends</td>                
        <td>{{Fmt newAttends "N0"}}</td></tr>
    <tr><td>Contacts</td>                   
        <td>{{Fmt contacts "N0"}}</td></tr>
    <tr><td>Registrations</td>              
        <td>{{Fmt registrations "N0"}}</td></tr>

    <tr><th colspan="2">Contributions-All Funds</th></tr>
    <tr><td>Average Gift Size</td>              
        <td>{{Fmt cnAvgAmtPerDonorYear "N2"}}</td</tr>
    <tr><td>Weekly average past 4 weeks</td>    
        <td>{{Fmt cnWeekly4WeekAvg "N2"}}</td></tr>
    <tr><td>Weekly average current year</td>    
        <td>{{Fmt cnWeeklyAvgCurrYear "N2"}}</td>
        <td>{{cnDateRangeCurrYear}}</td></tr>
    <tr><td>Weekly average previous year</td>   
        <td>{{Fmt cnWeeklyAvgPrevYear "N2"}}</td>
        <td>{{cnDateRangePrevYear}}</td></tr>
</table>
"""

print model.RenderTemplate(template)
