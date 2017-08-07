using System.Text;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class DirectoryInfo
    {
        public Person Person { get; set; }
        public string SpouseFirst { get; set; }
        public string SpouseLast { get; set; }
        public string SpouseEmail { get; set; }
        public string SpouseCellPhone { get; set; }
        public bool? SpouseDoNotPublishPhone { get; set; }
        public string SpouseName => SpouseLast == Person.LastName ? SpouseFirst : $"{SpouseFirst} {SpouseLast}";
        public string FamilyTitle { get; set; }
        public string FamilyName
        {
            get
            {
                if (!SpouseFirst.HasValue())
                    return Person.Name;
                if(CoupleName.HasValue())
                    return CoupleName;
                if (SpouseLast != Person.LastName)
                    return $"{Person.PreferredName} {Person.LastName} & {SpouseFirst} {SpouseLast}";
                return $"{Person.PreferredName} & {SpouseFirst} {Person.LastName}";
            }
        }
        public string FirstNames
        {
            get
            {
                if (!SpouseFirst.HasValue())
                    return Person.PreferredName;
                if (SpouseLast != Person.LastName)
                    return $"{Person.PreferredName} & {SpouseFirst} {SpouseLast}";
                return $"{Person.PreferredName} & {SpouseFirst}";
            }
        }
        public string HomePhone => Person.DoNotPublishPhones == true ? "" : Person.HomePhone.FmtFone();
        public string CellPhone => Person.DoNotPublishPhones == true ? "" : Person.CellPhone.FmtFone();
        public string SpouseCell => SpouseDoNotPublishPhone == true ? "" : SpouseCellPhone.FmtFone();
        public string Address
        {
            get
            {
                var sb = new StringBuilder();
                AddLine(sb, Person.PrimaryAddress);
                AddLine(sb, Person.PrimaryAddress2);
                AddLine(sb, Person.CityStateZip);
                return sb.ToString();
            }
        }

        private void AddLine(StringBuilder sb, string s)
        {
            if (!s.HasValue())
                return;
            if (sb.Length > 0)
                sb.Append("\n");
            sb.Append(s);
        }
        public string CoupleName { get; set; }
        public string Children { get; set; }
        public string ChildrenAges { get; set; }
        public int? ImageId { get; set; }
        public int? FamImageId { get; set; }
    }
}