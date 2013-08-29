using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class CodeInfo : IModelViewModelObject
    {
        private string _name;
        private string _value;

        public CodeInfo()
        {

        }
        public CodeInfo(object value, string name)
        {
            if (value != null)
                Value = value.ToString();
            Name = name;
        }
        public CodeInfo(object value, IEnumerable<CodeValueItem> items)
        {
            if (value != null)
                Value = value.ToString();
            Items = items.ToSelect();
        }
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public IEnumerable<SelectListItem> Items { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (!value.HasValue())
                    return;
                _name = value;
                var cv = new CodeValueModel();
                switch (value)
                {
                    case "State":
                        Items = cv.StateList().ToSelect("Code");
                        break;
                    case "Country":
                        Items = cv.CountryList().ToSelect("Value");
                        break;
                    default:
                        var getlist = cv.GetType().GetMethod(value + "List");
                        Items = ((IEnumerable<CodeValueItem>)getlist.Invoke(cv, null)).ToSelect();
                        break;
                }
            }
        }

        public override string ToString()
        {
            if (Items == null)
                return Value;
            var i = Items.SingleOrDefault(ii => ii.Value == Value);
            if (i == null)
                return "";
            return i.Text;
        }

        public string CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps)
        {
            string altname = vm.Name + "Id";
            var attr = vm.GetAttribute<FieldInfoAttribute>();
            if (attr != null && attr.IdField.HasValue())
                altname = attr.IdField;
            var mid = modelProps.FirstOrDefault(ss => ss.Name == altname);
            if (mid == null)
                return string.Empty;
            var track = Attribute.IsDefined(vm, typeof(TrackChangesAttribute));
            if (track)
            {
                var changes = new StringBuilder();
                if(mid.PropertyType == typeof(int?))
                    if(Value == "0")
                        model.UpdateValue(changes, altname, null);
                    else
                        model.UpdateValue(changes, altname, Value.ToInt());
                return changes.ToString();
            }
            mid.SetPropertyFromText(model, Value);
            return string.Empty;
        }

        public void CopyFromModel(PropertyInfo vm, object model, PropertyInfo[] modelProps)
        {
            string altname = vm.Name + "Id";
            var attr = vm.GetAttribute<FieldInfoAttribute>();
            if (attr != null && attr.IdField.HasValue())
                altname = attr.IdField;
            var mid = modelProps.FirstOrDefault(mm => mm.Name == altname);
            Debug.Assert(mid != null, "mid != null");
            var midvalue = mid.GetValue(model, null);
            Value = midvalue.ToInt().ToString();
            Name = vm.Name;
        }
    }
}