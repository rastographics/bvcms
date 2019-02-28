using CmsData;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class EmailInfo : IModelViewModelObject
    {
        [StringLength(150)]
        public string Address { get; set; }
        public bool Send { get; set; }

        public List<ChangeDetail> CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, bool track)
        {
            var changes = new List<ChangeDetail>();
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(mm => mm.Name == ckf);
            if (track)
            {
                model.UpdateValue(changes, vm.Name, Address);
                model.UpdateValue(changes, ckf, Send);
                return changes;
            }
            var ci = modelProps.FirstOrDefault(ss => ss.Name == vm.Name);
            Debug.Assert(ci != null, "ci != null");
            ci.SetValue(model, Address, null);
            ckpi.SetValue(model, Send, null);
            return changes;
        }

        public void CopyFromModel(PropertyInfo vm, object existing, object model, PropertyInfo[] modelProps)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(ss => ss.Name == ckf);
            var ck = ckpi.GetValue(model, null) as bool?;
            var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);
            Debug.Assert(m != null, "m != null");
            Address = ((string)m.GetValue(model, null));
            Send = ck ?? false;
        }
    }
    public class CellPhoneInfo : IModelViewModelObject
    {
        [StringLength(20)]
        public string Number { get; set; }
        public bool ReceiveText { get; set; }

        public List<ChangeDetail> CopyToModel(PropertyInfo vm, object model, PropertyInfo[] modelProps, bool track)
        {
            var changes = new List<ChangeDetail>();
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(mm => mm.Name == ckf);
            if (track)
            {
                model.UpdateValue(changes, vm.Name, Number.GetDigits());
                model.UpdateValue(changes, ckf, ReceiveText);
                return changes;
            }
            var ci = modelProps.FirstOrDefault(ss => ss.Name == vm.Name);
            Debug.Assert(ci != null, "ci != null");
            ci.SetValue(model, Number.GetDigits(), null);
            ckpi.SetValue(model, ReceiveText, null);
            return changes;
        }

        public void CopyFromModel(PropertyInfo vm, object existing, object model, PropertyInfo[] modelProps)
        {
            var ckf = vm.GetAttribute<FieldInfoAttribute>().CheckboxField;
            var ckpi = modelProps.Single(ss => ss.Name == ckf);
            var ck = ckpi.GetValue(model, null) as bool?;
            var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);
            Number = ((string)m.GetValue(model, null)).FmtFone();
            ReceiveText = ck ?? false;
        }
    }

    [TrackChanges]
    public class BasicPersonInfo
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id == 0)
                {
                    return;
                }

                person = DbUtil.Db.LoadPersonById(value);
            }
        }

        [NoUpdate]
        public int PeopleId { get; set; }
        public Person person { get; set; }

        [DisplayName("Title"), StringLength(10)]
        public string TitleCode { get; set; }

        [TrackChanges, StringLength(25)]
        public string FirstName { get; set; }

        [DisplayName("Goes By"), StringLength(15)]
        public string NickName { get; set; }

        [StringLength(30)]
        public string MiddleName { get; set; }

        [StringLength(100), Required]
        public string LastName { get; set; }

        [StringLength(100)]
        public string AltName { get; set; }

        [DisplayName("Former Name"), StringLength(20)]
        public string MaidenName { get; set; }

        [DisplayName("Suffix"), StringLength(10)]
        public string SuffixCode { get; set; }

        public CodeInfo Gender { get; set; }

        public CodeInfo PositionInFamily { get; set; }

        public CodeInfo Campus { get; set; }

        [DisplayName("Birthday")]
        [DateEmptyOrValid]
        [BirthdateValid]
        public string DOB { get; set; }

        public CodeInfo MaritalStatus { get; set; }

        public DateTime? WeddingDate { get; set; }

        [DisplayName("Occupation"), NoTrack, StringLength(60)]
        public string OccupationOther { get; set; }

        [DisplayName("Employer"), NoTrack, StringLength(60)]
        public string EmployerOther { get; set; }

        [DisplayName("School"), NoTrack, StringLength(100)]
        public string SchoolOther { get; set; }

        [DisplayFormat(DataFormatString = "{0:g}")]
        public int? Grade { get; set; }

        [DisplayName("Primary Email"), FieldInfo(CheckboxField = "SendEmailAddress1")]
        public EmailInfo EmailAddress { get; set; }

        [DisplayName("Alt Email"), FieldInfo(CheckboxField = "SendEmailAddress2")]
        public EmailInfo EmailAddress2 { get; set; }

        [PhoneNumber, StringLength(20)]
        public string HomePhone { get; set; }

        [PhoneNumber, FieldInfo(CheckboxField = "ReceiveSMS")]
        public CellPhoneInfo CellPhone { get; set; }

        [PhoneNumber, StringLength(20)]
        public string WorkPhone { get; set; }

        public DateTime? DeceasedDate { get; set; }

        [DisplayName("Do not call")]
        public bool DoNotCallFlag { get; set; }

        [DisplayName("Do not visit")]
        public bool DoNotVisitFlag { get; set; }

        [DisplayName("Do not mail")]
        public bool DoNotMailFlag { get; set; }

        public bool DoNotPublishPhones { get; set; }

        [NoUpdate]
        public DateTime? CreatedDate { get; set; }
        [NoUpdate]
        public DateTime? JoinDate { get; set; }
        [NoUpdate]
        public string Spouse { get; set; }

        [NoUpdate]
        public string Age { get; set; }

        [NoUpdate]
        public CodeInfo MemberStatus { get; set; }

        public BasicPersonInfo()
        {

        }
        public BasicPersonInfo(int id)
        {
            Id = id;
            if (person == null)
            {
                return;
            }

            person.SendEmailAddress1 = person.SendEmailAddress1 ?? true; // set sendemailaddress1 to either true or false, null is not allowed, default is true
            this.CopyPropertiesFrom(person);
        }
        public void UpdatePerson()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);

            if (Grade == null && p.Grade == 0) // special case to fix bug
            {
                p.Grade = null;
            }

            var changes = this.CopyPropertiesTo(p);
            p.LogChanges(DbUtil.Db, changes);

            var fsb = new List<ChangeDetail>();
            p.Family.UpdateValue(fsb, "HomePhone", HomePhone.GetDigits());
            p.Family.LogChanges(DbUtil.Db, fsb, p.PeopleId, Util.UserPeopleId ?? 0);

            if (p.DeceasedDateChanged)
            {
                var ret = p.MemberProfileAutomation(DbUtil.Db);
                if (ret != "ok")
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(
                        new Exception(ret + " for PeopleId:" + p.PeopleId));
                }
            }

            DbUtil.Db.SubmitChanges();

            if (!HttpContextFactory.Current.User.IsInRole("Access"))
            {
                changes.AddRange(fsb);
                if (changes.Count > 0)
                {
                    var np = DbUtil.Db.GetNewPeopleManagers();
                    if (np != null)
                    {
                        DbUtil.Db.EmailRedacted(p.FromEmail, np,
                            "Basic Person Info Changed on " + Util.Host,
                            $"{Util.UserName} changed the following information for <a href='{DbUtil.Db.ServerLink($"/Person2/{PeopleId}")}'>{FirstName} {LastName}</a> ({PeopleId}):<br />\n"
                            + ChangeTable(changes));
                    }
                }
            }
        }

        private static string ChangeTable(IEnumerable<ChangeDetail> changes)
        {
            var sb = new StringBuilder();
            sb.Append("<table cellspacing='5'>\n");
            sb.Append("<tr><td><b>Field</b></td><td><b>Before</b></td><td><b>After</b></td></tr>\n");
            foreach (var c in changes)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
            }
            sb.Append("</table>");
            return sb.ToString();
        }
    }
}
