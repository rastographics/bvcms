using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models.Person;
using CmsWeb.Areas.Public;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class TrackChangesAttribute : Attribute { }
    public class ZeroToNullAttribute : Attribute { }
    public class CodeValueAttribute : Attribute { }
    public class PhoneAttribute : Attribute { }

    public class FieldInfoAttribute : Attribute
    {
        public string IdField { get; set; }
        public string CheckboxField { get; set; }
    }

    public interface IModelViewModelObject
    {
        void CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, StringBuilder changes);
        void CopyFromModel(PropertyInfo vm, object model, PropertyInfo[] modelProps);
    }

    public static class ModelViewModel
    {
        public static void CopyPropertiesFrom(this object viewmodel, object model, string onlyfields = "", string excludefields = "")
        {
            var cv = new CodeValueModel();
            var modelProps = model.GetType().GetProperties();
            var viewmodelProps = viewmodel.GetType().GetProperties();
            var only = onlyfields.Split(',');
            var exclude = excludefields.Split(',');

            foreach (var vm in viewmodelProps)
            {
                if (onlyfields.HasValue() && !only.Contains(vm.Name))
                    continue;
                if (excludefields.HasValue() && exclude.Contains(vm.Name))
                    continue;

                if (viewmodel is IModelViewModelObject)
                {
                    var ivm = viewmodel as IModelViewModelObject;
                    ivm.CopyFromModel(vm, model, modelProps);
                    continue;
                }
                if (vm.HasAttribute<CodeValueAttribute>() || vm.PropertyType == typeof(CodeInfo))
                {
                    var getlist = cv.GetType().GetMethod(vm.Name + "List");
                    var list = (IEnumerable<CodeValueItem>)getlist.Invoke(cv, null);
                    string altname = vm.Name + "Id";
                    var attr = vm.GetAttribute<FieldInfoAttribute>();
                    if (attr != null && attr.IdField.HasValue())
                        altname = attr.IdField;
                    var mid = modelProps.FirstOrDefault(mm => mm.Name == altname);
                    if (mid == null)
                        continue;
                    var midvalue = mid.GetValue(model, null);
                    if (vm.HasAttribute<CodeValueAttribute>())
                    {
                        var v = list.ItemValue((int)(midvalue ?? 0));
                        vm.SetPropertyFromText(viewmodel, v);
                    }
                    else // CodeInfo
                        vm.SetValue(viewmodel, new CodeInfo(midvalue, list), null);
                    continue;
                }

                // find a model property of the same name as viewmodel
                var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);

                if (m == null)
                    continue;

                // get the model value we are going to copy
                var modelvalue = m.GetValue(model, null);

                // if they are the same type, then straight copy
                if (m.PropertyType == vm.PropertyType)
                    vm.SetValue(viewmodel, modelvalue, null);

                else if (vm.PropertyType == typeof(CellPhoneInfo))
                {
                    var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
                    var ckpi = modelProps.Single(ss => ss.Name == ckf);
                    var ck = ckpi.GetValue(model, null) as bool?;
                    var emi = new CellPhoneInfo(((string)modelvalue).FmtFone(), ck ?? false);
                    vm.SetValue(viewmodel, emi, null);
                }

                else if (vm.PropertyType == typeof(EmailInfo))
                {
                    var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
                    var ckpi = modelProps.Single(ss => ss.Name == ckf);
                    var ck = ckpi.GetValue(model, null) as bool?;
                    var emi = new EmailInfo((string)modelvalue, ck ?? false);
                    vm.SetValue(viewmodel, emi, null);
                }

                else if (vm.HasAttribute<PhoneAttribute>())
                    vm.SetPropertyFromText(viewmodel, ((string)modelvalue).FmtFone());

                else if (modelvalue is string)
                    vm.SetPropertyFromText(viewmodel, (string)modelvalue);

                else if (modelvalue is DateTime && (DateTime)modelvalue == ((DateTime)modelvalue).Date)
                    vm.SetPropertyFromText(viewmodel, ((DateTime)modelvalue).ToShortDateString());

                else // Handle any other type mismatches like int = Nullable<int> or vice-versa
                    vm.SetPropertyFromText(viewmodel, (modelvalue ?? "").ToString());
            }
        }
        public static string CopyPropertiesTo(this object viewmodel, object model, string onlyfields = "", string excludefields = "")
        {
            var modelProps = model.GetType().GetProperties();
            var viewmodelProps = viewmodel.GetType().GetProperties();
            var changes = new StringBuilder();
            var only = onlyfields.Split(',');
            var exclude = excludefields.Split(',');

            foreach (var vm in viewmodelProps)
            {
                if (onlyfields.HasValue() && !only.Contains(vm.Name))
                    continue;
                if (excludefields.HasValue() && exclude.Contains(vm.Name))
                    continue;

                if (viewmodel is IModelViewModelObject)
                {
                    var ivm = viewmodel as IModelViewModelObject;
                    ivm.CopyToModel(vm, model, modelProps, changes);
                    continue;
                }

                // get the viewmodel value we are going to copy
                var viewmodelvalue = vm.GetValue(viewmodel, null);

                var track = Attribute.IsDefined(vm, typeof(TrackChangesAttribute));

                // find a target property of the same name as source
                var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);

                if (m == null)
                    continue;

                // if they are the same type, then straight copy
                if (m.PropertyType == vm.PropertyType)
                    if (track)
                        model.UpdateValue(changes, m.Name, viewmodelvalue);
                    else
                        m.SetValue(model, viewmodelvalue, null);

                else if (vm.HasAttribute<ZeroToNullAttribute>())
                {
                    if ((int)viewmodelvalue == 0)
                        viewmodelvalue = null;
                    if (track)
                        model.UpdateValue(changes, m.Name, viewmodelvalue);
                    else
                        m.SetValue(model, viewmodelvalue, null);
                }

                else if (vm.PropertyType == typeof(CellPhoneInfo))
                {
                    var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
                    var ckpi = modelProps.Single(mm => mm.Name == ckf);
                    var ci = viewmodelvalue as CellPhoneInfo;
                    Debug.Assert(ci != null, "ci != null");
                    if (track)
                    {
                        model.UpdateValue(changes, m.Name, ci.Number.GetDigits());
                        model.UpdateValue(changes, ckf, ci.ReceiveText);
                    }
                    else
                    {
                        m.SetValue(model, ci.Number.GetDigits(), null);
                        ckpi.SetValue(model, ci.ReceiveText, null);
                    }
                }

                else if (vm.PropertyType == typeof(EmailInfo))
                {
                    var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
                    var ckpi = modelProps.Single(mm => mm.Name == ckf);
                    var emi = viewmodelvalue as EmailInfo;
                    Debug.Assert(emi != null, "emi != null");
                    if (track)
                    {
                        model.UpdateValue(changes, m.Name, emi.Address);
                        model.UpdateValue(changes, ckf, emi.Send);
                    }
                    else
                    {
                        m.SetValue(model, emi.Address, null);
                        ckpi.SetValue(model, emi.Send, null);
                    }
                }

                else if (vm.HasAttribute<PhoneAttribute>())
                {
                    var ph = ((string)viewmodelvalue).GetDigits();
                    if (track)
                        model.UpdateValue(changes, m.Name, ph);
                    else
                        m.SetPropertyFromText(model, ph);
                }

                else if (viewmodelvalue is string)
                    if (track)
                        model.UpdateValue(changes, m.Name, viewmodelvalue);
                    else
                        m.SetPropertyFromText(model, (string)viewmodelvalue);

                else // Handle any other type mismatches like int = Nullable<int> or vice-versa
                    if (track)
                        model.UpdateValue(changes, m.Name, viewmodelvalue);
                    else
                        m.SetPropertyFromText(model, (viewmodelvalue ?? "").ToString());
            }
            return changes.ToString();
        }

        public static SelectList ToSelect(this IEnumerable<CodeValueItem> items, string datafield = "Id")
        {
            if (items == null)
                throw new Exception("items are null in SelectList");
            return new SelectList(items, datafield, "Value");
        }
        public static IEnumerable<CodeValueItem> AddNotSpecified(this IEnumerable<CodeValueItem> q)
        {
            return q.AddNotSpecified(0);
        }
        public static IEnumerable<CodeValueItem> AddNotSpecified(this IEnumerable<CodeValueItem> q, int value)
        {
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Id = value, Code = value.ToString(), Value = "(not specified)" });
            return list;
        }

        public static bool HasAttribute<T>(this PropertyInfo a)
        {
            return Attribute.IsDefined(a, typeof(T));
        }

        public static T GetAttribute<T>(this PropertyInfo a)
        {
            return a.GetCustomAttributes(true).OfType<T>().SingleOrDefault();
        }
    }
}