using System;
using System.Reflection;
using System.Collections;
using System.Web.UI;
using System.Collections.Generic;
using System.Data.Common;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public const BindingFlags bindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public static Object GetAs(DbDataReader reader, Type type)
        {
            Object o = Activator.CreateInstance(type);
            PropertyInfo[] props = type.GetProperties();
            for (int i = 0; i < props.Length; i++)
                props[i].SetPropertyFromText(o, reader[props[i].Name].ToString().Trim());
            return o;
        }
        public static Object GetAs(string line, Type type)
        {
            Object o = Activator.CreateInstance(type);
            PropertyInfo[] props = type.GetProperties();
            var a = line.Split('\t');
            for (int i = 0; i < props.Length; i++)
                props[i].SetPropertyFromText(o, a[i].Trim());
            return o;
        }
        public static object GetProperty(object Object, string Property)
        {
            return Object.GetType().GetProperty(Property, bindingFlags).GetValue(Object, null);
        }
        public static bool HasProperty(object Object, string Property)
        {
            var p = Object.GetType().GetProperty(Property, bindingFlags);
            return p != null;
        }

        public static object GetField(object Object, string Property)
        {
            return Object.GetType().GetField(Property, bindingFlags).GetValue(Object);
        }

        private static object GetPropertyInternal(object Parent, string Property)
        {
            if (Property == "this")
                return Parent;

            object Result = null;
            string PureProperty = Property;
            string Indexes = null;
            bool IsArrayOrCollection = false;

            if (Property.IndexOf("[") > -1)
            {
                PureProperty = Property.Substring(0, Property.IndexOf("["));
                Indexes = Property.Substring(Property.IndexOf("["));
                IsArrayOrCollection = true;
            }

            MemberInfo Member = Parent.GetType().GetMember(PureProperty, bindingFlags)[0];
            if (Member.MemberType == MemberTypes.Property)
                Result = ((PropertyInfo)Member).GetValue(Parent, null);
            else
                Result = ((FieldInfo)Member).GetValue(Parent);

            if (IsArrayOrCollection)
            {
                Indexes = Indexes.Replace("[", "").Replace("]", "");

                if (Result is Array)
                {
                    int Index = -1;
                    int.TryParse(Indexes, out Index);
                    Result = CallMethod(Result, "GetValue", Index);
                }
                else if (Result is ICollection)
                {
                    if (Indexes.StartsWith("\""))
                    {
                        Indexes = Indexes.Trim('\"');
                        Result = CallMethod(Result, "get_Item", Indexes);
                    }
                    else
                    {
                        int Index = -1;
                        int.TryParse(Indexes, out Index);
                        Result = CallMethod(Result, "get_Item", Index);
                    }
                }

            }
            return Result;
        }

        private static void SetPropertyInternal(object Parent, string Property, object Value)
        {
            if (Property == "this")
                return;

            object Result = null;
            string Property0 = Property;
            string Indexes = null;
            bool IsArrayOrCollection = false;

            if (Property.IndexOf("[") > -1)
            {
                Property0 = Property.Substring(0, Property.IndexOf("["));
                Indexes = Property.Substring(Property.IndexOf("["));
                IsArrayOrCollection = true;
            }

            if (!IsArrayOrCollection)
            {
                MemberInfo Member = Parent.GetType().GetMember(Property0, bindingFlags)[0];
                if (Member.MemberType == MemberTypes.Property)
                    ((PropertyInfo)Member).SetValue(Parent, Value, null);
                else
                    ((FieldInfo)Member).SetValue(Parent, Value);
                return;
            }
            else
            {
                MemberInfo Member = Parent.GetType().GetMember(Property0, bindingFlags)[0];
                if (Member.MemberType == MemberTypes.Property)
                    Result = ((PropertyInfo)Member).GetValue(Parent, null);
                else
                    Result = ((FieldInfo)Member).GetValue(Parent);
            }
            if (IsArrayOrCollection)
            {
                Indexes = Indexes.Replace("[", "").Replace("]", "");
                if (Result is Array)
                {
                    int i = -1;
                    int.TryParse(Indexes, out i);
                    CallMethod(Result, "SetValue", Value, i);
                }
                else if (Result is ICollection)
                {
                    if (Indexes.StartsWith("\""))
                    {
                        Indexes = Indexes.Trim('\"');
                        CallMethod(Result, "set_Item", Indexes, Value);
                    }
                    else
                    {
                        int i = -1;
                        int.TryParse(Indexes, out i);
                        CallMethod(Result, "set_Item", i, Value);
                    }
                }
            }
        }

        public static object GetPropertyEx(object Parent, string Property)
        {
            if (Parent == null)
                return null;
            Type Type = Parent.GetType();
            int DotAt = Property.IndexOf(".");
            if (DotAt < 0)
                return GetPropertyInternal(Parent, Property);
            string Main = Property.Substring(0, DotAt);
            string Subs = Property.Substring(DotAt + 1);
            object Sub = GetPropertyInternal(Parent, Main);
            return GetPropertyEx(Sub, Subs);
        }

        public static void SetProperty(object Object, string Property, object Value)
        {
            var p = Object.GetType().GetProperty(Property, bindingFlags);
            p.SetValue(Object, Value, null);
        }

        public static void SetField(object Object, string Property, object Value)
        {
            Object.GetType().GetField(Property, bindingFlags).SetValue(Object, Value);
        }

        public static void SetPropertyEx(object Parent, string Property, object Value)
        {
            Type Type = Parent.GetType();
            int DotAt = Property.IndexOf(".");
            if (DotAt < 0)
            {
                SetPropertyInternal(Parent, Property, Value);
                return;
            }
            string Main = Property.Substring(0, DotAt);
            string Subs = Property.Substring(DotAt + 1);
            object Sub = GetPropertyInternal(Parent, Main);
            SetPropertyEx(Sub, Subs, Value);
        }

        public static bool SetPropertyFromText(object obj, string member, string textvalue)
        {
            Type typBindingSource = null;
            var a = obj.GetType().GetMember(member, bindingFlags);
            if (a.Length == 0)
                return false;
            object minfo = a[0];
            MemberTypes mt;
            if (minfo is FieldInfo)
                mt = ((FieldInfo)minfo).MemberType;
            else
                mt = ((PropertyInfo)minfo).MemberType;

            if (mt == MemberTypes.Field)
                typBindingSource = ((FieldInfo)minfo).FieldType;
            else
                typBindingSource = ((PropertyInfo)minfo).PropertyType;

            object value = null;
            if (!GetValue(textvalue, typBindingSource, ref value))
                return true;

            SetPropertyEx(obj, member, value);
            return true;
        }

        private static bool GetValue(string textvalue, Type typBindingSource, ref object value)
        {
            if (typBindingSource == typeof(string))
                value = textvalue;
            else if (typBindingSource == typeof(int))
                value = textvalue.ToInt();
            else if (typBindingSource == typeof(int?))
                value = textvalue.ToInt2();
            else if (typBindingSource == typeof(bool))
                value = textvalue.ToBool();
            else if (typBindingSource == typeof(bool?))
                value = textvalue.ToBool2();
            else if (typBindingSource == typeof(DateTime))
                value = textvalue.ToDate() ?? DateTime.MinValue;
            else if (typBindingSource == typeof(DateTime?))
                value = textvalue.ToDate();
            else if (typBindingSource == typeof(long?))
                value = textvalue.ToLong2();
            else if (typBindingSource == typeof(decimal))
                value = textvalue.ToDecimal() ?? 0;
            else if (typBindingSource == typeof(decimal?))
                value = textvalue.ToDecimal();
            else if (typBindingSource == typeof(double))
                value = double.Parse(textvalue);
            else if (typBindingSource.IsEnum)
                value = Enum.Parse(typBindingSource, textvalue);
            else
                return false;
            return true;
        }

        public static void SetPropertyFromText(this PropertyInfo p, object obj, string textvalue)
        {
            object value = null;
            if (!GetValue(textvalue, p.PropertyType, ref value))
                return;
            p.SetValue(obj, value, null);
        }

        public static object CallMethod(object Object, string Method, params object[] Params)
        {
            return Object.GetType().InvokeMember(Method, bindingFlags | BindingFlags.InvokeMethod, null, Object, Params);
        }

        public static object CallMethodEx(object Parent, string Method, params object[] Params)
        {
            Type Type = Parent.GetType();
            int DotAt = Method.IndexOf(".");
            if (DotAt < 0)
                return CallMethod(Parent, Method, Params);
            string Main = Method.Substring(0, DotAt);
            string Subs = Method.Substring(DotAt + 1);
            object Sub = GetPropertyInternal(Parent, Main);
            return CallMethodEx(Sub, Subs, Params);
        }

        public static Control FindControlRecursive(Control Root, string Id)
        {
            if (Root.ID == Id)
                return Root;

            foreach (Control Ctl in Root.Controls)
            {
                Control FoundCtl = FindControlRecursive(Ctl, Id);
                if (FoundCtl != null)
                    return FoundCtl;
            }

            return null;
        }
        public class Numbered<T>
        {
            public int Index { get; set; }
            public T Value { get; set; }
        }

        public static IEnumerable<Numbered<T>> ToNumbered<T>(this IEnumerable<T> q)
        {
            int i = 0;
            foreach (T element in q)
            {
                yield return new Numbered<T> { Index = i, Value = element };
                i++;
            }
        }
    }
}
