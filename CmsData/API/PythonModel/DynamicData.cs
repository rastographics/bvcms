using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using HandlebarsDotNet;

namespace CmsData.API
{
    public class DynamicData : DynamicObject
    {
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, object> d { get; }
        public DynamicData()
        {
            d = new Dictionary<string, object>();
        }
        public DynamicData(Dictionary<string, object> dict)
        {
            d = dict;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            d[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = d.ContainsKey(binder.Name) 
                ? d[binder.Name] : "";
            return true;
        }
    }
}
