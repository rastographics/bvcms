using System;
using CmsData;

namespace CmsWeb.Models.ExtraValues
{
    public class ExtraValue
    {
        public int Id;
        public int? Id2;
        public string Field { get; set; }
        public string StrValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string Data { get; set; }
        public int? IntValue { get; set; }
        public bool? BitValue { get; set; }
        public string Type { get; set; } 

        public ExtraValueModel Model { get; set; }

        public ExtraValue() { }

        public ExtraValue(PeopleExtra v, ExtraValueModel model)
        {
            Type = v.Type;
            Field = v.Field.Trim();
            StrValue = v.StrValue;
            DateValue = v.DateValue;
            Data = v.Data;
            IntValue = v.IntValue;
            BitValue = v.BitValue;
            Id = v.PeopleId;
            Model = model;
        }
        public ExtraValue(FamilyExtra v, ExtraValueModel model)
        {
            Type = v.Type;
            Field = v.Field.Trim();
            StrValue = v.StrValue;
            DateValue = v.DateValue;
            Data = v.Data;
            IntValue = v.IntValue;
            BitValue = v.BitValue;
            Id = v.FamilyId;
            Model = model;
        }
        public ExtraValue(OrganizationExtra v, ExtraValueModel model)
        {
            Type = v.Type;
            Field = v.Field.Trim();
            StrValue = v.StrValue;
            DateValue = v.DateValue;
            Data = v.Data;
            IntValue = v.IntValue;
            BitValue = v.BitValue;
            Id = v.OrganizationId;
            Model = model;
        }
        public ExtraValue(ContactExtra v, ExtraValueModel model)
        {
            Type = v.Type;
            Field = v.Field.Trim();
            StrValue = v.StrValue;
            DateValue = v.DateValue;
            Data = v.Data;
            IntValue = v.IntValue;
            BitValue = v.BitValue;
            Id = v.ContactId;
            Model = model;
        }
        public ExtraValue(MeetingExtra v, ExtraValueModel model)
        {
            Type = v.Type;
            Field = v.Field.Trim();
            StrValue = v.StrValue;
            DateValue = v.DateValue;
            Data = v.Data;
            IntValue = v.IntValue;
            BitValue = v.BitValue;
            Id = v.MeetingId;
            Model = model;
        }
        public ExtraValue(OrgMemberExtra v, ExtraValueModel model)
        {
            Type = v.Type;
            Field = v.Field.Trim();
            StrValue = v.StrValue;
            DateValue = v.DateValue;
            Data = v.Data;
            IntValue = v.IntValue;
            BitValue = v.BitValue;
            Id = v.OrganizationId;
            Id2 = v.PeopleId;
            Model = model;
        }
    }
}
