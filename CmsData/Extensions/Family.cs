using CmsData.Classes.GoogleCloudMessaging;
using CmsData.Codes;
using ImageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData
{
    public partial class Family : ITableWithExtraValues
    {
        public string CityStateZip => Util.FormatCSZ4(CityName, StateCode, ZipCode);

        public string AddrCityStateZip => AddressLineOne + " " + CityStateZip;

        public string Addr2CityStateZip => AddressLineTwo + " " + CityStateZip;

        public string FullAddress
        {
            get
            {
                var sb = new StringBuilder(AddressLineOne + "\n");
                if (AddressLineTwo.HasValue())
                {
                    sb.AppendLine(AddressLineTwo);
                }

                sb.Append(CityStateZip);
                return sb.ToString();
            }
        }


        public string HohName(CMSDataContext Db)
        {
            if (HeadOfHouseholdId.HasValue)
            {
                return Db.People.Where(p => p.PeopleId == HeadOfHouseholdId.Value).Select(p => p.Name).SingleOrDefault();
            }

            return "";
        }

        public string HohSpouseName(CMSDataContext Db)
        {
            if (HeadOfHouseholdSpouseId.HasValue)
            {
                return Db.People.Where(p => p.PeopleId == HeadOfHouseholdSpouseId.Value).Select(p => p.Name).SingleOrDefault();
            }

            return "";
        }
        public string FamilyName(CMSDataContext Db)
        {
            return "The " + HohName(Db) + " Family";
        }

        public int MemberCount => People.Count;

        private List<ChangeDetail> fsbDefault;
        public void UpdateValue(string field, object value)
        {
            if (fsbDefault == null)
            {
                fsbDefault = new List<ChangeDetail>();
            }

            this.UpdateValue(fsbDefault, field, value);
        }
        public void UpdateValueFromText(string field, string value)
        {
            if (fsbDefault == null)
            {
                fsbDefault = new List<ChangeDetail>();
            }

            this.UpdateValueFromText(fsbDefault, field, value);
        }
        public void UpdateValueFromText(List<ChangeDetail> fsb, string field, string value)
        {
            value = value.TrimEnd();
            var o = Util.GetProperty(this, field);
            if (o is string)
            {
                o = ((string)o).TrimEnd();
            }

            if (o == null && value == null)
            {
                return;
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
            //fsb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            fsb.Add(new ChangeDetail(field, o, value));
            Util.SetPropertyFromText(this, field, value);
        }
        public void LogChanges(CMSDataContext Db, int PeopleId, int UserPeopleId)
        {
            if (fsbDefault != null)
            {
                LogChanges(Db, fsbDefault, PeopleId, UserPeopleId);
            }
        }

        public void LogChanges(CMSDataContext Db, List<ChangeDetail> changes, int PeopleId)
        {
            LogChanges(Db, changes, PeopleId, Util.UserPeopleId ?? 0);
        }

        public void LogChanges(CMSDataContext Db, List<ChangeDetail> changes, int PeopleId, int UserPeopleId)
        {
            if (changes.Count > 0)
            {
                var c = new ChangeLog
                {
                    FamilyId = FamilyId,
                    UserPeopleId = UserPeopleId,
                    PeopleId = PeopleId,
                    Field = "Family",
                    Created = Util.Now
                };
                Db.ChangeLogs.InsertOnSubmit(c);
                c.ChangeDetails.AddRange(changes);
            }
        }
        public void LogPictureUpload(CMSDataContext Db, int PeopleId, int UserPeopleId)
        {
            var c = new ChangeLog
            {
                UserPeopleId = UserPeopleId,
                PeopleId = PeopleId,
                FamilyId = FamilyId,
                Field = "Family",
                Created = Util.Now
            };
            Db.ChangeLogs.InsertOnSubmit(c);
            c.ChangeDetails.Add(new ChangeDetail("Picture", null, "(new upload)"));
        }
        public void UploadPicture(CMSDataContext db, System.IO.Stream stream, int PeopleId)
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
            LogPictureUpload(db, PeopleId, Util.UserPeopleId ?? 1);
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
        public void SetExtra(string field, string value)
        {
            var e = FamilyExtras.FirstOrDefault(ee => ee.Field == field);
            if (e == null)
            {
                e = new FamilyExtra { Field = field, FamilyId = FamilyId, TransactionTime = DateTime.Now };
                this.FamilyExtras.Add(e);
            }
            e.StrValue = value;
        }
        public string GetExtra(string field)
        {
            var e = FamilyExtras.SingleOrDefault(ee => ee.Field == field);
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
        public FamilyExtra GetExtraValue(string field)
        {
            if (!field.HasValue())
            {
                field = "blank";
            }

            field = field.Trim();
            var ev = FamilyExtras.AsEnumerable().FirstOrDefault(ee => ee.Field == field);
            if (ev == null)
            {
                ev = new FamilyExtra
                {
                    FamilyId = FamilyId,
                    Field = field,
                    TransactionTime = DateTime.Now
                };
                FamilyExtras.Add(ev);
            }
            return ev;
        }

        public void AddEditExtraValue(string field, string code, DateTime? date, string text, bool? bit, int? intn, DateTime? dt = null)
        {
            var ev = GetExtraValue(field);
            ev.StrValue = code;
            ev.Data = text;
            ev.DateValue = date;
            ev.IntValue = intn;
            ev.BitValue = bit;
            ev.UseAllValues = true;
            ev.TransactionTime = dt ?? DateTime.Now;
        }

        public void RemoveExtraValue(CMSDataContext db, string field)
        {
            var ev = FamilyExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase: true) == 0);
            if (ev != null)
            {
                db.FamilyExtras.DeleteOnSubmit(ev);
            }
        }

        public void LogExtraValue(string op, string field)
        {
            DbUtil.LogActivity($"EVFamily {op}:{field}");
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
            ev.StrValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public void AddEditExtraDate(string field, DateTime? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.DateValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public void AddEditExtraText(string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.Data = value;
            ev.TransactionTime = dt ?? DateTime.Now;
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

            ev.TransactionTime = DateTime.Now;
        }
        public void AddEditExtraInt(string field, int value)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public void AddEditExtraBool(string field, bool tf, string name = null, string location = null)
        {
            if (!field.HasValue())
            {
                return;
            }

            var ev = GetExtraValue(field);
            ev.BitValue = tf;
            ev.TransactionTime = DateTime.Now;
        }
        public static FamilyExtra GetExtraValue(CMSDataContext db, int pid, string field)
        {
            var fid = (from v in db.People
                       where v.PeopleId == pid
                       select v.FamilyId).SingleOrDefault();
            if (fid == 0)
            {
                return null;
            }

            field = field.Trim();
            var q = from v in db.FamilyExtras
                    where v.Field == field
                    where v.FamilyId == fid
                    select v;
            var ev = q.SingleOrDefault();
            if (ev == null)
            {
                ev = new FamilyExtra
                {
                    FamilyId = fid,
                    Field = field,
                    TransactionTime = DateTime.Now
                };
                db.FamilyExtras.InsertOnSubmit(ev);
            }
            return ev;
        }
        public static FamilyExtra GetExtraValueFamilyId(CMSDataContext db, int fid, string field)
        {
            if (fid == 0)
            {
                return null;
            }

            field = field.Trim();
            var q = from v in db.FamilyExtras
                    where v.Field == field
                    where v.FamilyId == fid
                    select v;
            var ev = q.SingleOrDefault();
            if (ev == null)
            {
                ev = new FamilyExtra
                {
                    FamilyId = fid,
                    Field = field,
                    TransactionTime = DateTime.Now
                };
                db.FamilyExtras.InsertOnSubmit(ev);
            }
            return ev;
        }
        public static bool ExtraValueExists(CMSDataContext db, int pid, string field)
        {
            var fid = (from v in db.People
                       where v.PeopleId == pid
                       select v.FamilyId).SingleOrDefault();
            if (fid == 0)
            {
                return false;
            }
            //field = field.Replace('/', '-');
            var q = from v in db.FamilyExtras
                    where v.Field == field
                    where v.FamilyId == fid
                    select v;
            var ev = q.SingleOrDefault();
            return ev != null;
        }
        public static FamilyExtra GetExtraValue(CMSDataContext db, int pid, string field, string value)
        {
            field = field.Trim();
            var fid = (from v in db.People
                       where v.PeopleId == pid
                       select v.FamilyId).SingleOrDefault();
            if (fid == 0)
            {
                return null;
            }

            var novalue = !Util.HasValue(value);
            var q = from v in db.FamilyExtras
                    where v.FamilyId == fid
                    where v.Field == field
                    where novalue || v.StrValue == value
                    select v;
            var ev = q.SingleOrDefault();
            return ev;
        }
        public static void AddEditExtraValue(CMSDataContext db, int pid, string field, string value)
        {
            if (!Util.HasValue(value))
            {
                return;
            }

            var ev = GetExtraValue(db, pid, field);
            ev.StrValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraData(CMSDataContext db, int pid, string field, string value)
        {
            if (!Util.HasValue(value))
            {
                return;
            }

            var ev = GetExtraValue(db, pid, field);
            ev.Data = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraDataWithFamilyId(CMSDataContext db, int fid, string field, string value)
        {
            if (!Util.HasValue(value))
            {
                return;
            }

            var ev = GetExtraValueFamilyId(db, fid, field);
            ev.Data = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraDate(CMSDataContext db, int pid, string field, DateTime? value)
        {
            var fid = (from v in db.People
                       where v.PeopleId == pid
                       select v.FamilyId).SingleOrDefault();
            if (fid == 0)
            {
                return;
            }

            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(db, pid, field);
            ev.DateValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraInt(CMSDataContext db, int pid, string field, int? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(db, pid, field);
            ev.IntValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static void AddEditExtraBool(CMSDataContext db, int pid, string field, bool? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var ev = GetExtraValue(db, pid, field);
            ev.BitValue = value;
            ev.TransactionTime = DateTime.Now;
        }
        public static Task AddTaskAbout(CMSDataContext db, int familyId, int assignTo, string description)
        {
            var f = db.Families.Single(ff => ff.People.Any(mm => mm.FamilyId == familyId));
            var primaryorchild = new[] { PositionInFamily.PrimaryAdult, PositionInFamily.Child };
            var fmembers = (from p in db.People
                            where p.FamilyId == familyId
                            where primaryorchild.Contains(p.PositionInFamilyId)
                            select p.Name).ToList();
            var hh = db.LoadPersonById(f.HeadOfHouseholdId ?? 0);
            var t = new Task
            {
                OwnerId = assignTo,
                Description = description,
                Notes = "Family: " + string.Join(", ", fmembers),
                ForceCompleteWContact = true,
                ListId = Task.GetRequiredTaskList(db, "InBox", assignTo).Id,
                StatusId = TaskStatusCode.Active,
            };
            hh.TasksAboutPerson.Add(t);
            if (Util.Host.HasValue())
            {
                var gcm = new GCMHelper(Util.Host, DbUtil.Db);
                gcm.sendRefresh(assignTo, GCMHelper.ACTION_REFRESH);
            }
            return t;

        }
        public bool IsHeadOfHouseold(int? pid) => pid != null && (pid == HeadOfHouseholdId || pid == HeadOfHouseholdSpouseId);
    }
}
