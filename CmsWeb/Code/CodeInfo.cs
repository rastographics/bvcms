using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Code
{
    [Serializable]
    public class CodeInfo : IModelViewModelObject, ISerializable
    {
        private string name;
        private string value;

        [NoUpdate] private SelectList items;

        public CodeInfo()
        {
        }

        public CodeInfo(string name) : this(null, name)
        {
        }

        public CodeInfo(object value, string name)
        {
            if (value != null)
                Value = value.ToString();
            Name = name;
        }

        public CodeInfo(object value, string name, SelectList items)
            : this(value, items)
        {
            this.name = name;
        }

        public CodeInfo(object value, SelectList items)
        {
            if (value != null)
                Value = value.ToString();
            Items = items;
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public int? IntVal => value.ToInt();

        public SelectList Items
        {
            get { return items; }
            set { items = value; }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (!value.HasValue())
                    return;
                name = value;
                var cv = new CodeValueModel();
                switch (name)
                {
                    case "State":
                        Items = cv.StateList().ToSelect("Code");
                        break;
                    case "Country":
                        Items = cv.CountryList().ToSelect("Value");
                        break;
                    case "ContactResult":
                        Items = cv.ContactResultList().ToSelect("Value");
                        break;
                    default:
                        if (items != null)
                            return;
                        var getlist = cv.GetType().GetMethod(value + "List");
                        if (getlist != null)
                        {
                            var list = getlist.Invoke(cv, null);
                            var enumerable = list as IEnumerable<CodeValueItem>;
                            if (enumerable != null)
                                Items = enumerable.ToSelect();
                            else if (list is SelectList)
                                Items = (SelectList)list;
                        }
                        else
                            Items = new List<CodeValueItem>().ToSelect();
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

        public List<ChangeDetail> CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, bool track)
        {
            var changes = new List<ChangeDetail>();
            string altname = vm.Name + "Id";
            var attr = vm.GetAttribute<FieldInfoAttribute>();
            if (attr != null && attr.IdField.HasValue())
                altname = attr.IdField;
            var mid = modelProps.FirstOrDefault(mm => mm.Name == altname || mm.Name == vm.Name);

            if (mid == null)
                return changes;

            if (mid.PropertyType.Name == "CodeInfo")
            {
                Util.SetPropertyEx(model, vm.Name + ".Value", Value == "0" ? null : Value);
            }
            else
            {
                if (track)
                {
                    if (mid.PropertyType == typeof(int?) || mid.PropertyType == typeof(int))
                        if (Value == "0")
                            model.UpdateValue(changes, altname, null);
                        else
                            model.UpdateValue(changes, altname, Value.ToInt());
                    return changes;
                }
                mid.SetPropertyFromText(model, Value == "0" ? null : Value);
            }

            return changes;
        }

        public void CopyFromModel(PropertyInfo vm, object existing, object model, PropertyInfo[] modelProps)
        {
            string altname = vm.Name + "Id";
            var fiattr = vm.GetAttribute<FieldInfoAttribute>();
            if (fiattr != null && fiattr.IdField.HasValue())
                altname = fiattr.IdField;
            var mid = modelProps.FirstOrDefault(mm => mm.Name == altname || mm.Name == vm.Name);
            var midvalue = mid.GetValue(model, null);
            if (mid.PropertyType == typeof(int?) || mid.PropertyType == typeof(int))
                Value = midvalue.ToInt().ToString();
            else if (midvalue == null)
                Value = null;
            else if (mid.PropertyType.Name == "CodeInfo")
                Value = ((CodeInfo) midvalue).Value;
            else
                Value = midvalue.ToString();
            var cinfo = midvalue as CodeInfo;
            if (cinfo == null && existing != null)
                cinfo = existing as CodeInfo;
            if (items == null && cinfo != null && cinfo.items != null)
                items = cinfo.items;
            Name = vm.Name;
        }

        protected CodeInfo(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Value = info.GetString("Value");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Value", Value);
        }
    }
}
