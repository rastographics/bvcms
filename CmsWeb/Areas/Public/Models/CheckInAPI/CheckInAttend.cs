using System;

namespace CmsWeb.Areas.Public.Models.CheckInAPI
{
    public class CheckInAttend
    {
        public int orgID = 0;
        public DateTime datetime = DateTime.Now;

        public int peopleID = 0;
        public bool present = false;
    }
}