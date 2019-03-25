using CmsData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Areas.OnlineReg.Models
{
    [Serializable]
    public partial class OnlineRegModel : IXmlSerializable
    {
        CMSDataContext CurrentDatabase { get; set; }
        public string Campus { get; set; }
        public string DefaultFunds { get; set; }
        public bool? testing { get; set; }
        public string FromMobile { get; set; }
        public string URL { get; set; }

        public bool DisplaySpecialFunds => OnlineGiving() || ManageGiving();

        private int? _tranId;
        public int? TranId
        {
            get { return _tranId; }
            set
            {
                _tranId = value;
                _transaction = null;
            }
        }

        [DisplayName("Username")]
        public string username { get; set; }
        public bool nologin { get; set; }
        public decimal? donation { get; set; }
        public int? donor { get; set; }
        public int? UserPeopleId { get; set; }
        public bool prospect { get; set; }
        public int? classid { get; set; }
        public string registertag { get; set; }
        public string registerLinkType { get; set; }

        private List<OnlineRegPersonModel> _list = new List<OnlineRegPersonModel>();

        public List<OnlineRegPersonModel> List
        {
            get { return _list; }
            set { _list = value; }
        }

        [XmlIgnore]
        [DisplayName("Password")]
        public string password { get; set; }

        public bool ShowFindInstructions;
        public bool ShowLoginInstructions;
        public bool ShowOtherInstructions;

        public int? GoerSupporterId { get; set; }
        public int? GoerId { get; set; }

        public Person Goer { get; set; }

        public bool SupportMissionTrip => GoerSupporterId.HasValue || GoerId.HasValue;
        public string MissionTripDates { get; set; }
        public decimal? MissionTripCost { get; set; }
        public decimal? MissionTripRaised { get; set; }

        public TransactionsModel.SupporterInfo[] Supporters { get; set; }

        private Transaction _transaction;
        public Transaction Transaction
        {
            get
            {
                if (_transaction == null && TranId.HasValue)
                {
                    _transaction = CurrentDatabase.Transactions.SingleOrDefault(tt => tt.Id == TranId);
                }

                return _transaction;
            }
        }

        private Person _user;
        public Person user
        {
            get
            {
                if (_user == null && UserPeopleId.HasValue)
                {
                    _user = CurrentDatabase.LoadPersonById(UserPeopleId.Value);
                }

                return _user;
            }
        }

        private Meeting _meeting;
        public Meeting meeting()
        {
            if (_meeting == null)
            {
                var q = from m in CurrentDatabase.Meetings
                        where m.Organization.OrganizationId == Orgid
                        where m.MeetingDate > Util.Now.AddHours(-12)
                        orderby m.MeetingDate
                        select m;
                _meeting = q.FirstOrDefault();
            }
            return _meeting;
        }

        private List<string> _history = new List<string>();
        public List<string> History
        {
            get { return _history; }
            set { _history = value; }
        }

        public void HistoryAdd(string s)
        {
            History.Add($"{s} {Util.Now:g} (c-ip={Util.GetIpAddress()})");
        }
    }
}
