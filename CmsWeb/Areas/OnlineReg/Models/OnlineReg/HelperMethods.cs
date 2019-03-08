using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Areas.Dialog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using UtilityExtensions;
using Person = CmsData.Person;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        //        public static Organization CreateAccountOrg()
        //        {
        //            var settings = HttpContextFactory.Current.Items["RegSettings"] as Dictionary<int, Settings>;
        //            if (settings == null)
        //            {
        //                settings = new Dictionary<int, Settings>();
        //                HttpContextFactory.Current.Items.Add("RegSettings", settings);
        //            }
        //            var o = new Organization { OrganizationId = Util.CreateAccountCode, OrganizationName = "My Data" };
        //            o.RegistrationTypeId = RegistrationTypeCode.CreateAccount;
        //            if (!settings.ContainsKey(Util.CreateAccountCode))
        //                settings.Add(Util.CreateAccountCode, ParseSetting("AllowOnlyOne: true", Util.CreateAccountCode));
        //            return o;
        //        }

        private Dictionary<int, Settings> _settings;

        public Dictionary<int, Settings> settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = HttpContextFactory.Current.Items["RegSettings"] as Dictionary<int, Settings>;
                }

                if (_settings == null)
                {
                    ParseSettings();
                    _settings = HttpContextFactory.Current.Items["RegSettings"] as Dictionary<int, Settings>;
                }
                return _settings;
            }
        }

        public bool DisplayLogin()
        {
            return List.Count == 0 && !UserPeopleId.HasValue && (nologin == false || !AllowAnonymous);
        }

        public string LoginName
        {
            get
            {
                if (user != null)
                {
                    return user.Name;
                }

                return "anonymous";
            }
        }

        public string MeetingTime
        {
            get { return meeting().MeetingDate.ToString2("f"); }
        }

        public OnlineRegPersonModel last
        {
            get
            {
                if (_list.Count > 0)
                {
                    return _list[_list.Count - 1];
                }

                return null;
            }
        }

        public string qtesting
        {
            get { return testing == true ? "?testing=true" : ""; }
        }

        public bool IsCreateAccount()
        {
            if (org != null)
            {
                return org.RegistrationTypeId == RegistrationTypeCode.CreateAccount;
            }

            return false;
        }

        public bool IsEnded()
        {
            return IsEnded(masterorg) || IsEnded(org);
        }

        private bool IsEnded(Organization o)
        {
            if (o != null)
            {
                return o.ClassFilled == true;
            }

            return false;
        }

        public bool AllowReregister
        {
            get
            {
                if (!Orgid.HasValue)
                {
                    return false;
                }

                return settings.ContainsKey(Orgid.Value) && settings[Orgid.Value].AllowReRegister;
            }
        }

        public bool AllowAnonymous => _allowAnonymous(masterorgid) && _allowAnonymous(Orgid);

        private bool _allowAnonymous(int? id)
        {
            if (RegisterLinkMaster())
            {
                return false;
            }

            if (id.HasValue)
            {
                if (settings.ContainsKey(id.Value))
                {
                    return !settings[id.Value].DisallowAnonymous;
                }
            }

            return true;
        }

        private bool Filled(Organization o)
        {
            if (SupportMissionTrip)
            {
                return false;
            }

            if (o != null)
            {
                if ((o.ClassFilled ?? false) || (o.Limit > 0 && o.Limit <= o.RegLimitCount(DbUtil.Db)))
                {
                    return true;
                }
            }
            return false;
        }

        public string Filled()
        {
            return Filled(masterorg) || Filled(org) ? "registration is full" : "";
        }

        public bool NotAvailable()
        {
            if (SupportMissionTrip)
            {
                return false;
            }

            var dt = Util.Now;
            var dt1 = DateTime.Parse("1/1/1900");
            var dt2 = DateTime.Parse("1/1/2200");
            if (masterorgid.HasValue)
            {
                return masterorg.RegistrationClosed == true
                       || masterorg.OrganizationStatusId == OrgStatusCode.Inactive
                       || dt < (masterorg.RegStart ?? dt1)
                       || (dt > (masterorg.RegEnd ?? dt2) && !GoerSupporterId.HasValue);
            }

            return org.RegistrationClosed == true
                   || org.OrganizationStatusId == OrgStatusCode.Inactive
                   || dt < (org.RegStart ?? dt1)
                   || (dt > (org.RegEnd ?? dt2) && !GoerSupporterId.HasValue);
        }

        public bool NotActive()
        {
            var organization = _org ?? _masterOrg;
            return organization == null || organization.OrganizationStatusId == OrgStatusCode.Inactive;
        }

        public bool UserSelectsOrganization()
        {
            return masterorgid.HasValue && masterorg.RegistrationTypeId == RegistrationTypeCode.UserSelects;
        }

        public bool OnlyOneAllowed()
        {
            if (ManagingSubscriptions())
            {
                return true;
            }

            if (org != null)
            {
                var setting = settings[org.OrganizationId];
                return org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes
                       || org.RegistrationTypeId == RegistrationTypeCode.CreateAccount
                       || org.IsMissionTrip == true
                       || setting.AllowOnlyOne
                       || setting.AskVisible("AskTickets")
                       || ChoosingSlots()
                       || OnlineGiving()
                       || ManageGiving()
                       || SupportMissionTrip;
            }
            if (settings != null)
            {
                var q = from o in settings.Values
                        where o.AllowOnlyOne || o.AskVisible("AskTickets")
                        select o;
                return q.Any();
            }
            return false;
        }

        public bool RecordFamilyAttendance()
        {
            return org != null && org.RegistrationTypeId == RegistrationTypeCode.RecordFamilyAttendance;
        }

        public bool ChoosingSlots()
        {
            if (org != null)
            {
                return org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes;
            }

            return false;
        }

        public bool ManagingSubscriptions()
        {
            return masterorgid.HasValue && masterorg.RegistrationTypeId == RegistrationTypeCode.ManageSubscriptions;
        }

        public bool RegisterLinkMaster()
        {
            return org != null && org.RegistrationTypeId == RegistrationTypeCode.RegisterLinkMaster;
        }

        public bool OnlinePledge()
        {
            if (org != null)
            {
                return org.RegistrationTypeId == RegistrationTypeCode.OnlinePledge;
            }

            return false;
        }

        public bool ManageGiving()
        {
            if (org != null)
            {
                return org.RegistrationTypeId == RegistrationTypeCode.ManageGiving;
            }

            return false;
        }

        public bool OnlineGiving()
        {
            if (org != null)
            {
                return org.RegistrationTypeId == RegistrationTypeCode.OnlineGiving;
            }

            return false;
        }
        public bool ShouldPullSpecificFund()
        {
            return OnlineGiving()
                   && !AskDonation()
                   && settings.Any(vv => vv.Value.DonationFundId > 0);
        }

        public bool NoCreditCardsAllowed()
        {
            if (org != null)
            {
                return org.NoCreditCards == true;
            }

            return List.Any(p => p.org.NoCreditCards == true);
        }

        public bool AskDonation()
        {
            if (org != null)
            {
                return settings[org.OrganizationId].AskDonation;
            }

            if (settings == null)
            {
                return false;
            }

            return settings.Values.Any(o => o.AskDonation);
        }

        public bool AllowSaveProgress()
        {
            if (SupportMissionTrip)
            {
                return false;
            }

            if (UserPeopleId == null)
            {
                return false;
            }

            return SaveProgressChecked();
        }

        public bool SaveProgressChecked()
        {
            if (org != null)
            {
                return settings[org.OrganizationId].AllowSaveProgress;
            }

            if (settings == null)
            {
                return false;
            }

            if (masterorg?.RegistrationTypeId == RegistrationTypeCode.ComputeOrgByAge)
            {
                return false;
            }

            return settings.Values.Any(o => o.AllowSaveProgress);
        }

        public string DonationLabel()
        {
            if (org != null)
            {
                return settings[org.OrganizationId].DonationLabel;
            }

            return settings.Values.First(o => o.AskDonation).DonationLabel;
        }

        public string Header
        {
            get
            {
                if (masterorgid.HasValue)
                {
                    return masterorg.Title;
                }

                if (SupportMissionTrip)
                {
                    if (GoerId.HasValue)
                    {
                        var g = DbUtil.Db.LoadPersonById(GoerId.Value);
                        if (g != null)
                        {
                            return $"Support: {org.Title} ({g.Name})";
                        }
                    }
                    return "Support: " + org.Title;
                }
                if (settings != null && org != null && settings.ContainsKey(org.OrganizationId))
                {
                    return org.Title;
                }

                return org == null ? "Missing Org" : org.Title;
            }
        }

        public string DescriptionForPayment
        {
            get
            {
                if (masterorgid.HasValue)
                {
                    try
                    {
                        if (settings != null && settings.ContainsKey(masterorgid.Value))
                        {
                            var accountcode = settings[masterorgid.Value].AccountingCode;
                            if (accountcode.HasValue())
                            {
                                return $"{masterorg.OrganizationName} ({accountcode})";
                            }

                            return masterorg.OrganizationName;
                        }
                    }
                    catch (Exception)
                    {
                        if (masterorgid == null)
                        {
                            throw new Exception("masterorgid was null");
                        }

                        if (settings == null)
                        {
                            throw new Exception("settings was null");
                        }

                        if (settings[masterorgid.Value] == null)
                        {
                            throw new Exception("setting not found for masterorgid " + masterorgid.Value);
                        }

                        throw;
                    }
                }
                if (settings != null && org != null && settings.ContainsKey(org.OrganizationId))
                {
                    var accountcode = settings[org.OrganizationId].AccountingCode;
                    if (accountcode.HasValue())
                    {
                        return $"{org.OrganizationName} ({accountcode})";
                    }

                    return org.OrganizationName;
                }
                return org?.OrganizationName ?? "no org";
            }
        }

        public string SubmitInstructions()
        {
            Settings v;
            settings.TryGetValue(org?.OrganizationId ?? 0, out v);
            return v?.InstructionSubmit;
        }

        public string Instructions
        {
            get
            {
                if (masterorg != null)
                {
                    var setting1 = new Settings();
                    if (settings.ContainsKey(masterorg.OrganizationId))
                    {
                        setting1 = settings[masterorg.OrganizationId];
                    }

                    var setting2 = setting1;
                    if (last != null && last.org != null && settings.ContainsKey(last.org.OrganizationId))
                    {
                        setting1 = settings[last.org.OrganizationId];
                    }

                    return $@"
<div class=""instructions login"">{Util.PickFirst(setting1.InstructionLogin, setting2.InstructionLogin)}</div>
<div class=""instructions select"">{Util.PickFirst(setting1.InstructionSelect, setting2.InstructionSelect)}</div>
<div class=""instructions find"">{Util.PickFirst(setting1.InstructionFind, setting2.InstructionFind)}</div>
<div class=""instructions options"">{Util.PickFirst(setting1.InstructionOptions, setting2.InstructionOptions)}</div>
<div class=""instructions submit"">{Util.PickFirst(setting1.InstructionSubmit, setting2.InstructionSubmit)}</div>
<div class=""instructions special"">{Util.PickFirst(setting1.InstructionSpecial, setting2.InstructionSpecial)}</div>
<div class=""instructions sorry"">{Util.PickFirst(setting1.InstructionSorry, setting2.InstructionSorry)}</div>
";
                }
                var setting = new Settings();
                if (settings.ContainsKey(org.OrganizationId))
                {
                    setting = settings[org.OrganizationId];
                }

                if (setting.InstructionAll != null)
                {
                    if (setting.InstructionAll.ToString().HasValue())
                    {
                        return setting.InstructionAll.ToString();
                    }
                }

                var v = $"{setting.InstructionLogin}{setting.InstructionSelect}{setting.InstructionFind}{setting.InstructionOptions}{setting.InstructionSubmit}{setting.InstructionSpecial}{setting.InstructionSorry}";
                string ins = null;
                if (v.HasValue())
                {
                    ins = $@"<div class=""instructions login"">{setting.InstructionLogin}</div>
<div class=""instructions select"">{setting.InstructionSelect}</div>
<div class=""instructions find"">{setting.InstructionFind}</div>
<div class=""instructions options"">{setting.InstructionOptions}</div>
<div class=""instructions submit"">{setting.InstructionSubmit}</div>
<div class=""instructions special"">{setting.InstructionSpecial}</div>
<div class=""instructions sorry"">{setting.InstructionSorry}</div>";
                }

                if (ins.Contains("{ev:", ignoreCase: true))
                {
                    ins = DoReplaceForExtraValueCode(ins, last.person);
                }

                return ins + "\n";
            }
        }

        public static string DoReplaceForExtraValueCode(string text, Person p)
        {
            const string RE = @"{ev:(?<name>.+?)}";

            var re = new Regex(RE, RegexOptions.Singleline | RegexOptions.Multiline);
            var match = re.Match(text);
            while (match.Success)
            {
                var tag = match.Value;
                var name = match.Groups["name"].Value;

                if (p == null)
                {
                    text = text.Replace(tag, "");
                }
                else
                {
                    text = text.Replace(tag, p.GetExtra(name));
                }

                match = match.NextMatch();
            }
            return text;
        }

        public static string YouMustAgreeStatement(int? orgid) => Util.PickFirst(
            Organization.GetExtra(DbUtil.Db, orgid, "YouMustAgreeStatement"),
            "<p>You must agree to the terms above for you or your minor child before you can continue with confirmation.</p>");

        public string Terms
        {
            get
            {
                if (masterorgid.HasValue)
                {
                    if (settings.ContainsKey(masterorgid.Value))
                    {
                        return Util.PickFirst(settings[masterorgid.Value].Terms, "");
                    }
                }

                if (Orgid.HasValue)
                {
                    if (settings.ContainsKey(Orgid.Value))
                    {
                        return Util.PickFirst(settings[org.OrganizationId].Terms, "");
                    }
                }

                return "";
            }
        }

        public string TrackingCode
        {
            get
            {
                var trackcode = DbUtil.Db.ContentText("OnlineRegTrackCode", "");
                if (masterorgid.HasValue)
                {
                    if (settings.ContainsKey(masterorgid.Value))
                    {
                        return Util.PickFirst(settings[masterorgid.Value].ConfirmationTrackingCode, trackcode);
                    }
                }

                if (Orgid.HasValue)
                {
                    if (settings.ContainsKey(Orgid.Value))
                    {
                        return Util.PickFirst(settings[org.OrganizationId].ConfirmationTrackingCode, trackcode);
                    }
                }

                return "";
            }
        }

        public OnlineRegPersonModel LoadExistingPerson(int id, int index)
        {
            var person = DbUtil.Db.LoadPersonById(id);
            var p = new OnlineRegPersonModel
            {
                Campus = person.CampusId.GetValueOrDefault().ToString(),
                DateOfBirth = person.DOB,
                EmailAddress = person.EmailAddress.HasValue() ? person.EmailAddress : user.EmailAddress,
                FirstName = person.PreferredName,
                LastName = person.LastName,
                PeopleId = id,
                Phone = Util.PickFirst(person.CellPhone, person.HomePhone),
                orgid = Orgid,
                masterorgid = masterorgid,
                IsFamily = true,
                Found = true,
                IsValidForExisting = true,
            };
            if (p.LoggedIn && org != null)
            {
                var setting = settings[org.OrganizationId];
                if (setting.AllowReRegister)
                {
                    var om = org.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == id);
                    if (om != null)
                    {
                        p.RepopulateRegistration(om);
                    }
                }
            }
            return p;
        }

        internal string email;

        public string GetThankYouMessage()
        {
            var def = DbUtil.Db.ContentHtml("OnlineRegThanks", Resource1.OnlineRegModel_ThankYouMessage);

            string msg = null;
            if (masterorg != null)
            {
                var setting1 = new Settings();
                if (settings.ContainsKey(masterorg.OrganizationId))
                {
                    setting1 = settings[masterorg.OrganizationId];
                }

                var setting2 = setting1;
                if (last != null && last.org != null && settings.ContainsKey(last.org.OrganizationId))
                {
                    setting1 = settings[last.org.OrganizationId];
                }

                msg = Util.PickFirst(setting1.ThankYouMessage, setting2.ThankYouMessage, def);
            }
            else
            {
                var setting = new Settings();
                if (settings.ContainsKey(org.OrganizationId))
                {
                    setting = settings[org.OrganizationId];
                }

                msg = Util.PickFirst(setting.ThankYouMessage, def);
            }
            msg = msg.Replace("{org}", Header)
                     .Replace("{email}", Util.ObscureEmail(email))
                     .Replace("{url}", URL)
                     .Replace(WebUtility.UrlEncode("{url}"), URL);
            return msg;
        }

        public string GetFinishRegistrationButton()
        {
            string def = DbUtil.Db.Setting("FinishRegBtnText", "Finish Registration");

            string text = null;
            if (masterorg != null)
            {
                var setting1 = new Settings();
                if (settings.ContainsKey(masterorg.OrganizationId))
                {
                    setting1 = settings[masterorg.OrganizationId];
                }

                var setting2 = setting1;
                if (last?.org != null && settings.ContainsKey(last.org.OrganizationId))
                {
                    setting1 = settings[last.org.OrganizationId];
                }

                text = Util.PickFirst(setting1.FinishRegistrationButton, setting2.FinishRegistrationButton, def);
            }
            else
            {
                var setting = new Settings();
                if (settings.ContainsKey(org.OrganizationId))
                {
                    setting = settings[org.OrganizationId];
                }

                text = Util.PickFirst(setting.FinishRegistrationButton, def);
            }
            return text;
        }

        private int? timeOut;

        public int TimeOut
        {
            get
            {
                if (!timeOut.HasValue)
                {
                    timeOut = Util.IsDebug()
                        ? 1600000
                        : DbUtil.Db.Setting("RegTimeout", "180000").ToInt();
                    if (masterorgid.HasValue)
                    {
                        if (settings.ContainsKey(masterorgid.Value))
                        {
                            timeOut = settings[masterorgid.Value].TimeOut ?? timeOut;
                        }
                    }

                    if (Orgid.HasValue)
                    {
                        if (settings.ContainsKey(Orgid.Value))
                        {
                            timeOut = settings[org.OrganizationId].TimeOut ?? timeOut;
                        }
                    }
                }
                return timeOut.Value;
            }
        }

        public void UpdateDatum(bool completed = false, bool abandoned = false)
        {
            if (DatumId.HasValue)
            {
                Datum = DbUtil.Db.RegistrationDatas.Single(dd => dd.Id == DatumId);
                Datum.UserPeopleId = UserPeopleId;
            }
            else
            {
                // Don't create a new Datum if there is no data yet.
                if (List.Count == 0)
                {
                    return;
                }

                var p = FirstRegistrant;
                if (List.Count > 0 && !p.FirstName.HasValue() && !p.LastName.HasValue() && p.EmailAddress.HasValue())
                {
                    return;
                }

                Datum = new RegistrationDatum
                {
                    OrganizationId = masterorgid ?? _orgid,
                    UserPeopleId = UserPeopleId,
                    Stamp = Util.Now
                };
                DbUtil.Db.RegistrationDatas.InsertOnSubmit(Datum);
                DbUtil.Db.SubmitChanges();
                DatumId = Datum.Id;
            }
            Datum.Data = Util.Serialize<OnlineRegModel>(this);
            if (completed)
            {
                Datum.Completed = true;
            }

            if (abandoned)
            {
                Datum.Abandoned = true;
            }
            DbUtil.Db.SubmitChanges();
        }

        public bool Completed { get; set; }
        public int? DatumId { get; set; }

        [XmlIgnore]
        public RegistrationDatum Datum { get; set; }

        public static OnlineRegModel GetRegistrationFromDatum(int id)
        {
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed == null)
            {
                return null;
            }

            if (ed.Completed == true || ed.Abandoned == true)
            {
                return null;
            }

            try
            {
                var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
                m.Datum = ed;
                m.DatumId = id;
                m.Completed = ed.Completed ?? false;
                return m;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return null;
            }
        }

        public OnlineRegModel GetExistingRegistration(int pid)
        {
            if (!AllowSaveProgress())
            {
                return null;
            }

            var dt30 = DateTime.Now.AddDays(-30);
            var ed = (from e in DbUtil.Db.RegistrationDatas
                      let o = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == (masterorgid ?? _orgid))
                      where e.Stamp > (o.RegStart ?? dt30)
                      where e.OrganizationId == (masterorgid ?? _orgid)
                      where e.UserPeopleId == pid
                      where (e.Abandoned ?? false) == false
                      where (e.Completed ?? false) == false
                      orderby e.Stamp descending
                      select e).FirstOrDefault();
            return ed != null
                ? GetRegistrationFromDatum(ed.Id)
                : null;
        }

