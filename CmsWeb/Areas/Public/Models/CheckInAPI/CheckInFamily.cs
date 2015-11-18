using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamily
    {
        public int id = 0;
        public string name = "";

        public List<CheckInPerson> members = new List<CheckInPerson>();

        public void addMember(CheckInPerson person)
        {
            members.Add(person);
        }
    }
}