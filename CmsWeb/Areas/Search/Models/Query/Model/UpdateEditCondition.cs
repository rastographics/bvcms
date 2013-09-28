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
        public void UpdateCondition()
        {
            var c = Current;
            this.CopyPropertiesTo(c);
            c.ConditionName = ConditionName;
            c.Comparison = Comparison;
            switch (c.FieldInfo.Type)
            {
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                    c.TextValue = TextValue;
                    break;
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                case FieldType.NullInteger:
                    c.TextValue = IntegerValue.ToString();
                    break;
                case FieldType.Number:
                case FieldType.NullNumber:
                    c.TextValue = NumberValue.ToString();
                    break;
                case FieldType.Date:
                case FieldType.DateSimple:
                    c.DateValue = DateValue;
                    break;
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    if (c.HasMultipleCodes && CodeValues != null)
                        c.CodeIdValue = string.Join(";", CodeValues);
                    else
                        c.CodeIdValue = CodeValue;
                    break;
            }
//            c.Program = Program.Value.ToInt();
//            c.Division = Division.Value.ToInt();
//            c.Organization = Organization.Value.ToInt();
//            if (MinistryVisible)
//                c.Program = Ministry.Value.ToInt();
//            c.Schedule = Schedule.Value.ToInt();
//            c.Campus = Campus.Value.ToInt();
//            c.OrgType = OrgType.Value.ToInt();
//            c.StartDate = StartDate;
//            c.EndDate = EndDate;
//            c.Days = Days.ToInt();
//            c.Age = Age;
//            c.Quarters = Quarters;
//            if (Tags != null)
//                c.Tags = string.Join(";", Tags);
//            else if (PmmLabels != null)
//                c.Tags = string.Join(",", PmmLabels);
            TopClause.Save(Db, increment: true);
        }
        public void EditCondition()
        {
            var c = Current;
            SelectedId = c.Id;
            this.CopyPropertiesFrom(c);
            SetVisibility();
            switch (c.FieldInfo.Type)
            {
//                case FieldType.String:
//                case FieldType.StringEqual:
//                case FieldType.StringEqualOrStartsWith:
//                    TextValue = c.TextValue;
//                    break;
//                case FieldType.Integer:
//                case FieldType.IntegerSimple:
//                case FieldType.IntegerEqual:
//                case FieldType.NullInteger:
//                    IntegerValue = c.IntegerValue;
//                    break;
//                case FieldType.Number:
//                case FieldType.NullNumber:
//                    NumberValue = c.NumberValue;
//                    break;
//                case FieldType.Date:
//                case FieldType.DateSimple:
//                    DateValue = c.DateValue;
//                    break;
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    CodeValue = c.CodeIdValue;
                    if (c.HasMultipleCodes && CodeValue.HasValue())
                    {
                        CodeValues = c.CodeIdValue.Split(';');
                        foreach (var i in GetCodeData())
                            i.Selected = CodeValues.Contains(i.Value);
                    }
                    break;
            }
//            Program = c.Program;
//            Division = c.Division;
//            Organization = c.Organization;
//            Schedule = c.Schedule;
//            Campus = c.Campus;
//            OrgType = c.OrgType;
//            StartDate = DateString(c.StartDate);
//            EndDate = DateString(c.EndDate);
//            HasMultipleCodes = c.HasMultipleCodes;
//            Days = c.Days.ToString();
//            Age = c.Age.ToString();
//            Quarters = c.Quarters;
            if (TagsVisible)
            {
                if (c.Tags != null)
                    Tags = c.Tags.Split(';');
                var cv = new CodeValueModel();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
                foreach (var i in TagData)
                    i.Selected = Tags.Contains(i.Value);
            }
            if (PmmLabelsVisible)
            {
                if (c.Tags != null)
                    PmmLabels = c.Tags.Split(',').Select(vv => vv).ToArray();
                var cv = new CodeValueModel();
                PmmLabelData = CodeValueModel.ConvertToSelect(cv.PmmLabels(), "Id");
                if (PmmLabels != null)
                    foreach (var i in PmmLabelData)
                        i.Selected = PmmLabels.Contains(i.Value);
            }
//            if (MinistryVisible)
//                Ministry = c.Program;
        }
    }
}