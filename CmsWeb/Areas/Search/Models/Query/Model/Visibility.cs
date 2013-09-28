using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel
    {
        public bool RightPanelVisible { get; set; }
        public bool ComparePanelVisible { get; set; }
        public bool TextVisible { get; set; }
        public bool NumberVisible { get; set; }
        public bool IntegerVisible { get; set; }
        public bool CodeVisible { get; set; }
        public bool DateVisible { get; set; }
        public bool ProgramVisible { get; set; }
        public bool DivisionVisible { get; set; }
        public bool EndDateVisible { get; set; }
        public bool StartDateVisible { get; set; }
        public bool OrganizationVisible { get; set; }
        public bool ScheduleVisible { get; set; }
        public bool CampusVisible { get; set; }
        public bool OrgTypeVisible { get; set; }
        public bool DaysVisible { get; set; }
        public bool AgeVisible { get; set; }
        public bool SavedQueryVisible { get; set; }
        public bool MinistryVisible { get; set; }
        public bool QuartersVisible { get; set; }
        public bool TagsVisible { get; set; }
        public bool PmmLabelsVisible { get; set; }

        public void SetVisibility()
        {
            ComparePanelVisible = fieldMap.Name != "MatchAnything";
            RightPanelVisible = ComparePanelVisible;
            ConditionName = ConditionName;
            DivisionVisible = fieldMap.HasParam("Division");
            ProgramVisible = fieldMap.HasParam("Program");
            OrganizationVisible = fieldMap.HasParam("Organization");
            ScheduleVisible = fieldMap.HasParam("Schedule");
            CampusVisible = fieldMap.HasParam("Campus");
            OrgTypeVisible = fieldMap.HasParam("OrgType");
            DaysVisible = fieldMap.HasParam("Days");
            AgeVisible = fieldMap.HasParam("Age");
            SavedQueryVisible = fieldMap.HasParam("SavedQueryIdDesc");
            MinistryVisible = fieldMap.HasParam("Ministry");
            QuartersVisible = fieldMap.HasParam("Quarters");
            if (QuartersVisible)
                QuartersLabel = fieldMap.QuartersTitle;
            PmmLabelsVisible = fieldMap.HasParam("PmmLabels");
            TagsVisible = fieldMap.HasParam("Tags");
            if (TagsVisible)
            {
                var cv = new CodeValueModel();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
            }
            StartDateVisible = fieldMap.HasParam("StartDate");
            EndDateVisible = fieldMap.HasParam("EndDate");

            TextVisible = NumberVisible = CodeVisible = DateVisible = false;
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                    CodeVisible = true;
                    break;
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    TextVisible = true;
                    break;
                case FieldType.NullNumber:
                case FieldType.Number:
                    NumberVisible = true;
                    break;
                case FieldType.NullInteger:
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                    IntegerVisible = true;
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    DateVisible = true;
                    break;
            }
        }
    }
}