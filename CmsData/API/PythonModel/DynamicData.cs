using IronPython.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace CmsData.API
{
    [Serializable]
    public class DynamicData : DynamicObject, ISerializable
    {
        // ReSharper disable once InconsistentNaming
        internal Dictionary<string, object> dict { get; }

        // This constructor creates a new DynamicData object 
        // with a new Dictionary
        public DynamicData()
        {
            dict = new Dictionary<string, object>();
        }

        // This constructor creates a new DynamicData object 
        // with an existing dictionary passed in.
        internal DynamicData(Dictionary<string, object> datadict)
        {
            dict = datadict;
        }

        // This constructor creates a new DynamicData object 
        // with a new dictionary populated from an existing non native dictionary passed in.
        public DynamicData(object datadict)
        {
            dict = AddDictionary(datadict);
        }

        private static Dictionary<string, object> AddDictionary(object d)
        {
            // determine what type of dictionary is passed in

            // Is it a DynamicData object?
            var dynamicData = d as DynamicData;
            if (dynamicData != null)
            {
                return new Dictionary<string, object>(dynamicData.dict);
            }

            // Is it a PythonDictionary object?
            var pythonDictionary = d as PythonDictionary;
            if (pythonDictionary != null)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in pythonDictionary)
                {
                    dict.Add("@" + kv.Key, kv.Value);
                }

                return dict;
            }

            // Is it a Dictionary of strings like QueryParameters?
            var dictionaryss = d as Dictionary<string, string>;
            if (dictionaryss != null)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in dictionaryss)
                {
                    dict.Add("@" + kv.Key, kv.Value);
                }

                return dict;
            }

            var dictionaryso = d as Dictionary<string, object>;
            if (dictionaryso != null)
            {
                return dictionaryso;
            }
            // Note the option to handle native Dictionary<string, object> is not needed
            // since it is handled by the second constructor.

            throw new Exception("data is an unexpected type");
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
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            return null;
        }

        public void Remove(string name)
        {
            if (dict.ContainsKey(name))
            {
                dict.Remove(name);
            }
        }
        public void AddValue(string name, object value)
        {
            dict[name] = value;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var kv in dict)
            {
                info.AddValue(kv.Key, kv.Value);
            }
        }
        /// <summary>
        /// This constructor is used by JsonConvert.DeserializeObject
        /// </summary>
        public DynamicData(SerializationInfo info, StreamingContext context)
        {
            dict = new Dictionary<string, object>();
            foreach (var kv in info)
            {
                if (kv.Value is JObject)
                {
                    var dd = JsonConvert.DeserializeObject<DynamicData>(kv.Value.ToString());
                    dict.Add(kv.Name, dd);
                }
                else // Must be a JValue Type which inherits from JToken
                {
                    var t = kv.Value as JToken;
                    switch (t?.Type)
                    {
                        case JTokenType.Integer:
                            dict.Add(kv.Name, Convert.ToInt32(kv.Value));
                            break;
                        case JTokenType.Date:
                            dict.Add(kv.Name, Convert.ToDateTime(kv.Value));
                            break;
                        case JTokenType.String:
                            dict.Add(kv.Name, kv.Value.ToString());
                            break;
                        case JTokenType.Float:
                            dict.Add(kv.Name, Convert.ToDecimal(kv.Value));
                            break;
                        default:
                            dict.Add(kv.Name, kv.Value);
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }
    }
}
