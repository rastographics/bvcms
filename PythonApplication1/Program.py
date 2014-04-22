import System
import System.Text
from System import *
from System.Text import *
import clr
clr.AddReferenceByName("CmsData")
from CmsData import PythonEvents
from CmsData import QueryFunctions
dbname = "2pc"
model = PythonEvents(dbname)
q = QueryFunctions(dbname)
model.CmsHost = "http://localhost:5000"
model.SmtpDebug = True

### THE ABOVE IS ONLY NECESSARY FOR RUNNING DIRECTLY FROM AN IRON PYTHON PROJECT
### BELOW IS THE START OF YOUR CODE THAT YOU WILL HAVE IN YOUR SCRIPT

CC = 1110 # ProgramId
Adults = 23 # DivisionId
Sender = 3 # PeopleId of sender (admin staff, the QueuedBy person)

model.TestEmail = True # cause all emails to go to the Sender
model.Transactional = True

FromEmail = 'robb@2pc.org' 
FromName = 'Robb Roaten' 

MemberTypes = 'Head Elder,Head Deacon'

Message = 'EMM-Elder-Deacon-Notice' # name of special content email message to send


OrganizationIds = model.OrganizationIds(CC, Adults)

for OrgId in OrganizationIds:
    QueryId = model.OrgMembersQuery(CC, Adults, OrgId, MemberTypes)
    # set the context for the email replacment codes {extraorg:EMMQuery} and {orgname}
    model.CurrentOrgId = OrgId 
    model.EmailContent2(QueryId, Sender, FromEmail, FromName, Message)





