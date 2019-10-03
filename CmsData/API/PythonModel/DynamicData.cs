using IronPython.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using UtilityExtensions;

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

            if (d is DynamicData dynamicData)
            {
                return new Dictionary<string, object>(dynamicData.dict);
            }

            if (d is PythonDictionary pythonDictionary)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in pythonDictionary)
                {
                    dict.Add(kv.Key.ToString(), kv.Value);
                }

                return dict;
            }

            if (d is IDictionary<string, object> dapperRow)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in dapperRow)
                {
                    dict.Add(kv.Key, kv.Value);
                }

                return dict;
            }

            if (d is Dictionary<string, string> dictionaryss) // QueryParameters
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in dictionaryss)
                {
                    dict.Add(kv.Key, kv.Value);
                }

                return dict;
            }

            if (d is Dictionary<string, object> dictionaryso)
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
        public void SetValue(string name, string value)
        {
            if (value == null)
                AddValue(name, value);
            else if(value.AllDigits())
                AddValue(name, value.ToInt());
            else if(value.IsDecimal())
                AddValue(name, value.ToDecimal());
            else
                AddValue(name, value);
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
                        case JTokenType.Array:
                            object o;
                            if (t.HasValues && t.First is JObject)
                                // a list of dictionary objects
                                o = JsonConvert.DeserializeObject<List<DynamicData>>(t.ToString());
                            else
                                // just a list of objects (strings or integers)
                                o = JsonConvert.DeserializeObject<IronPython.Runtime.List>(t.ToString());
                            dict.Add(kv.Name, o);
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
            var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            return json.Replace("'", @"''");
        }
        public string ToFlatString()
        {
            var emptyvalues = dict.Where(vv => vv.Value == null || vv.Value?.ToString() == "").Select(vv => vv.Key).ToList();
            foreach (var k in emptyvalues)
                this.Remove(k);
            var json = JsonConvert.SerializeObject(dict);
            return json.Replace("'", @"''");
        }

        public List<string> Keys(DynamicData metadata = null)
        {
            var keys = new List<string>();
            if (metadata != null)
            {
                foreach (var k in metadata.dict)
                {
                    var typ = k.Value.ToString();
                    var tok = typ.GetCsvToken(1, sep: " ");
                    switch (tok)
                    {
                        case "hidden":
                        case "special":
                        case "readonly":
                            continue;
                    }
                    keys.Add(k.Key);
                }
                return keys;
            }
            foreach (var k in dict)
            {
                if(k.Value is DynamicData || k.Value is Array)
                    continue;
                keys.Add(k.Key);
            }
            return keys;
        }
        public List<string> SpecialKeys(DynamicData metadata)
        {
            var keys = new List<string>();
            foreach (var k in metadata.dict)
            {
                if(k.Value.ToString().StartsWith("special "))
                    keys.Add(k.Key);
            }
            return keys;
        }
    }
}
