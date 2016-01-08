Data.days = 7
fund = 0 # 0=all funds

Data.members = q.QueryCodeCount(
    "MemberStatusId = 10[Member]")

Data.uniqueAttends = q.QueryCodeCount(
    "RecentAttendCount( Days=7 ) > 0")

Data.newAttends = q.QueryCodeCount(
    "RecentNewVisitCount( Days=7, NumberOfDaysForNoAttendance='365' ) > 0")

Data.meetings = q.MeetingCount(Data.days, 0, 0, 0)
Data.numPresent = q.NumPresent(Data.days, 0, 0, 0)

Data.decisions = q.QueryCodeCount("""
    RecentDecisionType( Days=7 ) IN ( 
        0[Unknown]
        ,10[POF for Membership]
        ,20[POF NOT for Membership]
        ,30[Letter], 40[Statement] 
        ,50[Stmt requiring Baptism] 
        )""")

Data.contacts = q.QueryCodeCount("""
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

Data.registrations = q.QueryCodeCount("""
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

Data.cnAmtPrev7 = q.ContributionTotals(7*2, 7, fund)
Data.cnCntPrev7 = q.ContributionCount(7*2, 7, fund)
tcount = q.ContributionCount(53*7, 7, fund)
Data.cnAvgAmtPerDonorYear = q.ContributionTotals(53*7, 7, fund) / tcount if tcount > 0 else 0
Data.cnWeekly4WeekAvg = q.ContributionTotals(7*5, 7, fund) / 4
Data.cnWeeklyAvgCurrYear = q.ContributionTotals(53*7, 7, fund) / 52
Data.cnWeeklyAvgPrevYear = q.ContributionTotals(53*7*2, 53*7+7, fund) / 52

template = """
<style>
    #vitalStats { width:auto; margin-left:auto; margin-right:auto; } 
    #vitalStats td { text-align: right; }
</style>
<table id="vitalStats" class="table">
    <tr><th colspan="2">Counts for past {{days}} days</th></tr>
    <tr><td>Local Members</td>              <td>{{Fmt members "N0"}}</td></tr>
    <tr><td>Decisions</td>                  <td>{{Fmt decisions "N0"}}</td></tr>
    <tr><td>Meetings</td>                   <td>{{Fmt meetings "N0"}}</td></tr>
    <tr><td>Sum of Present in Meetings</td> <td>{{Fmt numPresent "N0"}}</td></tr>
    <tr><td>Unique Attends</td>             <td>{{Fmt uniqueAttends "N0"}}</td></tr>
    <tr><td>New Attends</td>                <td>{{Fmt newAttends "N0"}}</td></tr>
    <tr><td>Contacts</td>                   <td>{{Fmt contacts "N0"}}</td></tr>
    <tr><td>Registrations</td>              <td>{{Fmt registrations "N0"}}</td></tr>

    <tr><th colspan="2">Contributions-Budget and Love Offering</th></tr>
    <tr><td>Average per Capita Year</td>     <td>{{Fmt cnAvgAmtPerDonorYear "N2"}}</td</tr>
    <tr><td>Weekly 4 week average</td>          <td>{{Fmt cnWeekly4WeekAvg "N2"}}</td></tr>
    <tr><td>Weekly average current year</td>    <td>{{Fmt cnWeeklyAvgCurrYear "N2"}}</td></tr>
    <tr><td>Weekly average previous year</td>   <td>{{Fmt cnWeeklyAvgPrevYear "N2"}}</td></tr>
</table>
"""

print model.RenderTemplate(template)