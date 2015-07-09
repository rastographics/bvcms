using CmsWeb;
using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.CheckInAPI
{
    public class CheckInPerson
    {
        public int id = 0;
        public string name = "";

        public Dictionary<string, object> fields;

        public CheckInPerson(Object person)
        {
            fields = person.AsDictionary();

            id = (int)fields["id"];
            name = (string)fields["name"];
        }

        public void addField(string name, object value)
        {
            if (fields == null)
            {
                fields = new Dictionary<string, object>();
            }

            fields.Add(name, value);
        }
    }
}