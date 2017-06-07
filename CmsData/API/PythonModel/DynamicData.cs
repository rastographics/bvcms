using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using HandlebarsDotNet;
using IronPython.Runtime;

namespace CmsData.API
{
    public class DynamicData : DynamicObject
    {
        // ReSharper disable once InconsistentNaming
        internal Dictionary<string, object> dict { get; }
        public DynamicData()
        {
            dict = new Dictionary<string, object>();
        }
        public DynamicData(object datadict)
        {
            dict = AddDictionary(datadict);
        }
        private static Dictionary<string, object> AddDictionary(object d)
        {
            var dictionary = d as Dictionary<string, object>;
            if (dictionary != null)
                return dictionary;

            var dynamicData = d as DynamicData;
            if (dynamicData != null)
                return dynamicData.dict;

            var pythonDictionary = d as PythonDictionary;
            if (pythonDictionary != null)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in pythonDictionary)
                    dict.Add("@" + kv.Key, kv.Value);
                return dict;
            }

            var dictionaryss = d as Dictionary<string, string>;
            if (dictionaryss != null)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in dictionaryss)
                    dict.Add("@" + kv.Key, kv.Value);
                return dict;
            }
            throw new Exception("data is not a dictionary");
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dict[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = dict.ContainsKey(binder.Name) 
                ? dict[binder.Name] : "";
            return true;
        }
        public string this[string key] => dict[key] == null ? null : dict[key].ToString();

        public object GetValue(string key)
        {
            if(dict.ContainsKey(key))
                return dict[key];
            return null;
        }
    }
}
