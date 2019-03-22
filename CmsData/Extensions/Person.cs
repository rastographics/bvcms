/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData.API;
using CmsData.Classes.GoogleCloudMessaging;
using CmsData.Codes;
using ImageData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using UtilityExtensions;

namespace CmsData
{
    public partial class Person : ITableWithExtraValues
    {
        public static int[] DiscClassStatusCompletedCodes =
        {
            NewMemberClassStatusCode.AdminApproval,
            NewMemberClassStatusCode.Attended,
            NewMemberClassStatusCode.ExemptedChild
        };

        public static int[] DropCodesThatDrop =
        {
            DropTypeCode.Administrative,
            DropTypeCode.AnotherDenomination,
            DropTypeCode.LetteredOut,
            DropTypeCode.Requested,
            DropTypeCode.Other,
        };

        public DateTime Now()
        {
            return Util.Now;
        }

        /* Origins
                10		Visit						Worship or BFClass Visit
                30		Referral					see Request
                40		Request						Task, use this for Referral too
                50		Deacon Telephone			Contact, type = phoned in
                60		Survey (EE)					Contact, EE
                70		Enrollment					Member of org
                80		Membership Decision			Contact, Type=Worship Visit
                90		Contribution				-1 peopleid in Excel with Name?
                98		Other						Task, use task description
                */
        public string CityStateZip => Util.FormatCSZ4(PrimaryCity, PrimaryState, PrimaryZip);

        public string CityStateZip5 => Util.FormatCSZ(PrimaryCity, PrimaryState, PrimaryZip);

        public string AddrCityStateZip => $"{PrimaryAddress}\n{CityStateZip}";

        public string Addr2CityStateZip => $"{PrimaryAddress2}\n{CityStateZip}";

        public string FullAddress
        {
            get
            {
                var sb = new StringBuilder(PrimaryAddress + "\n");
                if (PrimaryAddress2.HasValue())
                {
                    sb.AppendLine(PrimaryAddress2);
                }

                sb.Append(CityStateZip);
                return sb.ToString();
            }
        }

        public string SpouseName(CMSDataContext db)
        {
            if (SpouseId.HasValue)
            {
                var q = from p in db.People
                        where p.PeopleId == SpouseId
                        select p.Name;
                return q.SingleOrDefault();
            }
            return "";
        }

        public DateTime? BirthDate
        {
            get
            {
                DateTime dt;
                if (DateTime.TryParse(DOB, out dt))
                {
                    return dt;
                }

                return null;
            }
        }

        public void SetFamilyFromPersonPage(Family f)
        {
            family = f;
        }

        public string DOB
        {
            get { return FormatBirthday(BirthYr, BirthMonth, BirthDay, PeopleId); }
            set
            {
                // reset all values before replacing b/c replacement may be partial
                BirthDay = null;
                BirthMonth = null;
                BirthYear = null;
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    BirthDay = dt.Day;
                    BirthMonth = dt.Month;
                    if (Regex.IsMatch(value, @"\d+/\d+/\d+"))
                    {
                        BirthYear = dt.Year;
                    }
                }
                else
                {
                    int n;
                    if (int.TryParse(value, out n))
                    {
                        if (n >= 1 && n <= 12)
                        {
                            BirthMonth = n;
                        }
                        else
                        {
                            BirthYear = n;
                        }
                    }
                }
            }
        }

        public static string ParseBirthdate(string s)
        {
            int? birthDay = null;
            int? birthMonth = null;
            int? birthYear = null;
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
            {
                birthDay = dt.Day;
                birthMonth = dt.Month;
                if (Regex.IsMatch(s, @"\d+/\d+/\d+"))
                {
                    birthYear = dt.Year;
                }
            }
            else
            {
                int n;
                if (int.TryParse(s, out n))
                {
                    if (n >= 1 && n <= 12)
                    {
                        birthMonth = n;
                    }
                    else
                    {
                        birthYear = n;
                    }
                }
            }
            return FormatBirthday(birthYear, birthMonth, birthDay, null);
        }

        public DateTime? GetBirthdate()
        {
            DateTime dt;
            if (DateTime.TryParse(DOB, out dt))
            {
                return dt;
            }

            return null;
        }

        public int GetAge()
        {
            int years;
            var dt0 = GetBirthdate();
            if (!dt0.HasValue)
            {
                return -1;
            }

            var dt = dt0.Value;
            years = Util.Now.Year - dt.Year;
            if (Util.Now.Month < dt.Month || (Util.Now.Month == dt.Month && Util.Now.Day < dt.Day))
            {
                years--;
            }

            return years;
        }

