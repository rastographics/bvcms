import configproxy
import clr
clr.AddReference('System')
clr.AddReference('System.Configuration')
clr.AddReference("CmsData")
from System import *
from System.Text import *
from CmsData import QueryFunctions, PythonEvents, DbUtil, CMSDataContext

configproxy.override('myconfig.config')
dbname = "bellevue"
q = QueryFunctions(dbname)

class VitalStats(object):
    def Run(self, m):
        days = 7
        fmtc = '{0:28}{1:10,.2f}'
        fmti = '{0:28}{1:10,d}'

        print "Counts for past {0} days".format(days)
        print fmti.format("Members", m.QueryCount("Stats:Members"))
        print fmti.format("Decisions", m.QueryCount("Stats:Decisions"))
        print fmti.format("Meetings", m.MeetingCount(days, 0, 0, 0))
        print fmti.format("Sum of Present in Meetings", m.NumPresent(days, 0, 0, 0))
        print fmti.format("Contacts", m.QueryCount("Stats:Contacts"))
        print fmti.format("Registrations", m.RegistrationCount(days, 0, 0, 0))
        print "Contributions"
        print fmtc.format("Amount Previous 7 days", m.ContributionTotals(7*2, 7, 0))
        print fmti.format("Count Previous 7 days", m.ContributionCount(7*2, 7, 0))
        print fmtc.format("Average per Capita Year", \
            m.ContributionTotals(53*7, 7, 0) / m.ContributionCount(53*7, 7, 0))
        print fmtc.format("Weekly 4 week average", m.ContributionTotals(7*5, 7, 0) / 4)
        print fmtc.format("Weekly average past 52wks", m.ContributionTotals(53*7, 7, 0) / 52)
        print fmtc.format("Weekly average prev 52wks", \
            m.ContributionTotals(53*7*2, 53*7+7, 0) / 52)

VitalStats().Run(q)

Console.ReadKey(True)