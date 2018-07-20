using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsWeb.Code;
using CmsWeb.Models;

namespace CmsWeb.Areas.Manage.Models
{
    public class UpdateFieldsLookups
    {
        public static IEnumerable<CodeValueItem> Fetch(string field, ref bool useCode)
        {
            IEnumerable<CodeValueItem> m = null;
            var lookups = new CodeValueModel();
            switch (field)
            {
                case "Approval Codes":
                    m = lookups.VolunteerCodes();
                    break;
                case "Baptism Status":
                    m = lookups.BaptismStatusList();
                    break;
                case "Baptism Type":
                    m = lookups.BaptismTypeList();
                    break;
                case "Bad Address Flag":
                    m = UpdateFieldsModel.BadAddressFlag();
                    useCode = true;
                    break;
                case "Campus":
                    m = lookups.AllCampuses();
                    break;
                case "Statement Options":
                    m = lookups.EnvelopeOptionList();
                    break;
                case "Electronic Statement":
                    m = UpdateFieldsModel.ElectronicStatement();
                    useCode = true;
                    break;
                case "Decision Type":
                    m = lookups.DecisionTypeList();
                    break;
                case "Do Not Call":
                    m = UpdateFieldsModel.DoNotCall();
                    useCode = true;
                    break;
                case "Do Not Mail":
                    m = UpdateFieldsModel.DoNotMail();
                    useCode = true;
                    break;
                case "Drop Type":
                    m = lookups.DropTypeList();
                    break;
                case "Envelope Options":
                    m = lookups.EnvelopeOptionList();
                    break;
                case "Entry Point":
                    m = lookups.EntryPoints();
                    useCode = false;
                    break;
                case "Family Position":
                    m = lookups.FamilyPositionCodes();
                    break;
                case "Gender":
                    m = lookups.GenderCodes();
                    break;
                case "Grade":
                    m = UpdateFieldsModel.Grades();
                    useCode = true;
                    break;
                case "Join Type":
                    m = lookups.JoinTypeList();
                    break;
                case "Marital Status":
                    m = lookups.MaritalStatusCodes();
                    break;
                case "Member Status":
                    m = lookups.MemberStatusCodes();
                    break;
                case "New Member Class":
                    m = lookups.NewMemberClassStatusList();
                    break;
                case "ReceiveSMS":
                    m = UpdateFieldsModel.ReceiveSMS();
                    useCode = true;
                    break;
            }
            return m;
        }
    }
}