        public static int? GetAge(int? y, int? m, int? d)
        {
            if (y == null)
            {
                return null;
            }
            //DateTime dt;
            //var s = $"{y:yyyy}-{m:MM}-{d:dd}";
            var dt = new DateTime(y.Value, m ?? 1, d ?? 1);
            //            if (!DateTime.TryParse(s, out dt))
            //                return null;
            var today = DateTime.Today;
            var age = today.Year - dt.Year;
            if (dt > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        public int? BirthYr
        {
            get
            {
                if (Util.Host == null || HttpContextFactory.Current == null)
                {
                    return BirthYear;
                }

                if (Util.UserPeopleId == PeopleId)
                {
                    return BirthYear;
                }

                if (IsHeadOfHouseholdFromMyDataPage)
                {
                    return BirthYear;
                }

                if (Age <= DbUtil.Db.Setting("NoBirthYearOverAge", "18").ToInt())
                {
                    return BirthYear;
                }

                if (HttpContextFactory.Current == null)
                {
                    return BirthYear;
                }

                if (HttpContextFactory.Current.User.IsInRole(DbUtil.Db.Setting("NoBirthYearRole", "")))
                {
                    return null;
                }

                return BirthYear;
            }
        }

        public static int? AgeDisplay(int? age, int? peopleid)
        {
            if (Util.Host == null || HttpContextFactory.Current == null)
            {
                return age;
            }

            if (Util.UserPeopleId == peopleid)
            {
                return age;
            }

            if (IsHeadOfHouseholdFromMyDataPage)
            {
                return age;
            }

            if (age <= DbUtil.Db.Setting("NoBirthYearOverAge", "18").ToInt())
            {
                return age;
            }

            if (HttpContextFactory.Current.User.IsInRole(DbUtil.Db.Setting("NoBirthYearRole", "")))
            {
                return null;
            }

            return age;
        }

        public static string AgeDisplay(string age, int? peopleid)
        {
            if (Util.Host == null || HttpContextFactory.Current == null)
            {
                return age;
            }

            if (Util.UserPeopleId == peopleid)
            {
                return age;
            }

            if (IsHeadOfHouseholdFromMyDataPage)
            {
                return age;
            }

            if (age.ToInt2() <= DbUtil.Db.Setting("NoBirthYearOverAge", "18").ToInt())
            {
                return age;
            }

            if (HttpContextFactory.Current.User.IsInRole(DbUtil.Db.Setting("NoBirthYearRole", "")))
            {
                return null;
            }

            return age;
        }

        private static int? Birthyear(int? y, int? age, int? peopleid)
        {
            if (Util.Host == null || HttpContextFactory.Current == null)
            {
                return y;
            }

            if (Util.UserPeopleId == peopleid)
            {
                return y;
            }

            if (IsHeadOfHouseholdFromMyDataPage)
            {
                return y;
            }

            if (age <= DbUtil.Db.Setting("NoBirthYearOverAge", "18").ToInt())
            {
                return y;
            }

            if (HttpContextFactory.Current.User.IsInRole(DbUtil.Db.Setting("NoBirthYearRole", "")))
            {
                return null;
            }

            return y;
        }

        public static string FormatBirthday(int? y, int? m, int? d, int? peopleid)
        {
            return FormatBirthday(y, m, d, "", peopleid);
        }

        public static string FormatBirthday(int? y, int? m, int? d, string def, int? peopleid = null)
        {
            try
            {
                var yr = Birthyear(y, GetAge(y, m, d), peopleid);
                if (m.HasValue && d.HasValue)
                {
                    if (!yr.HasValue)
                    {
                        return new DateTime(2000, m.Value, d.Value).ToString("m");
                    }
                    else
                    {
                        return new DateTime(yr.Value, m.Value, d.Value).ToString("d");
                    }
                }

                if (yr.HasValue)
                {
                    if (m.HasValue)
                    {
                        return new DateTime(yr.Value, m.Value, 1).ToString("y");
                    }
                    else
                    {
                        return y.ToString();
                    }
                }
            }
            catch (Exception)
            {
                return $"bad date {m ?? 0}/{d ?? 0}/{y ?? 0}";
            }
            return def;
        }


        public void MovePersonStuff(CMSDataContext db, int targetid)
        {
            var toperson = db.People.Single(p => p.PeopleId == targetid);
            foreach (var om in this.OrganizationMembers)
            {
                var om2 = OrganizationMember.InsertOrgMembers(db, om.OrganizationId, targetid, om.MemberTypeId,
                    om.EnrollmentDate ?? DateTime.Now, om.InactiveDate, om.Pending ?? false);
                db.UpdateMainFellowship(om.OrganizationId);
                om2.CreatedBy = om.CreatedBy;
                om2.CreatedDate = om.CreatedDate;
                om2.AttendPct = om.AttendPct;
                om2.AttendStr = om.AttendStr;
                om2.LastAttended = om.LastAttended;
                om2.Request = om.Request;
                om2.Grade = om.Grade;
                om2.Amount = om.Amount;
                om2.TranId = om.TranId;
                om2.AmountPaid = om.AmountPaid;
                om2.PayLink = om.PayLink;
                om2.Moved = om.Moved;
                om2.InactiveDate = om.InactiveDate;
                om.Pending = om.Pending;
                om.Request = om.Request;
                om2.RegisterEmail = om.RegisterEmail;
                om2.ShirtSize = om.ShirtSize;
                om2.Tickets = om.Tickets;
                om2.UserData = om.UserData;
                om2.OnlineRegData = om.OnlineRegData;
                db.SubmitChanges();
                foreach (var m in om.OrgMemMemTags)
                {
                    if (om2.OrgMemMemTags.All(mm => mm.MemberTagId != m.MemberTagId))
                    {
                        om2.OrgMemMemTags.Add(new OrgMemMemTag { MemberTagId = m.MemberTagId });
                    }
                }

                db.SubmitChanges();
                db.OrgMemMemTags.DeleteAllOnSubmit(om.OrgMemMemTags);
                foreach (var m in om.OrgMemberExtras)
                {
                    om2.AddEditExtraValue(m.Field, m.StrValue, m.DateValue, m.Data, m.BitValue, m.IntValue, m.DateValue);
                }

                db.SubmitChanges();
                db.OrgMemberExtras.DeleteAllOnSubmit(om.OrgMemberExtras);
                db.SubmitChanges();
                TrySubmit(db, $"Organizations (orgid:{om.OrganizationId})");
            }
            db.OrganizationMembers.DeleteAllOnSubmit(this.OrganizationMembers);
            TrySubmit(db, "DeletingMemberships");

            foreach (var et in this.EnrollmentTransactions)
            {
                et.PeopleId = targetid;
            }

            TrySubmit(db, "EnrollmentTransactions");

            var tplist = TransactionPeople.ToList();
            if (tplist.Any())
            {
                db.TransactionPeople.DeleteAllOnSubmit(TransactionPeople);
                TrySubmit(db, "Delete TransactionPeople");
                foreach (var tp in tplist)
                {
                    if (
                        !db.TransactionPeople.Any(
                            tt => tt.Id == tp.Id && tt.PeopleId == targetid && tt.OrgId == tp.OrgId))
                    {
                        db.TransactionPeople.InsertOnSubmit(new TransactionPerson
                        {
                            OrgId = tp.OrgId,
                            Amt = tp.Amt,
                            Id = tp.Id,
                            PeopleId = targetid
                        });
                    }
                }

                TrySubmit(db, "Add TransactionPeople");
            }
            var q = from a in db.Attends
                    where a.AttendanceFlag == true
                    where a.PeopleId == this.PeopleId
                    select a;
            foreach (var a in q)
            {
                Attend.RecordAttendance(db, targetid, a.MeetingId, true);
            }

            db.AttendUpdateN(targetid, 10);

            foreach (var c in this.Contributions)
            {
                c.PeopleId = targetid;
            }

            TrySubmit(db, "Contributions");

            foreach (var u in this.Users)
            {
                u.PeopleId = targetid;
            }

            TrySubmit(db, "Users");

            if (this.Volunteers.Any() && !toperson.Volunteers.Any())
            {
                foreach (var v in this.Volunteers)
                {
                    var vv = new Volunteer
                    {
                        PeopleId = targetid,
                        Children = v.Children,
                        Comments = v.Comments,
                        Leader = v.Leader,
                        ProcessedDate = v.ProcessedDate,
                        Standard = v.Standard,
                        StatusId = v.StatusId,
                    };
                    db.Volunteers.InsertOnSubmit(vv);
                }
            }

            TrySubmit(db, "Volunteers");

            foreach (var v in this.VolunteerForms)
            {
                v.PeopleId = targetid;
            }

            TrySubmit(db, "VolunteerForms");

            foreach (var c in this.contactsMade)
            {
                var cp = db.Contactors.SingleOrDefault(c2 => c2.PeopleId == targetid && c.ContactId == c2.ContactId);
                if (cp == null)
                {
                    c.contact.contactsMakers.Add(new Contactor { PeopleId = targetid });
                }

                db.Contactors.DeleteOnSubmit(c);
            }
            TrySubmit(db, "ContactsMade");

            foreach (var c in this.contactsHad)
            {
                var cp = db.Contactees.SingleOrDefault(c2 => c2.PeopleId == targetid && c.ContactId == c2.ContactId);
                if (cp == null)
                {
                    c.contact.contactees.Add(new Contactee { PeopleId = targetid });
                }

                db.Contactees.DeleteOnSubmit(c);
            }
            TrySubmit(db, "ContactsHad");

            foreach (var e in this.PeopleExtras)
            {
                var field = e.Field;
                FindExisting:
                var cp = db.PeopleExtras.FirstOrDefault(c2 => c2.PeopleId == targetid && c2.Field == field);
                if (cp != null)
                {
                    field = field + "_mv";
                    goto FindExisting;
                }
                var e2 = new PeopleExtra
                {
                    PeopleId = targetid,
                    Field = field,
                    Data = e.Data,
                    StrValue = e.StrValue,
                    DateValue = e.DateValue,
                    IntValue = e.IntValue,
                    IntValue2 = e.IntValue2,
                    IsAttributes = e.IsAttributes,
                    TransactionTime = e.TransactionTime
                };
                db.PeopleExtras.InsertOnSubmit(e2);
                TrySubmit(db, $"ExtraValues (pid={e2.PeopleId},field={e2.Field})");
            }
            db.PeopleExtras.DeleteAllOnSubmit(PeopleExtras);
            TrySubmit(db, "Delete ExtraValues");

            var torecreg = toperson.RecRegs.SingleOrDefault();
            var frrecreg = RecRegs.SingleOrDefault();
            if (torecreg == null && frrecreg != null)
            {
                frrecreg.PeopleId = targetid;
            }

            if (torecreg != null && frrecreg != null)
            {
                torecreg.Comments = frrecreg.Comments + "\n" + torecreg.Comments;
                if (frrecreg.ShirtSize.HasValue())
                {
                    torecreg.ShirtSize = frrecreg.ShirtSize;
                }

                if (frrecreg.MedicalDescription.HasValue())
                {
                    torecreg.MedicalDescription = frrecreg.MedicalDescription;
                }

                if (frrecreg.Doctor.HasValue())
                {
                    torecreg.Doctor = frrecreg.Doctor;
                }

                if (frrecreg.Docphone.HasValue())
                {
                    torecreg.Docphone = frrecreg.Docphone;
                }

                if (frrecreg.MedAllergy.HasValue)
                {
                    torecreg.MedAllergy = frrecreg.MedAllergy;
                }

                if (frrecreg.Tylenol.HasValue)
                {
                    torecreg.Tylenol = frrecreg.Tylenol;
                }

                if (frrecreg.Robitussin.HasValue)
                {
                    torecreg.Robitussin = frrecreg.Robitussin;
                }

                if (frrecreg.Advil.HasValue)
                {
                    torecreg.Advil = frrecreg.Advil;
                }

                if (frrecreg.Maalox.HasValue)
                {
                    torecreg.Maalox = frrecreg.Maalox;
                }

                if (frrecreg.Insurance.HasValue())
                {
                    torecreg.Insurance = frrecreg.Insurance;
                }

                if (frrecreg.Policy.HasValue())
                {
                    torecreg.Policy = frrecreg.Policy;
                }

                if (frrecreg.Mname.HasValue())
                {
                    torecreg.Mname = frrecreg.Mname;
                }

                if (frrecreg.Fname.HasValue())
                {
                    torecreg.Fname = frrecreg.Fname;
                }

                if (frrecreg.Emcontact.HasValue())
                {
                    torecreg.Emcontact = frrecreg.Emcontact;
                }

                if (frrecreg.Emphone.HasValue())
                {
                    torecreg.Emphone = frrecreg.Emphone;
                }

                if (frrecreg.ActiveInAnotherChurch.HasValue)
                {
                    torecreg.ActiveInAnotherChurch = frrecreg.ActiveInAnotherChurch;
                }
            }
            TrySubmit(db, "RegReg");

            var mg = db.ManagedGivings.FirstOrDefault(mm => mm.PeopleId == targetid);
            if (mg == null)
            {
                var v = this.ManagedGivings.FirstOrDefault();
                if (v != null)
                {
                    db.ManagedGivings.InsertOnSubmit(new ManagedGiving()
                    {
                        Day1 = v.Day1,
                        Day2 = v.Day2,
                        EveryN = v.EveryN,
                        NextDate = v.NextDate,
                        PeopleId = targetid,
                        Period = v.Period,
                        SemiEvery = v.SemiEvery,
                        StartWhen = v.StartWhen,
                        StopAfter = v.StopAfter,
                        StopWhen = v.StopWhen,
                        Type = v.Type,
                    });
                    var qq = from ra in db.RecurringAmounts
                             where ra.PeopleId == PeopleId
                             select ra;
                    foreach (var ra in qq)
                    {
                        db.RecurringAmounts.InsertOnSubmit(
                            new RecurringAmount()
                            {
                                PeopleId = targetid,
                                Amt = ra.Amt,
                                FundId = ra.FundId,
                            });
                    }
                }
                TrySubmit(db, "ManagedGivings");
            }

            var pi = db.PaymentInfos.FirstOrDefault(mm => mm.PeopleId == targetid);
            if (pi == null) // the target has none
            {
                foreach (var i in PaymentInfos)
                {
                    DbUtil.Db.PaymentInfos.InsertOnSubmit(
                        new PaymentInfo
                        {
                            Address = i.Address,
                            Address2 = i.Address2,
                            AuNetCustId = i.AuNetCustId,
                            AuNetCustPayId = i.AuNetCustPayId,
                            AuNetCustPayBankId = i.AuNetCustPayBankId,
                            BluePayCardVaultId = i.BluePayCardVaultId,
                            City = i.City,
                            Country = i.Country,
                            Expires = i.Expires,
                            FirstName = i.FirstName,
                            LastName = i.LastName,
                            MaskedAccount = i.MaskedAccount,
                            MaskedCard = i.MaskedCard,
                            MiddleInitial = i.MiddleInitial,
                            PeopleId = targetid,
                            Phone = i.Phone,
                            PreferredGivingType = i.PreferredGivingType,
                            PreferredPaymentType = i.PreferredPaymentType,
                            Routing = i.Routing,
                            SageBankGuid = i.SageBankGuid,
                            SageCardGuid = i.SageCardGuid,
                            State = i.State,
                            Suffix = i.Suffix,
                            Testing = i.Testing,
                            Zip = i.Zip,
                            TbnBankVaultId = i.TbnBankVaultId,
                            TbnCardVaultId = i.TbnCardVaultId
                        });
                }
            }

            TrySubmit(db, "PaymentInfos");

            foreach (var bc in this.BackgroundChecks)
            {
                bc.PeopleID = targetid;
            }

            TrySubmit(db, "BackgroundChecks");

            foreach (var c in this.CheckInTimes)
            {
                c.PeopleId = targetid;
            }

            TrySubmit(db, "CheckinTimes");

            db.ExecuteCommand(@"
UPDATE dbo.GoerSupporter SET GoerId = {1} WHERE GoerId = {0};
UPDATE dbo.GoerSupporter SET SupporterId = {1} WHERE SupporterId = {0};
UPDATE dbo.GoerSenderAmounts SET GoerId = {1} WHERE GoerId = {0};
UPDATE dbo.GoerSenderAmounts SET SupporterId = {1} WHERE SupporterId = {0}", PeopleId, targetid);
        }

        private void TrySubmit(CMSDataContext db, string message)
        {
            try
            {
                db.SubmitChanges();
            }
            catch (SqlException ex)
            {
                throw new Exception("Merge Error: " + message + " \nFrom SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Merge Error: " + message + " \n" + ex.Message);
            }
        }

        public bool Deceased => DeceasedDate.HasValue;
        public string FromEmail => Util.FullEmail(EmailAddress, Name);

        public string FromEmail2 => Util.FullEmail(EmailAddress2, Name);

        private static void NameSplit(string name, out string first, out string last)
        {
            first = "";
            last = "";
            if (!name.HasValue())
            {
                return;
            }

            var a = name.Trim().Split(' ');
            if (a.Length > 1)
            {
                first = a[0];
                last = a[1];
            }
            else
            {
                last = a[0];
            }
        }

        public static Person Add(Family fam, int position, Tag tag, string name, string dob, bool married, int gender,
                                 int originId, int? entryPointId)
        {
            string first, last;
            NameSplit(name, out first, out last);
            if (!first.HasValue() || married)
            {
                switch (gender)
                {
                    case 0:
                        first = "A";
                        break;
                    case 1:
                        if (!first.HasValue())
                        {
                            first = "Husbander";
                        }

                        break;
                    case 2:
                        first = "Wifey";
                        break;
                }
            }

            return Add(fam, position, tag, first, null, last, dob, married, gender, originId, entryPointId);
        }

        public static Person Add(Family fam,
                                 int position,
                                 Tag tag,
                                 string firstname,
                                 string nickname,
                                 string lastname,
                                 string dob,
                                 int marriedCode,
                                 int gender,
                                 int originId,
                                 int? entryPointId)
        {
            return Add(DbUtil.Db, true, fam, position, tag, firstname, nickname, lastname, dob, marriedCode, gender,
                originId, entryPointId);
        }

        // Used for Conversions
        public static Person Add(CMSDataContext db, Family fam, string firstname, string nickname, string lastname, DateTime? dob)
        {
            return Add(db, false, fam, 20, null, firstname, nickname, lastname, dob.ToString2("M/d/yyyy"), 0, 0, 0, 0);
        }

        public static Person Add(CMSDataContext db, bool sendNotices, Family fam, int position, Tag tag,
                                 string firstname, string nickname, string lastname, string dob, int marriedCode,
                                 int gender, int originId, int? entryPointId, bool testing = false)
        {
            var p = new Person { CreatedDate = Util.Now, CreatedBy = Util.UserId };
            db.People.InsertOnSubmit(p);
            p.PositionInFamilyId = position;
            p.AddressTypeId = 10;

            p.FirstName = firstname.HasValue() ? firstname.Trim().ToProper().Truncate(25) : "";

            if (nickname.HasValue())
            {
                p.NickName = nickname.Trim().ToProper().Truncate(15);
            }

            p.LastName = lastname.HasValue() ? lastname.Trim().ToProper().Truncate(100) : "?";

            p.GenderId = gender;
            if (p.GenderId == 99)
            {
                p.GenderId = 0;
            }

            p.MaritalStatusId = marriedCode;

            if (Util.BirthDateValid(dob, out var dt))
            {
                if (dt.Year == Util.SignalNoYear)
                {
                    p.BirthDay = dt.Day;
                    p.BirthMonth = dt.Month;
                    p.BirthYear = null;
                }
                else
                {
                    while (dt.Year < 1900)
                    {
                        dt = dt.AddYears(100);
                    }

                    if (dt > Util.Now)
                    {
                        dt = dt.AddYears(-100);
                    }

                    p.BirthDay = dt.Day;
                    p.BirthMonth = dt.Month;
                    p.BirthYear = dt.Year;
                }
                if (p.GetAge() < 18 && marriedCode == 0)
                {
                    p.MaritalStatusId = MaritalStatusCode.Single;
                }
            }
            // I think this else statement is no longer necessary
            else if (DateTime.TryParse(dob, out dt))
            {
                p.BirthDay = dt.Day;
                p.BirthMonth = dt.Month;
                if (Regex.IsMatch(dob, @"\d+[-/]\d+[-/]\d+"))
                {
                    p.BirthYear = dt.Year;
                    while (p.BirthYr < 1900)
                    {
                        p.BirthYear += 100;
                    }

                    if (p.GetAge() < 18 && marriedCode == 0)
                    {
                        p.MaritalStatusId = MaritalStatusCode.Single;
                    }
                }
            }

            p.MemberStatusId = MemberStatusCode.JustAdded;
            if (fam == null)
            {
                fam = new Family();
                db.Families.InsertOnSubmit(fam);
                p.Family = fam;
            }
            else
            {
                fam.People.Add(p);
            }

            tag?.PersonTags.Add(new TagPerson { Person = p });

            p.OriginId = originId;
            p.EntryPointId = entryPointId;
            p.FixTitle();
            if (db.Setting("ElectronicStatementDefault", "false").Equal("true"))
            {
                p.ElectronicStatement = true;
            }

            if (!testing)
            {
                db.SubmitChanges();
            }

            if (!sendNotices)
            {
                return p;
            }

            if (Util.UserPeopleId.HasValue
                && Util.UserPeopleId.Value != db.NewPeopleManagerId
                && HttpContextFactory.Current.User.IsInRole("Access")
                && !HttpContextFactory.Current.User.IsInRole("OrgMembersOnly")
                && !HttpContextFactory.Current.User.IsInRole("OrgLeadersOnly"))
            {
                Task.AddNewPerson(db, p.PeopleId);
            }
            else
            {
                var np = db.GetNewPeopleManagers();
                if (np != null && Util.AdminMail != null)
                {
                    db.Email(Util.AdminMail, np,
                        $"Just Added Person on {db.Host}",
                        $"<a href='{db.ServerLink("/Person2/" + p.PeopleId)}'>{p.Name}</a>");
                }
            }            
            return p;
        }

        public static Person Add(Family fam, int position, Tag tag, string firstname, string nickname, string lastname,
                                 string dob, bool married, int gender, int originId, int? entryPointId)
        {
            return Add(fam, position, tag, firstname, nickname, lastname, dob, married ? 20 : 10, gender, originId,
                entryPointId);
        }

        public void FixTitle()
        {
            if (TitleCode.HasValue())
            {
                return;
            }

            TitleCode = ComputeTitle();
        }

        public string ComputeTitle()
        {
            if (GenderId == 1)
            {
                return "Mr.";
            }

            if (GenderId == 2)
            {
                if (MaritalStatusId == 20 || MaritalStatusId == 50)
                {
                    return "Mrs.";
                }
                else
                {
                    return "Ms.";
                }
            }

            return null;
        }

        public string OptOutKey(string fromEmail)
        {
            return Util.EncryptForUrl($"{PeopleId}|{fromEmail}");
        }

        public static bool ToggleTag(int peopleId, string tagName, int? ownerId, int tagTypeId)
        {
            var db = DbUtil.Db;
            var tag = db.FetchOrCreateTag(tagName, ownerId, tagTypeId);
            if (tag == null)
            {
                throw new Exception("ToggleTag, tag '{0}' not found");
            }

            var tp = db.TagPeople.SingleOrDefault(t => t.Id == tag.Id && t.PeopleId == peopleId);
            if (tp == null)
            {
                tag.PersonTags.Add(new TagPerson { PeopleId = peopleId });
                return true;
            }
            db.TagPeople.DeleteOnSubmit(tp);
            return false;
        }

        public static void Tag(CMSDataContext db, int peopleId, string tagName, int? ownerId, int tagTypeId)
        {
            var tag = db.FetchOrCreateTag(tagName, ownerId, tagTypeId);
            var tp = db.TagPeople.SingleOrDefault(t => t.Id == tag.Id && t.PeopleId == peopleId);
            var isperson = db.People.Count(p => p.PeopleId == peopleId) > 0;
            if (tp == null && isperson)
            {
                tag.PersonTags.Add(new TagPerson { PeopleId = peopleId });
            }
        }

        public static void UnTag(CMSDataContext db, int peopleId, string tagName, int? ownerId, int tagTypeId)
        {
            var tag = db.FetchOrCreateTag(tagName, ownerId, tagTypeId);
            var tp = db.TagPeople.SingleOrDefault(t => t.Id == tag.Id && t.PeopleId == peopleId);
            if (tp != null)
            {
                db.TagPeople.DeleteOnSubmit(tp);
            }
        }

        partial void OnNickNameChanged()
        {
            if (NickName != null && NickName.Trim() == String.Empty)
            {
                NickName = null;
            }
        }

        private bool decisionTypeIdChanged;
        public bool DecisionTypeIdChanged => decisionTypeIdChanged;

        partial void OnDecisionTypeIdChanged()
        {
            decisionTypeIdChanged = true;
        }

        private bool newMemberClassStatusIdChanged;
        public bool NewMemberClassStatusIdChanged => newMemberClassStatusIdChanged;

        partial void OnNewMemberClassStatusIdChanged()
        {
            newMemberClassStatusIdChanged = true;
        }

        private bool baptismStatusIdChanged;
        public bool BaptismStatusIdChanged => baptismStatusIdChanged;

        partial void OnBaptismStatusIdChanged()
        {
            baptismStatusIdChanged = true;
        }

        private bool deceasedDateChanged;
        public bool DeceasedDateChanged => deceasedDateChanged;

        partial void OnDeceasedDateChanged()
        {
            deceasedDateChanged = true;
        }

        private bool dropCodeIdChanged;
        public bool DropCodeIdChanged => dropCodeIdChanged;

        partial void OnDropCodeIdChanged()
        {
            dropCodeIdChanged = true;
        }

        private bool? canUserEditAll;

        public bool CanUserEditAll
        {
            get
            {
                if (!canUserEditAll.HasValue)
                {
                    canUserEditAll = HttpContextFactory.Current.User.IsInRole("Edit");
                }

                return canUserEditAll.Value;
            }
        }

        private bool? canUserEditFamilyAddress;

        public bool CanUserEditFamilyAddress
        {
            get
            {
                if (!canUserEditFamilyAddress.HasValue)
                {
                    canUserEditFamilyAddress = CanUserEditAll
                                               || Util.UserPeopleId == Family.HeadOfHouseholdId
                                               || Util.UserPeopleId == Family.HeadOfHouseholdSpouseId;
                }

                return canUserEditFamilyAddress.Value;
            }
        }

        private bool? canUserEditCampus;

        public bool CanUserEditCampus
        {
            get
            {
                if (CanUserEditAll)
                {
                    if (DbUtil.Db.Setting("EnforceEditCampusRole", "false").ToBool())
                    {
                        return ((HttpContextFactory.Current.User.IsInRole("EditCampus")) ||
                                Util.UserPeopleId == Family.HeadOfHouseholdId
                                || Util.UserPeopleId == Family.HeadOfHouseholdSpouseId);
                    }
                    else
                    {
                        return (true);
                    }
                }

                if (!canUserEditCampus.HasValue)
                {
                    canUserEditCampus = CanUserEditFamilyAddress &&
                                        DbUtil.Db.Setting("MyDataCanEditCampus", "false").ToBool();
                }

                if (DbUtil.Db.Setting("EnforceEditCampusRole", "false").ToBool())
                {
                    canUserEditCampus = (canUserEditCampus.Value) || (HttpContextFactory.Current.User.IsInRole("EditCampus"));
                }

                return canUserEditCampus.Value;
            }
        }

        private bool? canUserEditBasic;

        public bool CanUserEditBasic
        {
            get
            {
                if (!canUserEditBasic.HasValue)
                {
                    canUserEditBasic = CanUserEditFamilyAddress
                                       || Util.UserPeopleId == PeopleId;
                }

                return canUserEditBasic.Value;
            }
        }

        private bool? canUserSee;

        public bool CanUserSee
        {
            get
            {
                if (!canUserSee.HasValue)
                {
                    canUserSee = CanUserEditBasic
                                 || Family.People.Any(m => m.PeopleId == Util.UserPeopleId);
                }

                return canUserSee.Value;
            }
        }

        private bool? canUserSeeGiving;

        public bool CanUserSeeGiving
        {
            get
            {
                if (!canUserSeeGiving.HasValue)
                {
                    var sameperson = Util.UserPeopleId == PeopleId;
                    var infinance = (HttpContextFactory.Current.User.IsInRole("Finance") && !HttpContextFactory.Current.User.IsInRole("FundManager"))
                                    && ((string)HttpContextFactory.Current.Session["testnofinance"]) != "true";
                    var ishead = (new int?[]
                        {
                            Family.HeadOfHouseholdId,
                            Family.HeadOfHouseholdSpouseId
                        })
                        .Contains(Util.UserPeopleId);
                    canUserSeeGiving = sameperson || infinance || ishead;
                }
                return canUserSeeGiving.Value;
            }
        }

        private bool? canUserSeeEmails;

        public bool CanUserSeeEmails
        {
            get
            {
                if (!canUserSeeEmails.HasValue)
                {
                    var sameperson = Util.UserPeopleId == PeopleId;
                    var ishead = (new int?[]
                        {
                            Family.HeadOfHouseholdId,
                            Family.HeadOfHouseholdSpouseId
                        })
                        .Contains(Util.UserPeopleId);
                    canUserSeeEmails = sameperson || ishead || HttpContextFactory.Current.User.IsInRole("Access");
                }
                return canUserSeeEmails.Value;
            }
        }

        public RecReg GetRecReg()
        {
            var rr = RecRegs.SingleOrDefault();
            if (rr == null)
            {
                return new RecReg();
            }

            return rr;
        }

        public RecReg SetRecReg()
        {
            var rr = RecRegs.SingleOrDefault();
            if (rr == null)
            {
                rr = new RecReg();
                RecRegs.Add(rr);
            }
            return rr;
        }

        public void AddMedical(string s)
        {
            var rr = SetRecReg();
            if (rr.MedicalDescription.HasValue())
            {
                rr.MedicalDescription += "\n" + s;
            }
            else
            {
                rr.MedicalDescription = s;
            }
        }

        private List<ChangeDetail> psbDefault;
        private Family family;

        public void UpdateValue(string field, object value)
        {
            if (psbDefault == null)
            {
                psbDefault = new List<ChangeDetail>();
            }

            this.UpdateValue(psbDefault, field, value);
        }

        public void UpdateValueFromText(string field, string value)
        {
            if (psbDefault == null)
            {
                psbDefault = new List<ChangeDetail>();
            }

            this.UpdateValueFromText(psbDefault, field, value);
        }

        public void UpdateValueFromText(List<ChangeDetail> psb, string field, string value)
        {
            value = value.TrimEnd();
            var o = Util.GetProperty(this, field);
            o = (o as string)?.TrimEnd();
            if (o == null && value == null)
            {
                return;
            }

            if (o as int? == value.ToInt())
            {
                return;
            }

            var i = o as int?;
            if (i != null)
            {
                if (i == value.ToInt2())
                {
                    return;
                }
            }

            if (o != null && o.Equals(value))
            {
                return;
            }

            if (o == null && value is string && !((string)value).HasValue())
            {
                return;
            }

            if (value == null && o is string && !((string)o).HasValue())
            {
                return;
            }
            //psb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            psb.Add(new ChangeDetail(field, o, value));
            Util.SetPropertyFromText(this, field, value);
        }

        public void LogChanges(CMSDataContext db)
        {
            if (psbDefault != null)
            {
                LogChanges(db, psbDefault, Util.UserPeopleId ?? 0);
            }
        }

        public void LogChanges(CMSDataContext db, int userPeopleId)
        {
            if (psbDefault != null)
            {
                LogChanges(db, psbDefault, userPeopleId);
            }
        }

        public void LogChanges(CMSDataContext db, List<ChangeDetail> changes)
        {
            LogChanges(db, changes, Util.UserPeopleId ?? 0);
        }

        public void LogChanges(CMSDataContext db, List<ChangeDetail> changes, int userPeopleId)
        {
            if (changes.Count > 0)
            {
                var c = new ChangeLog
                {
                    UserPeopleId = userPeopleId,
                    PeopleId = PeopleId,
                    Field = "Basic Info",
                    Created = Util.Now
                };
                db.ChangeLogs.InsertOnSubmit(c);
                c.ChangeDetails.AddRange(changes.Where(x => db.ChangeDetails.GetOriginalEntityState(x) == null));
            }
        }

        public void LogPictureUpload(CMSDataContext db, int userPeopleId)
        {
            var c = new ChangeLog
            {
                UserPeopleId = userPeopleId,
                PeopleId = PeopleId,
                Field = "Basic Info",
                Created = Util.Now
            };
            db.ChangeLogs.InsertOnSubmit(c);
            c.ChangeDetails.Add(new ChangeDetail("Picture", null, "(new upload)"));
            var np = db.GetNewPeopleManagers();
            if (np != null)
            {
                db.EmailRedacted(db.Setting("AdminMail", ConfigurationManager.AppSettings["supportemail"]), np,
                    "Picture Uploaded on " + Util.Host,
                    $"{Util.UserName} Uploaded a picture for <a href=\"{db.ServerLink($"/Person2/{PeopleId}")}\">{Name}</a><br />\n");
            }
        }

        public override string ToString()
        {
            return Name + " (" + PeopleId + ")";
        }

        public void SetExtra(string field, string value)
        {
            var e = PeopleExtras.FirstOrDefault(ee => ee.Field == field);
            if (e == null)
            {
                e = new PeopleExtra { Field = field.Trim(), PeopleId = PeopleId, TransactionTime = Util.Now };
                this.PeopleExtras.Add(e);
            }
            e.StrValue = value;
        }

        public string GetExtra(string field)
        {
            var e = PeopleExtras.SingleOrDefault(ee => ee.Field == field);
            if (e == null)
            {
                return "";
            }

            if (e.StrValue.HasValue())
            {
                return e.StrValue;
            }

            if (e.Data.HasValue())
            {
                return e.Data;
            }

            if (e.DateValue.HasValue)
            {
                return e.DateValue.FormatDate();
            }

            if (e.IntValue.HasValue)
            {
                return e.IntValue.ToString();
            }

            return e.BitValue.ToString();
        }

        public PeopleExtra GetExtraValue(string field)
        {
            if (!field.HasValue())
            {
                field = "blank";
            }

            field = field.Trim();

            var ev = PeopleExtras.AsEnumerable().FirstOrDefault(ee => ee.Field == field);

            if (ev == null)
            {
                ev = new PeopleExtra
                {
                    Field = field.Trim(),
                    TransactionTime = Util.Now
                };
                PeopleExtras.Add(ev);
            }
            return ev;
        }

        public void RemoveExtraValue(CMSDataContext db, string field)
        {
            var ev = (from ee in db.PeopleExtras
                      where ee.Field == field
                      where ee.PeopleId == PeopleId
                      select ee).FirstOrDefault();
            if (ev != null)
            {
                db.PeopleExtras.DeleteOnSubmit(ev);
            }
        }

        public void LogExtraValue(string op, string field)
        {
            DbUtil.LogActivity($"EV {op}:{field}", peopleid: PeopleId);
        }

        public void AddEditExtraCode(string field, string value, string location = null)
        {
            if (!field.HasValue())
            {
                return;
            }

            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.StrValue = value.Replace(',', '-');
            ev.TransactionTime = Util.Now;
        }

        public void AddEditExtraDate(string field, DateTime? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.DateValue = value;
            ev.TransactionTime = Util.Now;
        }

        public void AddEditExtraText(string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.Data = value;
            ev.TransactionTime = dt ?? Util.Now;
        }

        public void AddEditExtraAttributes(string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.Data = value;
            ev.IsAttributes = true;
            ev.TransactionTime = dt ?? Util.Now;
        }

        public void AddToExtraText(string field, string value)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(field);
            if (ev.Data.HasValue())
            {
                ev.Data = value + "\n" + ev.Data;
            }
            else
            {
                ev.Data = value;
            }

            ev.TransactionTime = Util.Now;
        }

        public void AddEditExtraInt(string field, int value)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
            ev.TransactionTime = Util.Now;
        }
        public void AddEditExtraInt(string field, int? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.IntValue = value;
            ev.TransactionTime = Util.Now;
        }

        public void AddEditExtraBool(string field, bool tf, string name = null, string location = null)
        {
            if (!field.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.BitValue = tf;
            ev.TransactionTime = Util.Now;
        }
        public void AddEditExtraBoolIfTrue(string field, bool? tf)
        {
            if (!field.HasValue())
            {
                return;
            }

            if (!tf.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.BitValue = tf;
            ev.TransactionTime = Util.Now;
        }

        public void AddEditExtraInts(string field, int value, int value2)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
            ev.IntValue2 = value2;
            ev.TransactionTime = Util.Now;
        }

        public void AddEditExtraValue(string field, string code, DateTime? date, string text, bool? bit, int? intn,
                                      DateTime? dt = null)
        {
            var ev = GetExtraValue(field);
            ev.StrValue = code;
            ev.Data = text;
            ev.DateValue = date;
            ev.IntValue = intn;
            ev.BitValue = bit;
            ev.UseAllValues = true;
            ev.TransactionTime = dt ?? Util.Now;
        }

        public static PeopleExtra GetExtraValue(CMSDataContext db, int id, string field)
        {
            field = field.Trim();
            var q = from v in db.PeopleExtras
                    where v.Field == field
                    where v.PeopleId == id
                    select v;
            var ev = q.SingleOrDefault();
            if (ev == null)
            {
                ev = new PeopleExtra
                {
                    PeopleId = id,
                    Field = field.Trim(),
                    TransactionTime = Util.Now
                };
                db.PeopleExtras.InsertOnSubmit(ev);
            }
            return ev;
        }

        public static bool ExtraValueExists(CMSDataContext db, int id, string field)
        {
            var q = from v in db.PeopleExtras
                    where v.Field == field
                    where v.PeopleId == id
                    select v;
            var ev = q.SingleOrDefault();
            return ev != null;
        }

        public static PeopleExtra GetExtraValue(CMSDataContext db, int id, string field, string value)
        {
            field = field.Trim();
            var novalue = !value.HasValue();
            var q = from v in db.PeopleExtras
                    where v.PeopleId == id
                    where v.Field == field
                    where novalue || v.StrValue == value
                    select v;
            var ev = q.SingleOrDefault();
            return ev;
        }

        public static void AddEditExtraValue(CMSDataContext db, int id, string field, string value)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(db, id, field);
            ev.StrValue = value;
            ev.TransactionTime = Util.Now;
        }
        public static void AddEditExtraValues(CMSDataContext db, int id, string field, string code, DateTime? date, string text, bool? bit, int? intn,
                                      DateTime? dt = null)
        {
            var ev = GetExtraValue(db, id, field);
            ev.StrValue = code;
            ev.Data = text;
            ev.DateValue = date;
            ev.IntValue = intn;
            ev.BitValue = bit;
            ev.UseAllValues = true;
            ev.TransactionTime = dt ?? Util.Now;
        }

        public static void AddEditExtraData(CMSDataContext db, int id, string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(db, id, field);
            ev.Data = value;
            ev.TransactionTime = dt ?? Util.Now;
        }

        public static void AddToExtraText(CMSDataContext db, int id, string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(db, id, field);
            if (ev.Data.HasValue())
            {
                ev.Data = value + "\n" + ev.Data;
            }
            else
            {
                ev.Data = value;
            }

            ev.TransactionTime = Util.Now;
        }

        public static void AddEditExtraAttributes(CMSDataContext db, int id, string field, string value,
                                                  DateTime? dt = null)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(db, id, field);
            ev.Data = value;
            ev.IsAttributes = true;
            ev.TransactionTime = dt ?? Util.Now;
        }

