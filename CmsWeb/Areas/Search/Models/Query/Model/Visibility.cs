using CmsData;
using System.Collections.Generic;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel
    {
        public bool RightPanelVisible => ComparePanelVisible;
        public bool ComparePanelVisible => fieldMap.Name != "MatchAnything";

        public bool TextVisible => texttypes.Contains(fieldMap.Type);
        public bool NumberVisible => numbertypes.Contains(fieldMap.Type);
        public bool IntegerVisible => integertypes.Contains(fieldMap.Type);
        public bool CodeVisible => codetypes.Contains(fieldMap.Type);
        public bool DateVisible => datetypes.Contains(fieldMap.Type);

        public bool ProgramVisible => fieldMap.HasParam("Program");
        public bool DivisionVisible => fieldMap.HasParam("Division");
        public bool EndDateVisible => fieldMap.HasParam("EndDate");
        public bool StartDateVisible => fieldMap.HasParam("StartDate");
        public bool OrganizationVisible => fieldMap.HasParam("Organization");
        public bool ScheduleVisible => fieldMap.HasParam("Schedule");
        public bool CampusVisible => fieldMap.HasParam("Campus");
        public bool OrgTypeVisible => fieldMap.HasParam("OrgType");
        public bool OrgType2Visible => fieldMap.HasParam("OrgType2");
        public bool DaysVisible => fieldMap.HasParam("Days");
        public bool AgeVisible => fieldMap.HasParam("Age");
        public bool SavedQueryVisible => fieldMap.HasParam("SavedQueryIdDesc");
        public bool MinistryVisible => fieldMap.HasParam("Ministry");
        public bool QuartersVisible => fieldMap.HasParam("Quarters");
        public bool TagsVisible => fieldMap.HasParam("Tags");
        public bool PmmLabelsVisible => fieldMap.HasParam("PmmLabels");
        public bool OrgNameVisible => fieldMap.HasParam("OrgName");
        public bool OrgStatusVisible => fieldMap.HasParam("OrgStatus");
        public bool OnlineRegVisible => fieldMap.HasParam("OnlineReg");

        public bool AutoRun => (Db?.UserPreference("QueryAutoRun", "false") ?? "").ToBool();

        private List<FieldType> texttypes = new List<FieldType>()
        {
            FieldType.String,
            FieldType.StringEqual,
            FieldType.StringEqualOrStartsWith
        };
        private List<FieldType> codetypes = new List<FieldType>()
        {
            FieldType.Bit,
            FieldType.NullBit,
            FieldType.Code,
            FieldType.NullCode,
            FieldType.CodeStr,
            FieldType.DateField,
        };
        private List<FieldType> integertypes = new List<FieldType>()
        {
            FieldType.Integer,
            FieldType.IntegerEqual,
            FieldType.IntegerSimple,
            FieldType.NullInteger,
        };
        private List<FieldType> numbertypes = new List<FieldType>()
        {
            FieldType.NullNumber,
            FieldType.Number,
            FieldType.NumberLG
        };
        private List<FieldType> datetypes = new List<FieldType>()
        {
            FieldType.Date,
            FieldType.DateSimple,
        };
    }
}
