using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using System.Data.Linq;
using CmsData.View;
using CmsWeb.Code;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace CmsWeb.Areas.People.Models
{
    public class MemberInfo
    {
        public Person person;
        private readonly CMSDataContext Db;

        [NoUpdate]
        public int PeopleId { get; set; }

        #region Contributions --------------------------------------------------

        [DisplayName("Statement Option")]
        public CodeInfo ContributionOptions { get; set; }

        [DisplayName("Envelope Option")]
        public CodeInfo EnvelopeOptions { get; set; }

        #endregion
        #region Decision --------------------------------------------------

        [DisplayName("Type")]
        public CodeInfo DecisionType { get; set; }

        [DisplayName("Date")]
        public DateTime? DecisionDate { get; set; }

        #endregion
        #region Baptism --------------------------------------------------

        [DisplayName("Status")]
        public CodeInfo BaptismStatus { get; set; }

        [DisplayName("Type")]
        public CodeInfo BaptismType { get; set; }

        [DisplayName("Date")]
        public DateTime? BaptismDate { get; set; }

        [DisplayName("Scheduled")]
        public DateTime? BaptismSchedDate { get; set; }

        #endregion
        #region Drop --------------------------------------------------

        [DisplayName("Type"), FieldInfo(IdField = "DropCodeId")]
        public CodeInfo DropType { get; set; }

        [DisplayName("Date")]
        public DateTime? DropDate { get; set; }

        [DisplayName("New Church"), StringLength(60)]
        public string OtherNewChurch { get; set; }

        #endregion
        #region New Member Class --------------------------------------------------

        [DisplayName("Status")]
        public CodeInfo NewMemberClassStatus { get; set; }

        [DisplayName("Date")]
        public DateTime? NewMemberClassDate { get; set; }

        #endregion
        #region Membership -----------------------------------------------------

        [TrackChanges]
        public CodeInfo MemberStatus { get; set; }

        [DisplayName("How Joined"), FieldInfo(IdField = "JoinCodeId")]
        public CodeInfo JoinType { get; set; }

        public DateTime? JoinDate { get; set; }

        [DisplayName("Prev Church"), StringLength(60)]
        public string OtherPreviousChurch { get; set; }

        #endregion
        #region Letter Status

        public CodeInfo LetterStatus { get; set; }

        [DisplayName("Letter Requested")]
        public DateTime? LetterDateRequested { get; set; }

        [DisplayName("Letter Received")]
        public DateTime? LetterDateReceived { get; set; }

        [UIHint("Textarea"), DisplayName("Letter Notes")]
        public string LetterStatusNotes { get; set; }

        #endregion

        public MemberInfo()
        {
            Db = DbUtil.Db;
        }
        public MemberInfo(int id)
            : this()
        {
            person = Db.LoadPersonById(id);
            if (person == null)
                return;
            this.CopyPropertiesFrom(person);
        }

        public string UpdateMember()
        {
            var i = (from p in DbUtil.Db.People
                     where p.PeopleId == PeopleId
                     select new
                     {
                         p,
                         p.Family
                     }).Single();

            var changes = this.CopyPropertiesTo(i.p, excludefields: "HomePhone");

            i.p.LogChanges(DbUtil.Db, changes);


            var ret = i.p.MemberProfileAutomation(DbUtil.Db);
            if (ret == "ok")
            {
                DbUtil.Db.SubmitChanges();
                person = i.p;
                DbUtil.Db.SubmitChanges();
                DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, person);
                this.CopyPropertiesFrom(person);
            }
            else
            {
                person = i.p;
                this.CopyPropertiesFrom(person);
            }
            return ret;
        }

        public List<AllStatusFlag> StatusFlags()
        {
            return (from s in DbUtil.Db.ViewAllStatusFlags.ToList()
                    where s.Role == null || HttpContext.Current.User.IsInRole(s.Role)
                    where s.PeopleId == PeopleId
                    orderby s.Flag
                    select s).ToList();
        }
    }
}
