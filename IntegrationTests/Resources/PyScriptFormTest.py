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

def FindElement(array, ident):
    return next((i for i in array if i.Id == int(ident)), None)

def DisplayForm(ident, records):
    record = FindElement(records, ident)
    rows = model.BuildDisplayRows(record, meta)
    return form.format( rows, Data.pyscript, 'edit', ident)

def EditForm(ident):
    record = FindElement(json.Records, ident)
    rows = model.BuildFormRows(record, meta)
    return form.format( rows, Data.pyscript, 'update', ident)

def UpdateRecord(ident, postdata):
    record = FindElement(json.Records, ident)
    for k in record.Keys(meta):
        record.SetValue(k, postdata[k])
    model.WriteContentText('jsondata', str(json))
    return DisplayForm(ident, json.Records)

if model.HttpMethod == 'get':
    model.Form = DisplayForm(Data.p2, json.Records)

elif model.HttpMethod == 'post':
    if Data.p1 == 'edit':
        print EditForm(Data.p2)
    elif Data.p1 == 'update':
        print UpdateRecord(Data.p2, Data)