        public static void AddEditExtraDate(CMSDataContext db, int id, string field, DateTime? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(db, id, field);
            ev.DateValue = value;
            ev.TransactionTime = Util.Now;
        }

        public static void AddEditExtraInt(CMSDataContext db, int id, string field, int? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(db, id, field);
            ev.IntValue = value;
            ev.TransactionTime = Util.Now;
        }

        public static void AddEditExtraBool(CMSDataContext db, int id, string field, bool? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(db, id, field);
            ev.BitValue = value;
            ev.TransactionTime = Util.Now;
        }

        public ManagedGiving ManagedGiving()
        {
            var mg = ManagedGivings.SingleOrDefault();
            return mg;
        }

        public PaymentInfo PaymentInfo()
        {
            var pi = PaymentInfos.SingleOrDefault();
            return pi;
        }

        public Contribution PostUnattendedContribution(CMSDataContext db, decimal amt, int? fund, string description,
                                                       bool pledge = false, int? typecode = null, int? tranid = null, string meta = null)
        {
            if (!typecode.HasValue)
            {
                typecode = BundleTypeCode.Online;
                if (pledge)
                {
                    typecode = BundleTypeCode.OnlinePledge;
                }
            }

            var now = Util.Now;
            var d = now.Date;
            BundleHeader bundle = null;

            var spec = db.Setting("OnlineContributionBundleDayTime", "");
            if (spec.HasValue())
            {
                var a = spec.SplitStr(" ", 2);
                try
                {
                    var next = DateTime.Parse(now.ToShortDateString() + " " + a[1]);
                    var dow = Enum.Parse(typeof(DayOfWeek), a[0], ignoreCase: true);
                    next = next.Sunday().Add(next.TimeOfDay).AddDays(dow.ToInt());
                    if (now > next)
                    {
                        next = next.AddDays(7);
                    }

                    var prev = next.AddDays(-7);
                    var bid = BundleTypeCode.MissionTrip == typecode
                        ? db.GetCurrentMissionTripBundle(next, prev)
                        : BundleTypeCode.OnlinePledge == typecode
                            ? db.GetCurrentOnlinePledgeBundle(next, prev)
                            : db.GetCurrentOnlineBundle(next, prev);
                    bundle = db.BundleHeaders.SingleOrDefault(bb => bb.BundleHeaderId == bid);
                }
                catch (Exception)
                {
                    spec = "";
                }
            }
            if (!spec.HasValue())
            {
                var nextd = d.AddDays(1);
                var bid = BundleTypeCode.MissionTrip == typecode
                    ? db.GetCurrentMissionTripBundle(nextd, d)
                    : BundleTypeCode.OnlinePledge == typecode
                        ? db.GetCurrentOnlinePledgeBundle(nextd, d)
                        : db.GetCurrentOnlineBundle(nextd, d);
                bundle = db.BundleHeaders.SingleOrDefault(bb => bb.BundleHeaderId == bid);
            }
            if (bundle == null)
            {
                bundle = new BundleHeader
                {
                    BundleHeaderTypeId = typecode.Value,
                    BundleStatusId = BundleStatusCode.Open,
                    CreatedBy = Util.UserId1,
                    ContributionDate = d,
                    CreatedDate = now,
                    FundId = db.Setting("DefaultFundId", "1").ToInt(),
                    RecordStatus = false,
                    TotalCash = 0,
                    TotalChecks = 0,
                    TotalEnvelopes = 0,
                    BundleTotal = 0
                };
                db.BundleHeaders.InsertOnSubmit(bundle);
                db.SubmitChanges();
            }
            if (!fund.HasValue)
            {
                fund = db.Setting("DefaultFundId", "1").ToInt();
            }

            var fundtouse = (from f in db.ContributionFunds
                             where f.FundId == fund
                             select f).SingleOrDefault();

            //failsafe if fund is not found
            if (fundtouse == null)
            {
                fund = (from f in db.ContributionFunds
                        where f.FundStatusId == 1
                        orderby f.FundId
                        select f.FundId).First();
            }

            var financeManagerId = db.Setting("FinanceManagerId", "").ToInt2();
            if (!financeManagerId.HasValue)
            {
                var qu = from u in db.Users
                         where u.UserRoles.Any(ur => ur.Role.RoleName == "Finance")
                         orderby u.Person.LastName
                         select u.UserId;
                financeManagerId = qu.FirstOrDefault();
                if (!financeManagerId.HasValue)
                {
                    financeManagerId = 1;
                }
            }
            var bd = new BundleDetail
            {
                BundleHeaderId = bundle.BundleHeaderId,
                CreatedBy = financeManagerId.Value,
                CreatedDate = now,
            };
            var typid = ContributionTypeCode.CheckCash;
            if (pledge)
            {
                typid = ContributionTypeCode.Pledge;
            }

            bd.Contribution = new Contribution
            {
                CreatedBy = financeManagerId.Value,
                CreatedDate = bd.CreatedDate,
                FundId = fund.Value,
                PeopleId = PeopleId,
                ContributionDate = bd.CreatedDate,
                ContributionAmount = amt,
                ContributionStatusId = 0,
                ContributionTypeId = typid,
                ContributionDesc = description,
                TranId = tranid,
                Source = Util2.FromMobile.HasValue() ? 1 : (int?)null,
                MetaInfo = meta,
                //CampusId is set with an update Trigger when peopleid is changed or when a new contribution is created that has a peopleId
            };
            bundle.BundleDetails.Add(bd);
            db.SubmitChanges();
            if (fundtouse == null)
            {
                db.LogActivity($"FundNotFound Used fund #{fund} on contribution #{bd.ContributionId}");
            }

            return bd.Contribution;
        }