#if DEBUG
        public void DebugCleanUp()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                    where new[] { 828612, Util.UserPeopleId }.Contains(om.PeopleId)
                    where om.OrganizationId == Orgid
                    select om;
            //            var q = from om in DbUtil.Db.OrganizationMembers
            //                    where new[] {2192117,2192118}.Contains(om.OrganizationId)
            //                    select om;
            foreach (var om in q)
            {
                om.Drop(DbUtil.Db, DateTime.Now);
                DbUtil.Db.ExecuteCommand("DELETE dbo.EnrollmentTransaction WHERE PeopleId = {0} AND OrganizationId = {1}", om.PeopleId, om.OrganizationId);
            }
            DbUtil.Db.SubmitChanges();
            //                DbUtil.Db.ExecuteCommand(@"
            //DELETE dbo.EnrollmentTransaction WHERE PeopleId = 58207 AND OrganizationId = 2202
            //
            //IF OBJECT_ID('tempDbUtil.Db..#t') IS NOT NULL
            //   DROP TABLE #t
            //
            //SELECT c.ContributionId INTO #t
            //FROM dbo.Contribution c
            //JOIN dbo.BundleDetail d ON d.ContributionId = c.ContributionId
            //JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
            //WHERE CONVERT(DATE, h.ContributionDate) = CONVERT(DATE, GETDATE())
            //AND c.PeopleId = 58207
            //
            //DELETE dbo.BundleDetail
            //FROM dbo.BundleDetail d
            //JOIN #t ON #t.ContributionId = d.ContributionId
            //
            //DELETE dbo.Contribution
            //FROM dbo.Contribution c
            //JOIN #t ON #t.ContributionId = c.ContributionId
            //
            //DELETE dbo.GoerSenderAmounts
            //WHERE OrgId = 2202
            //AND SupporterId = 58207
            //");
        }
