using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.People.Models.Person;
using CmsWeb.Models;

namespace CmsWeb.Code
{
    public class CodeInfo
    {
        private string _listName;
        private string _value;

        public CodeInfo()
        {
            
        }
        public CodeInfo(object value, string listname)
        {
            if (value != null)
                Value = value.ToString();
            ListName = listname;
        }
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string ListName
        {
            get { return _listName; }
            set 
            { 
                _listName = value;
                var cv = new CodeValueModel();
                switch (value)
                {
                    case "Marital":
                        Items = new SelectList(cv.MaritalStatusCodes99(), "Id", "Value", Value);
                        break;
                    case "Gender":
                        Items = new SelectList(cv.GenderCodesWithUnspecified(), "Id", "Value", Value);
                        break;
                    case "EntryPoint":
                        Items = new SelectList(cv.EntryPoints(), "Id", "Value", Value);
                        break;
                    case "Campus":
                        Items = new SelectList(cv.AllCampuses0(), "Id", "Value", Value);
                        break;
                    case "MemberStatus":
                        Items = new SelectList(cv.MemberStatusList(), "Id", "Value", Value);
                        break;
                    case "MemberType":
                        Items = new SelectList(CodeValueModel.MemberTypeCodes0(), "Id", "Value", Value);
                        break;
                    case "FamilyPosition":
                        Items = new SelectList(cv.FamilyPositionCodes(), "Id", "Value", Value);
                        break;
                    case "Title":
                        Items = new SelectList(cv.TitleCodes(), "Value", "Value", Value);
                        break;
                    case "State":
                        Items = new SelectList(cv.GetStateListUnknown(), "Code", "Value", Value);
                        break;
                    case "Country":
                        Items = new SelectList(CodeValueModel.GetCountryList(), "Value", "Value", Value);
                        break;
                    case "ResCode":
                        Items = new SelectList(CodeValueModel.ResidentCodesWithZero(), "Id", "Value", Value);
                        break;
                    default:
                        throw new Exception("UnknownCodeListName = " + value);
                }
            }
        }

        public IEnumerable<SelectListItem> Items { get; set; }
        public override string ToString()
        {
            if (Items == null)
                return Value;
            var i = Items.SingleOrDefault(ii => ii.Value == Value);
            if (i == null)
                return "";
            return i.Text;
        }
    }
}