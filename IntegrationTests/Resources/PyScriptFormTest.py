json = model.DynamicDataFromJson(model.Content('jsondata'))
meta = model.DynamicDataFromJson('''{'Id':'readonly int','Name':'str','City':'str','Work':'str'}''')
form = '''<form action='/PyScriptForm' method='post'>
    {}
    <input id='submitit' type='submit' value='Submit' />
    <input type='hidden' name='pyscript' value='{}' />
    <input type='hidden' name='p1' value='{}' />
    <input type='hidden' name='p2' value='{}' />
</form>
'''

def FindElement(array, id):
    return next((i for i in array if i.Id == int(id)), None)

def DisplayForm(id, records):
    record = FindElement(records, id)
    rows = model.BuildDisplayRows(record, meta)
    return form.format( rows, Data.pyscript, 'edit', id)

def EditForm(id):
    record = FindElement(json.Records, id)
    rows = model.BuildFormRows(record, meta)
    return form.format( rows, Data.pyscript, 'update', id)

def UpdateRecord(id, postdata):
    record = FindElement(json.Records, id)
    for k in record.Keys(meta):
        record.SetValue(k, postdata[k])
    model.WriteContentText('jsondata', str(json))
    return DisplayForm(id, json.Records)

if model.HttpMethod == 'get':
    model.Form = DisplayForm(Data.p2, json.Records)

elif model.HttpMethod == 'post':
    if Data.p1 == 'edit':
        print EditForm(Data.p2)
    elif Data.p1 == 'update':
        print UpdateRecord(Data.p2, Data)