#endif

        // Make sure that we only use the 5 find fields and no previous data from a previous find attempt
        public OnlineRegPersonModel GetFreshFindInfo(int id)
        {
            var p = List[id];
            List[id] = new OnlineRegPersonModel
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                Phone = p.Phone,
                EmailAddress = p.EmailAddress,
                orgid = Orgid,
                masterorgid = masterorgid,
            };
            return List[id];
        }
        public void CancelRegistrant(int n)
        {
            HistoryAdd("Cancel id=" + n);
            List.RemoveAt(n);
            if (List.Count == 0)
            {
                List.Add(new OnlineRegPersonModel
                {
                    orgid = Orgid,
                    masterorgid = masterorgid,
#if DEBUG2
                    FirstName = "Another",
                    LastName = "Person",
                    DateOfBirth = "12/1/1955",
                    EmailAddress = "sombody@nowhere.com",
#endif
                });
            }
        }

        public bool RegistrantComplete
        {
            get
            {
                return last != null
                       && last.QuestionsOK
                       && last.FinishedFindingOrAddingRegistrant;
            }
        }


        private string selfsupportpaylink;
        public string MissionTripSelfSupportPaylink
        {
            get
            {
                if (org != null && org.IsMissionTrip == true && (GoerId == UserPeopleId || GoerId == 0))
                {
                    return selfsupportpaylink ?? (selfsupportpaylink = OrgMemberModel.GetPayLink(Orgid, UserPeopleId));
                }

                return null;
            }
        }

        public void Log(string action)
        {
            int? pid = null;
            if (List.Count > 0)
            {
                pid = List[0].PeopleId;
            }

            DbUtil.LogActivity("OnlineReg " + action, masterorgid ?? Orgid, UserPeopleId ?? pid, DatumId);
        }

        internal void StartOver()
        {
            HistoryAdd("startover");
            UpdateDatum(abandoned: true);
            DbUtil.Db.ExecuteCommand(@"
UPDATE dbo.RegistrationData
SET abandoned = 1
WHERE ISNULL(abandoned, 0) = 0
AND UserPeopleid = {0}
AND OrganizationId = {1}", Datum.UserPeopleId, Datum.OrganizationId);
        }

        internal string CheckExpiredOrCompleted()
        {
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == DatumId);
            if (ed?.Completed == true && Orgid.HasValue && !settings[Orgid.Value].AllowReRegister)
            {
                return "Registration Already Completed";
            }

            if (!AllowReregister && !AllowSaveProgress())
            {
                // Don't allow a submit to SubmitQuestions on an old form
                var re = new Regex("index (?<dt>[0-9/]* [0-9:]* [AP]M)", RegexOptions.IgnoreCase);
                var result = re.Match(History[0]).Groups["dt"].Value.ToDate();
                if (result.HasValue && DateTime.Now.Subtract(result.Value).TotalMinutes > 120)
                {
                    return "Registration Page has expired after 2 hours";
                }
            }
            return null;
        }

    }
}