        public static int FetchOrCreateMemberStatus(CMSDataContext db, string type)
        {
            if (!type.HasValue())
            {
                return 0;
            }

            var ms = db.MemberStatuses.SingleOrDefault(m => m.Description == type);
            if (ms == null)
            {
                var max = db.MemberStatuses.Max(mm => mm.Id) + 1;
                ms = new MemberStatus() { Id = max, Code = "M" + max, Description = type };
                db.MemberStatuses.InsertOnSubmit(ms);
                db.SubmitChanges();
            }
            return ms.Id;
        }

        public static int FetchOrCreateJoinType(CMSDataContext db, string status)
        {
            var ms = db.JoinTypes.SingleOrDefault(m => m.Description == status);
            if (ms == null)
            {
                var id = db.JoinTypes.Max(mm => mm.Id) + 10;
                ms = new JoinType() { Id = id, Code = "J" + id, Description = status };
                db.JoinTypes.InsertOnSubmit(ms);
                db.SubmitChanges();
            }
            return ms.Id;
        }

        public static int FetchOrCreateMaritalStatusId(CMSDataContext db, string status)
        {
            var ms = db.MaritalStatuses.SingleOrDefault(m => m.Description == status);
            if (ms == null)
            {
                var max = db.MaritalStatuses.Max(mm => mm.Id) + 1;
                ms = new MaritalStatus() { Id = max, Code = "M" + max, Description = status };
                db.MaritalStatuses.InsertOnSubmit(ms);
                db.SubmitChanges();
            }
            return ms.Id;
        }
        public static int FetchOrCreatePositionInFamilyId(CMSDataContext db, string position)
        {
            var fp = db.FamilyPositions.SingleOrDefault(m => m.Description == position);
            if (fp == null)
            {
                var max = db.FamilyPositions.Max(mm => mm.Id) + 1;
                fp = new FamilyPosition() { Id = max, Code = "M" + max, Description = position };
                db.FamilyPositions.InsertOnSubmit(fp);
                db.SubmitChanges();
            }
            return fp.Id;
        }

