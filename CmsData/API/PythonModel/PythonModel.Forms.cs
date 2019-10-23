using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.API;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public string Script { get; set; } // set this in the python code for javascript on the output page
        public string Header { get; set; } // set this in the python code to set the output page header element
        public string Title { get; set; }  // set this in the python code to set the output page title
        public string Output { get; set; } // this is set automatically from the print statements for the output page
        public string Form { get; set; }
        public string HttpMethod { get; set; }

        public string BuildForm(string name, DynamicData dd, string buttons = "", DynamicData metadata = null)
        {
            var s = BuildFormRows(dd, metadata) + buttons;
            var form = $@"
<h4>{name.SpaceCamelCase()}</h4>
<form class='form-horizontal' id='{name}'>
    <fieldset>
<!--Inputs-->
    </fieldset>
</form>
";
            return form.Replace("<!--Inputs-->", s);
        }

        public string BuildDisplay(string name, DynamicData dd,
                                   string edit = "", string add = "",
                                   DynamicData metadata = null)
        {
            var displaytemplate = @"
<div class='report box box-responsive'>
  <div class='box-content'>
      <div class='table-responsive'>
          <h4>{0}</h4>
          <table class='table notwide'>
              <thead>
              <tr><th>Name</th><th>Value</th></tr>
              </thead>
              <tbody>
{1}
              </tbody>
          </table>
{2}
      </div>
  </div>
</div>

";
            if (dd == null)
                return string.Format(displaytemplate, name.SpaceCamelCase(), add, null);

            var s = BuildDisplayRows(dd, metadata);
            return string.Format(displaytemplate, name.SpaceCamelCase(), s, edit);
        }
        private string CreateControl(string name, object obj, bool hidden = false, string readOnly = null, bool textarea = false)
        {
            if (hidden)
                return null;
            if (obj is DynamicData || obj is Array)
                return null;

            string input;
            if (!textarea)
            {
                var value = string.Empty;
                if (obj?.ToString().HasValue() ?? true)
                    value = $"value='{obj}'";
                input =
                    $"<input id='{name}' name='{name}' {value} {readOnly} type='text' class='form-control input-md'>";
            }
            else
                input = $"<textarea class='form-control' id='{name}' name='{name}' {readOnly}>{obj}</textarea>";

            return $@"
<div class='form-group'>
    <label class='col-md-4 control-label' for='{name}'>{name.SpaceCamelCase()}</label>  
    <div class='col-md-4'>
        {input}
    </div>
</div>
";
        }
        private string CreateTextControl(string name, object o, bool hidden = false)
        {
            var controltemplate = @"
<div class='form-group'>
    <label class='col-md-4 control-label'>{0}</label>  
    <div class='col-md-4'>
        <div class='form-control'>{1}</div>
    </div>
</div>
";
            if (hidden)
                return null;
            if (o is DynamicData || o is Array)
                return null;
            var s = string.Format(controltemplate, name.SpaceCamelCase(), o);
            return s;
        }

        public string BuildFormRows(DynamicData dd, DynamicData metadata = null)
        {
            var sb = new StringBuilder();
            if (metadata != null)
            {
                foreach (var k in metadata.dict)
                {
                    var hidden = k.Value.ToString().StartsWith("hidden ");
                    var readOnly = k.Value.ToString().StartsWith("readonly ") ? "readonly" : "";
                    var textArea = k.Value.ToString().Contains("textarea");
                    var c = CreateControl(k.Key, dd[k.Key], hidden, readOnly, textArea);
                    if (c == null)
                        continue;
                    sb.Append(c);
                }
            }
            else
            {
                foreach (var k in dd.dict)
                {
                    var c = CreateControl(k.Key, k.Value);
                    if (c == null)
                        continue;
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string BuildDisplayRows(DynamicData dd, DynamicData metadata = null)
        {
            if (dd == null)
                return null;

            var sb = new StringBuilder();
            if (metadata != null)
            {
                foreach (var k in metadata.dict)
                {
                    var hidden = k.Value.ToString().StartsWith("hidden ");
                    var special = k.Value.ToString().StartsWith("special ");
                    var val = dd[k.Key]?.ToString();
                    switch (k.Value)
                    {
                        case "money":
                            var v = val?.ToString();
                            if (val != null)
                                val = v.ToDecimal().ToString2("c");
                            break;
                    }
                    var c = CreateTextControl(k.Key.SpaceCamelCase(), val, hidden);
                    if (c == null)
                        continue;
                    sb.Append(c);
                }
            }
            else
            {
                foreach (var k in dd.dict)
                {
                    var c = CreateTextControl(k.Key, k.Value);
                    if (c == null)
                        continue;
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
