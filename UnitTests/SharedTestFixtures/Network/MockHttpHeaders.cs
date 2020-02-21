using System;
using System.Collections.Specialized;
using System.Linq;

namespace SharedTestFixtures.Network
{
    public class MockHttpHeaders : NameValueCollection
    {
        public MockHttpHeaders() : base() { }

        public MockHttpHeaders(NameValueCollection collection) : base()
        {
            this.Add(collection);
            AddMissing("Timestamp", DateTime.Now.ToString());
            AddMissing("Content-Type", "text/html");
            AddMissing("Cache-Control", "no-cache, no-store, must-revalidate");
            AddMissing("Pragma", "no-cache");
            AddMissing("Expires", "0");
        }

        public void AddMissing(string key, string value)
        {
            if (AllKeys.Contains(key)) { return; }
            Add(key, value);
        }
    }
}
