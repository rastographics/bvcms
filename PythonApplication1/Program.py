import clr
clr.AddReferenceToFileAndPath("../CmsData/bin/Debug/CmsData.dll")
from CmsData import PythonEvents
model = PythonEvents()

q = model.ChangedAddresses()
for v in q:
    model.Email
    print 'Hi {} {}, \nI noticed you have moved to {}\n'.format(v.FirstName, v.LastName, v.PrimaryCity)



