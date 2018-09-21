using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class NoTrackAttribute : Attribute { }
    public class PhoneNumberAttribute : Attribute { }
    public class TrackChangesAttribute : Attribute { }
    public class NoUpdate : Attribute { }
    public class SkipFieldOnCopyProperties : Attribute { }
    public class RemoveNA : Attribute { }

    public class FieldInfoAttribute : Attribute
    {
        public string IdField { get; set; }
        public string CheckboxField { get; set; }
    }

    public interface IModelViewModelObject
    {
        List<ChangeDetail> CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, bool track);
        void CopyFromModel(PropertyInfo vm, object existing, object model, PropertyInfo[] modelProps);
    }

    public static class ModelViewModel
    {
        public static void CopyPropertiesFrom(this object viewmodel, object model, Type type = null, string onlyfields = "", string excludefields = "")
        {
            var modelProps = model.GetType().GetProperties();
            var viewmodelProps = viewmodel.GetType().GetProperties().Where(mm => mm.CanWrite);
            var only = onlyfields.Split(',');
            var exclude = excludefields.Split(',');

            foreach (var vm in viewmodelProps)
            {
                if (onlyfields.HasValue() && !only.Contains(vm.Name))
                    continue;
                if (excludefields.HasValue() && exclude.Contains(vm.Name))
                    continue;
                if (vm.HasAttribute<SkipFieldOnCopyProperties>())
                    continue;
                if (type != null && !Attribute.IsDefined(vm, type))
                    continue;

                if (typeof(IModelViewModelObject).IsAssignableFrom(vm.PropertyType))
                {
                    var existing = vm.GetValue(viewmodel);
                    var ivm = Activator.CreateInstance(vm.PropertyType);
                    ((IModelViewModelObject)ivm).CopyFromModel(vm, existing, model, modelProps);
                    vm.SetValue(viewmodel, ivm, null);
                    continue;
                }
                // find a model property of the same name as viewmodel
                var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);

                if (m == null)
                    continue;

                // get the model value we are going to copy
                var modelvalue = m.GetValue(model, null);

                if (vm.HasAttribute<PhoneNumberAttribute>())
                    if(vm.HasAttribute<RemoveNA>())
                        vm.SetPropertyFromText(viewmodel, ((string)modelvalue).Disallow("na").FmtFone());
                    else
                        vm.SetPropertyFromText(viewmodel, ((string)modelvalue).FmtFone());

                else if (vm.HasAttribute<RemoveNA>())
                    vm.SetPropertyFromText(viewmodel, ((string)modelvalue).Disallow("na"));

                    // if they are the same type, then straight copy

                else if (m.PropertyType == vm.PropertyType)
                    vm.SetValue(viewmodel, modelvalue, null);

                else if (modelvalue is string)
                    vm.SetPropertyFromText(viewmodel, (string)modelvalue);

                else if (modelvalue is DateTime && (DateTime)modelvalue == ((DateTime)modelvalue).Date)
                    vm.SetPropertyFromText(viewmodel, ((DateTime)modelvalue).ToShortDateString());

                else // Handle any other type mismatches like int = Nullable<int> or vice-versa
                    vm.SetPropertyFromText(viewmodel, (modelvalue ?? "").ToString());
            }
        }
        public static List<ChangeDetail> CopyPropertiesTo(this object viewmodel, object model, Type type = null, string onlyfields = "", string excludefields = "")
        {
            var modelProps = model.GetType().GetProperties().Where(pp => pp.CanWrite).ToArray();
            var viewmodelProps = viewmodel.GetType().GetProperties().Where(pp => pp.CanWrite).ToArray();
            var changes = new List<ChangeDetail>();
            var only = onlyfields.Split(',');
            var exclude = excludefields?.Split(',');

            foreach (var vm in viewmodelProps)
            {
                if (onlyfields.HasValue() && !only.Contains(vm.Name))
                    continue;
                if (exclude != null && exclude.Contains(vm.Name))
                    continue;
                if (vm.HasAttribute<NoUpdate>())
                    continue;
                if (type != null && !Attribute.IsDefined(vm, type))
                    continue;

                // get the viewmodel value we are going to copy
                var viewmodelvalue = vm.GetValue(viewmodel, null);

                var track = (Attribute.IsDefined(vm, typeof (TrackChangesAttribute))
                            || viewmodel.GetType().IsDefined(typeof (TrackChangesAttribute), false))
                            && !Attribute.IsDefined(vm, typeof(NoTrackAttribute));

                if (viewmodelvalue is IModelViewModelObject)
                {
                    var ivm = viewmodelvalue as IModelViewModelObject;
                    changes.AddRange(ivm.CopyToModel(vm, model, modelProps, track));
                    continue;
                }

                // find a target property of the same name as source
                var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);

                if (m == null)
                    continue;

                if (vm.HasAttribute<PhoneNumberAttribute>() || vm.HasAttribute<PhoneAttribute>())
                {
                    var ph = ((string)viewmodelvalue).GetDigits();
                    if (track)
                        model.UpdateValue(changes, m.Name, ph);
                    else
                        m.SetPropertyFromText(model, ph);
                    vm.SetValue(viewmodel, ph.FmtFone(), null);
                }

                // if they are the same type, then straight copy
                else if (m.PropertyType == vm.PropertyType)
                    if (track)
                        model.UpdateValue(changes, m.Name, viewmodelvalue);
                    else
                        m.SetValue(model, viewmodelvalue, null);

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
            return changes;
        }

        public static SelectList ToSelect(this IEnumerable<CodeValueItem> items, string datafield = "Id")
        {
            if (items == null)
                throw new Exception("items are null in SelectList");
            return new SelectList(items, datafield, "Value");
        }
        public static IEnumerable<CodeValueItem> AddNotSpecified(this IEnumerable<CodeValueItem> q, string notspecified = "(not specified)")
        {
            return q.AddNotSpecified(0, notspecified);
        }
        public static IEnumerable<CodeValueItem> AddNotSpecified(this IEnumerable<CodeValueItem> q, int value, string notspecified = "(not specified)")
        {
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Id = value, Code = value.ToString(), Value = notspecified });
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
