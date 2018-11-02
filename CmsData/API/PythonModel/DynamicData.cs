using System;
using System.Collections.Generic;
using System.Dynamic;
using IronPython.Runtime;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CmsData.API
{
    [Serializable]
    public class DynamicData : DynamicObject, ISerializable
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
            var dynamicData = d as DynamicData;
            if (dynamicData != null)
                return new Dictionary<string, object>(dynamicData.dict);

            var pythonDictionary = d as PythonDictionary;
            if (pythonDictionary != null)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in pythonDictionary)
                    dict.Add("@" + kv.Key, kv.Value);
                return dict;
            }

            var dictionary = d as Dictionary<string, object>;
            if (dictionary != null)
                return new Dictionary<string, object>(dictionary);
       
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

        public object this[string key] => dict.ContainsKey(key) ? dict[key] : null;

        public object GetValue(string key)
        {
            if(dict.ContainsKey(key))
                return dict[key];
            return null;
        }

        public void Remove(string name)
        {
            if(dict.ContainsKey(name))
                dict.Remove(name);
        }
        public void AddValue(string name, object value)
        {
            dict[name] = value;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var kv in dict)
                info.AddValue(kv.Key, kv.Value);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }
    }
}
