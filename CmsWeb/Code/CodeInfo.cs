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
        private string _listName;
        private string _value;

        public CodeInfo()
        {

        }
        public CodeInfo(object value, string listname)
        {
            if (value != null)
                Value = value.ToString();
            ListName = listname;
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

        public string ListName
        {
            get { return _listName; }
            set
            {
                if (!value.HasValue())
                    return;
                _listName = value;
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

        public void CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, StringBuilder changes)
        {
            string altname = vm.Name + "Id";
            var attr = vm.GetAttribute<FieldInfoAttribute>();
            if (attr != null && attr.IdField.HasValue())
                altname = attr.IdField;
            var mid = modelProps.FirstOrDefault(ss => ss.Name == altname);
            if (mid == null)
                return;
            var track = Attribute.IsDefined(vm, typeof(TrackChangesAttribute));
            if (track)
                model.UpdateValue(changes, altname, Value);
            else
                mid.SetPropertyFromText(model, Value);
        }

        public void CopyFromModel(PropertyInfo vm, object model, PropertyInfo[] modelProps)
        {
            var cv = new CodeValueModel();
            var getlist = cv.GetType().GetMethod(vm.Name + "List");
            var list = (IEnumerable<CodeValueItem>)getlist.Invoke(cv, null);
            string altname = vm.Name + "Id";
            var attr = vm.GetAttribute<FieldInfoAttribute>();
            if (attr != null && attr.IdField.HasValue())
                altname = attr.IdField;
            var mid = modelProps.FirstOrDefault(mm => mm.Name == altname);
            Debug.Assert(mid != null, "mid != null");
            var midvalue = mid.GetValue(model, null);
            Value = list.ItemValue((int)(midvalue ?? 0));
        }
    }
}