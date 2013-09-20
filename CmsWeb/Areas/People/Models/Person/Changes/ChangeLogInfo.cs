using System;

namespace CmsWeb.Areas.People.Models.Person
{
    public class ChangeLogInfo
    {
        public string User { get; set; }
        public string FieldSet { get; set; }
        public DateTime? Time { get; set; }
        public string Field { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string pf { get; set; }
        public bool Reversable { get; set; }
    }
}