        public static int FetchOrCreateBaptismType(CMSDataContext db, string type)
        {
            var bt = db.BaptismTypes.SingleOrDefault(m => m.Description == type);
            if (bt == null)
            {
                var max = db.BaptismTypes.Max(mm => mm.Id) + 10;
                bt = new BaptismType() { Id = max, Code = "b" + max, Description = type };
                db.BaptismTypes.InsertOnSubmit(bt);
                db.SubmitChanges();
            }
            return bt.Id;
        }

        public static int FetchOrCreateDropType(CMSDataContext db, string type)
        {
            var dr = db.DropTypes.SingleOrDefault(m => m.Description == type);
            if (dr == null)
            {
                var max = db.DropTypes.Max(mm => mm.Id) + 10;
                dr = new DropType() { Id = max, Code = "dr" + max, Description = type };
                db.DropTypes.InsertOnSubmit(dr);
                db.SubmitChanges();
            }
            return dr.Id;
        }

        public static int FetchOrCreateMaritalStatus(CMSDataContext db, string type)
        {
            var ms = db.MaritalStatuses.SingleOrDefault(m => m.Description == type);
            if (ms == null)
            {
                var max = db.BaptismTypes.Max(mm => mm.Id) + 10;
                ms = new MaritalStatus() { Id = max, Code = "ms" + max, Description = type };
                db.MaritalStatuses.InsertOnSubmit(ms);
                db.SubmitChanges();
            }
            return ms.Id;
        }

