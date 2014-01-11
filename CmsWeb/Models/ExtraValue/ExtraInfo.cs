using System.Web;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class ExtraInfo
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string TypeDisplay { get; set; }
        public int Count { get; set; }
        public bool Standard { get; set; }
        public bool CanView { get; set; }

        public string QueryUrl
        {
            get
            {
                if (Type == "Bit" || Type == "Code")
                    return "/ExtraValue/QueryCodes?field={0}&value={1}"
                        .Fmt(HttpUtility.UrlEncode(Field), HttpUtility.UrlEncode(Value));
                return "/ExtraValue/QueryData?field={0}&type={1}"
                    .Fmt(HttpUtility.UrlEncode(Field), Type);
            }
        }
        public string DeleteAllUrl
        {
            get
            {
                return "/ExtraValue/DeleteAll?field={0}&type={1}&value={2}"
                    .Fmt(HttpUtility.UrlEncode(Field), Type, HttpUtility.UrlEncode(Value));
            }
        }
        public string ConvertToStandardUrl
        {
            get
            {
                return "/ExtraValue/ConvertToStandard/People?name={0}".Fmt(HttpUtility.UrlEncode(Field));
            }
        }
    }
}