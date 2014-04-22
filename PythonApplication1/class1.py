import System
import System.Text
from System import *
from System.Text import *

import clr
clr.AddReferenceByName("CmsData")
from CmsData import QueryFunctions



class VitalStats(object):
    def Run(self, m):
        days = 7
        #fmt = '<tr><td align="right">{0}:</td><td align="right">{1:n0}</td></tr>\r\n'
        #fmt0 = '<tr><td colspan="2">{0}\r\n'
        fmt = '{0,28}:{1,10:n0}\r\n'
        fmt0 = '{0}\r\n'
        sb = StringBuilder()

        sb.AppendLine('<table cellspacing="5" class="grid">')
        sb.AppendFormat(fmt0, String.Format("Counts for past {0} days", days))
        sb.AppendFormat(fmt, "Members", m.QueryCount("Stats:Members"))
        sb.AppendFormat(fmt, "Decisions", m.QueryCount("Stats:Decisions"))
        sb.AppendFormat(fmt, "Meetings", m.MeetingCount(days, 0, 0, 0))
        sb.AppendFormat(fmt, "Sum of Present in Meetings", m.NumPresent(days, 0, 0, 0))
        sb.AppendFormat(fmt, "Unique Attends", m.QueryCount("Stats:Attends"))
        sb.AppendFormat(fmt, "New Attends", m.QueryCount("Stats:New Attends"))
        sb.AppendFormat(fmt, "Contacts", m.QueryCount("Stats:Contacts"))
        sb.AppendFormat(fmt, "Registrations", m.RegistrationCount(days, 0, 0, 0))
        sb.AppendFormat(fmt0, "Contributions")
        sb.AppendFormat(fmt, "Amount Previous 7 days", m.ContributionTotals(7*2, 7, 0))
        sb.AppendFormat(fmt, "Count Previous 7 days", m.ContributionCount(7*2, 7, 0))
        sb.AppendFormat(fmt, "Average per Capita Year", \
            m.ContributionTotals(53*7, 7, 0) / m.ContributionCount(53*7, 7, 0))
        sb.AppendFormat(fmt, "Weekly 4 week average", m.ContributionTotals(7*5, 7, 0) / 4)
        sb.AppendFormat(fmt, "Weekly average past 52wks", m.ContributionTotals(53*7, 7, 0) / 52)
        sb.AppendFormat(fmt, "Weekly average prev 52wks", \
            m.ContributionTotals(53*7*2, 53*7+7, 0) / 52)
        sb.AppendLine('</table>')
        return sb.ToString()

m = QueryFunctions()
vs = VitalStats()
ret = vs.Run(m)

Console.Write(ret);
Console.Write('press any key')
Console.ReadKey(True)