        public static int FetchOrCreateDecisionType(CMSDataContext db, string type)
        {
            var dt = db.DecisionTypes.SingleOrDefault(m => m.Description == type);
            if (dt == null)
            {
                var max = db.DecisionTypes.Max(mm => mm.Id) + 10;
                dt = new DecisionType() { Id = max, Code = "d" + max, Description = type };
                db.DecisionTypes.InsertOnSubmit(dt);
                db.SubmitChanges();
            }
            return dt.Id;
        }

        public static int FetchOrCreateNewMemberClassStatus(CMSDataContext db, string type)
        {
            var i = db.NewMemberClassStatuses.SingleOrDefault(m => m.Description == type);
            if (i == null)
            {
                var max = db.NewMemberClassStatuses.Max(mm => mm.Id) + 10;
                i = new NewMemberClassStatus() { Id = max, Code = "NM" + max, Description = type };
                db.NewMemberClassStatuses.InsertOnSubmit(i);
                db.SubmitChanges();
            }
            return i.Id;
        }

        public static int FetchOrCreateVolunteerApplicationStatus(CMSDataContext db, string status)
        {
            var i = db.VolApplicationStatuses.SingleOrDefault(m => m.Description == status);
            if (i == null)
            {
                var max = db.VolApplicationStatuses.Max(mm => mm.Id) + 10;
                i = new VolApplicationStatus() { Id = max, Code = "VS" + max, Description = status };
                db.VolApplicationStatuses.InsertOnSubmit(i);
                db.SubmitChanges();
            }
            return i.Id;
        }

