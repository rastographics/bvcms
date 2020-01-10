import re

def Get():
    model.Form = '''
<!DOCTYPE html>
<form enctype="multipart/form-data" action="InstallPyScriptProject" method=post>
<p><label for=zip>File: <input type=file name=zip>
<p><input type=submit>
</form>
'''

def Post(data):
    zipfile = data['zip']
    keyword = zipfile["keyword"]
    Install = None
    print('<pre>')
    keys = list(zipfile.Keys())
    keys.sort()
    for key in keys:
        text = zipfile[key]
        if key == "keyword":
            continue
        if key == "Install.py":
            Install = text
            continue

        if key.endswith(".view.sql"):
            key = re.search("\d*(.*)\.view\.sql", key).group(1)
            model.CreateCustomView(key, text)
            print("%s installed in custom.Views" % key)

        elif key.endswith(".text.html"):
            key = re.search("(.*)\.text\.html", key).group(1)
            model.WriteContentText(key, text, keyword)
            print("%s installed in Text Content with Keyword %s" % (key, keyword))

        elif key.endswith(".sql"):
            key = re.search("(.*)\.sql", key).group(1)
            model.WriteContentSql(key, text, keyword)
            print("%s installed in Sql Scripts" % key)

        elif key.endswith(".py"):
            key = re.search("(.*)\.py", key).group(1)
            model.WriteContentPython(key, text, keyword)
            print("%s installed in Python Scripts with Keyword %s" % (key, keyword))

        elif key.endswith(".json"):
            key = re.search("(.*).json", key).group(1)
            model.WriteContentText(key, text, keyword)
            print("%s installed in Text Content with Keyword %s" % (key, keyword))

        elif key.endswith(".html"):
            key = re.search("(.*)\.html", key).group(1)
            model.WriteContentHtml(key, text, keyword)
            print("%s installed in Html Content with Keyword %s" % (key, keyword))

    if Install:
        model.RunScript(Install)
        print("Install.py run but not added to Content")

    print('</pre>')

if model.HttpMethod == 'get':
    Get()
elif model.HttpMethod == 'post':
    Post(Data)
