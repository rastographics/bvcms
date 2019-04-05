using System.Collections.Generic;
using System.Web;

namespace UtilityExtensionsTests
{
    internal class MockHttpSessionState : HttpSessionStateBase
    {
        private Dictionary<string, object> internalDictionary;

        public MockHttpSessionState() { }

        public MockHttpSessionState(Dictionary<string, object> dictionary)
        {
            internalDictionary = dictionary;
        }

        public override object this[string name]
        {
            get => internalDictionary.ContainsKey(name) ? internalDictionary [name] : null;
            set => internalDictionary[name] = value;
        }
    }
}