        public static Campu FetchOrCreateCampus(CMSDataContext db, string campus)
        {
            if (!campus.HasValue())
            {
                return null;
            }

            var cam = db.Campus.SingleOrDefault(pp => pp.Description == campus);
            if (cam == null)
            {
                int max = 10;
                if (db.Campus.Any())
                {
                    max = db.Campus.Max(mm => mm.Id) + 10;
                }

                cam = new Campu() { Id = max, Description = campus, Code = campus.Truncate(20) };
                db.Campus.InsertOnSubmit(cam);
                db.SubmitChanges();
            }
            else if (!cam.Code.HasValue())
            {
                cam.Code = campus.Truncate(20);
                db.SubmitChanges();
            }
            return cam;
        }

        public Task AddTaskAbout(CMSDataContext db, int assignTo, string description)
        {
            var t = new Task
            {
                OwnerId = assignTo,
                Description = description,
                ForceCompleteWContact = true,
                ListId = Task.GetRequiredTaskList(db, "InBox", assignTo).Id,
                StatusId = TaskStatusCode.Active,
            };
            TasksAboutPerson.Add(t);

            var gcm = new GCMHelper(Util.Host, DbUtil.Db);
            gcm.sendRefresh(assignTo, GCMHelper.ACTION_REFRESH);

            return t;
        }

        public void UpdatePosition(CMSDataContext db, int value)
        {
            this.UpdateValue("PositionInFamilyId", value);
            LogChanges(db, Util.UserPeopleId.Value);
            db.SubmitChanges();
        }

        public void UpdateCampus(CMSDataContext db, object value)
        {
            var campusid = value.ToInt2();
            if (campusid == 0)
            {
                campusid = null;
            }

            this.UpdateValue("CampusId", campusid);
            LogChanges(db, Util.UserPeopleId.Value);
            db.SubmitChanges();
        }

        public void UploadPicture(CMSDataContext db, Stream stream)
        {
            if (Picture == null)
            {
                Picture = new Picture();
            }

            var bits = new byte[stream.Length];
            stream.Read(bits, 0, bits.Length);
            var p = Picture;
            p.CreatedDate = Util.Now;
            p.CreatedBy = Util.UserName;
            p.ThumbId = Image.NewImageFromBits(bits, 50, 50).Id;
            p.SmallId = Image.NewImageFromBits(bits, 120, 120).Id;
            p.MediumId = Image.NewImageFromBits(bits, 320, 400).Id;
            p.LargeId = Image.NewImageFromBits(bits).Id;
            LogPictureUpload(db, Util.UserPeopleId ?? 1);
            db.SubmitChanges();
        }

        public void DeletePicture(CMSDataContext db)
        {
            if (Picture == null)
            {
                return;
            }

            Image.Delete(Picture.ThumbId);
            Image.Delete(Picture.SmallId);
            Image.Delete(Picture.MediumId);
            Image.Delete(Picture.LargeId);
            var pid = PictureId;
            Picture = null;
            db.SubmitChanges();
            db.ExecuteCommand("DELETE dbo.Picture WHERE PictureId = {0}", pid);
        }

        public void DeleteThumbnail(CMSDataContext db)
        {
            if (Picture == null)
            {
                return;
            }

            Image.Delete(Picture.ThumbId);
            Picture.ThumbId = null;
            db.SubmitChanges();
        }

        public void UploadDocument(CMSDataContext db, Stream stream, string name, string mimetype)
        {
            var mdf = new MemberDocForm
            {
                PeopleId = PeopleId,
                DocDate = Util.Now,
                UploaderId = Util2.CurrentPeopleId,
                Name = Path.GetFileName(name).Truncate(100)
            };
            db.MemberDocForms.InsertOnSubmit(mdf);
            var bits = new byte[stream.Length];
            stream.Read(bits, 0, bits.Length);
            switch (mimetype)
            {
                case "image/jpeg":
                case "image/pjpeg":
                case "image/gif":
                case "image/png":
                    mdf.IsDocument = false;
                    mdf.SmallId = Image.NewImageFromBits(bits, 165, 220).Id;
                    mdf.MediumId = Image.NewImageFromBits(bits, 675, 900).Id;
                    mdf.LargeId = Image.NewImageFromBits(bits).Id;
                    break;
                case "text/plain":
                case "application/pdf":
                case "application/msword":
                case "application/vnd.ms-excel":
                    mdf.MediumId = Image.NewImageFromBits(bits, mimetype).Id;
                    mdf.SmallId = mdf.MediumId;
                    mdf.LargeId = mdf.MediumId;
                    mdf.IsDocument = true;
                    break;
                default:
                    throw new FormatException("file type not supported: " + mimetype);
            }
            db.SubmitChanges();
        }

        public void SplitFamily(CMSDataContext db)
        {
            var f = new Family
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                AddressLineOne = PrimaryAddress,
                AddressLineTwo = PrimaryAddress2,
                CityName = PrimaryCity,
                StateCode = PrimaryState,
                ZipCode = PrimaryZip,
                HomePhone = Family.HomePhone
            };
            var oldf = this.FamilyId;
            f.People.Add(this);
            db.Families.InsertOnSubmit(f);
            db.SubmitChanges();
        }

        public void PromoteToHeadOfHousehold(CMSDataContext db)
        {
            var churchHasEnabledFeature = DbUtil.Db.GetSetting("CanOverrideHeadOfHousehold", "false");

            if (!string.Equals(churchHasEnabledFeature, "true", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Family.HeadOfHousehold = this;
            Family.HeadOfHouseholdSpouse = db.LoadPersonById(SpouseId ?? 0);

            db.SubmitChanges();
        }

        public RelatedFamily AddRelated(CMSDataContext db, int pid)
        {
            var p2 = db.LoadPersonById(pid);
            var rf = db.RelatedFamilies.SingleOrDefault(r =>
                (r.FamilyId == FamilyId && r.RelatedFamilyId == p2.FamilyId)
                || (r.FamilyId == p2.FamilyId && r.RelatedFamilyId == FamilyId)
            );
            if (rf == null)
            {
                rf = new RelatedFamily
                {
                    FamilyId = FamilyId,
                    RelatedFamilyId = p2.FamilyId,
                    FamilyRelationshipDesc = "",
                    CreatedBy = Util.UserId1,
                    CreatedDate = Util.Now,
                };
                db.RelatedFamilies.InsertOnSubmit(rf);
                db.SubmitChanges();
            }
            return rf;
        }

        public bool CanViewStatementFor(CMSDataContext db, int id)
        {
            // todo: improve performance
            bool canview = Util.UserPeopleId == id || (HttpContextFactory.Current.User.IsInRole("Finance") && !HttpContextFactory.Current.User.IsInRole("FundManager"));

            if (!canview)
            {
                var p = db.CurrentUserPerson;
                if (p.SpouseId == id)
                {
                    var sp = db.LoadPersonById(id);
                    if ((p.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint &&
                        (sp.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
                    {
                        canview = true;
                    }
                }
            }
            return canview;
        }

        public List<EmailOptOut> GetOptOuts()
        {
            return EmailOptOuts.ToList();
        }

        public List<User> GetUsers()
        {
            return Users.ToList();
        }

        public void UpdateContributionOption(CMSDataContext db, int option)
        {
            UpdateOption(db, "ContributionOptionsId", option);
        }

        public void UpdateEnvelopeOption(CMSDataContext db, int option)
        {
            UpdateOption(db, "EnvelopeOptionsId", option);
        }

        private void UpdateOption(CMSDataContext db, string field, int option)
        {
            int? opt = option;
            if (opt == 0)
            {
                opt = null;
            }

            UpdateValue(field, opt);
            LogChanges(db);
            var sp = db.LoadPersonById(SpouseId ?? 0);
            if (sp != null)
            {
                switch (opt)
                {
                    case StatementOptionCode.Joint:
                    case StatementOptionCode.Individual:
                    case StatementOptionCode.None:
                        sp.UpdateValue(field, opt);
                        sp.LogChanges(db);
                        break;
                    case null:
                        if (sp.ContributionOptionsId == StatementOptionCode.Joint)
                        {
                            sp.UpdateValue(field, null);
                            sp.LogChanges(db);
                        }
                        break;
                }
            }

            db.SubmitChanges();
        }

        public void UpdateElectronicStatement(CMSDataContext db, bool tf)
        {
            const string field = "ElectronicStatement";
            UpdateValue(field, tf);
            var sp = db.LoadPersonById(SpouseId ?? 0);
            if (sp != null && ContributionOptionsId == StatementOptionCode.Joint)
            {
                sp.UpdateValue(field, tf);
                sp.LogChanges(db);
            }
            LogChanges(db);
            db.SubmitChanges();
        }

        public static Person FindAddPerson(CMSDataContext db, string context, string first, string last, string dob,
                                           string email, string phone, string streetaddress = null, string zip = null,
                                           string extraValue = null, string extraValueField = null)
        {
            Person person = null;
            List<View.FindPerson> list = null;

            if (!String.IsNullOrEmpty(extraValue))
            {
                // 1.Do we have a pushpay key that matches a peopleextra
                list = db.FindPersonByExtraValue(extraValueField, extraValue).ToList();
            }
            else if (!String.IsNullOrEmpty(email))
            {
                // 2.Do we have an email address that matches one person
                list = db.FindPersonByEmail(email).ToList();
            }
            else if (!String.IsNullOrEmpty(first) && !String.IsNullOrEmpty(last))
            {
                //3.Do we have a person with the first / last name specified
                list = db.FindPerson(first, last, null, null, null).ToList();
            }
                
     
            var count = list?.Count;
            if (count > 0)
            {
                person = db.LoadPersonById(list[0].PeopleId ?? 0);
            }

            dynamic result = new DynamicData();
            if (count > 1)
            {
                result.MultipleMatches = true;
            }

            if (person != null)
            {
                return person;
            }

            result.NewPerson = true;
            var f = new Family
            {
                HomePhone = phone.GetDigits().Truncate(20),
                AddressLineOne = streetaddress,
                ZipCode = zip
            };
            db.SubmitChanges();

            //4.Create a new person record            
            var position = 10;
            person = Add(db, true, f, position, null, first.Trim(), null, last.Trim(), "", 0, 0,
                OriginCode.Contribution, null);
            person.EmailAddress = email?.Trim();
            person.SendEmailAddress1 = true;

            //5.Associate the pushpay key to the person's record (if it's not already there)
            if (!String.IsNullOrEmpty(extraValue))
                db.AddExtraValueData(person.PeopleId, extraValueField, extraValue, null, null, null, null);

            if (count == 0)
            {
                person.Comments = $"Added in context of {context} because record was not found";
            }

            return person;
        }

        private static bool IsHeadOfHouseholdFromMyDataPage
        {
            get
            {
                var fam = HttpContextFactory.Current?.Items["FamilyFromMyDataPage"] as Family;
                return fam?.IsHeadOfHouseold(Util.UserPeopleId) == true;
            }
        }
    }
}
