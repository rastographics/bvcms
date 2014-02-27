using System;
using System.Linq;
using ImageData;
using UtilityExtensions;
using System.Text;

namespace CmsData
{
    public partial class Family : ITableWithExtraValues
    {
        public string CityStateZip
        {
            get { return Util.FormatCSZ4(CityName,StateCode,ZipCode); }
        }
        public string AddrCityStateZip
        {
            get { return AddressLineOne + " " + CityStateZip; }
        }
        public string Addr2CityStateZip
        {
            get { return AddressLineTwo + " " + CityStateZip; }
        }
        public string FullAddress
        {
            get
            {
                var sb = new StringBuilder(AddressLineOne + "\n");
                if (AddressLineTwo.HasValue())
                    sb.AppendLine(AddressLineTwo);
                sb.Append(CityStateZip);
                return sb.ToString();
            }
        }


        public string HohName(CMSDataContext Db)
        {
                if (HeadOfHouseholdId.HasValue) 
                    return Db.People.Where(p => p.PeopleId == HeadOfHouseholdId.Value).Select(p => p.Name).SingleOrDefault();
                return "";
        }

        public string HohSpouseName(CMSDataContext Db)
        {
                if (HeadOfHouseholdSpouseId.HasValue) 
                    return Db.People.Where(p => p.PeopleId == HeadOfHouseholdSpouseId.Value).Select(p => p.Name).SingleOrDefault();
                return "";
        }
        public string FamilyName(CMSDataContext Db)
        {
            return "The " + HohName(Db) + " Family";
        }

        public int MemberCount
        {
            get { return People.Count; }
        }

        private StringBuilder fsbDefault;
        public void UpdateValue(string field, object value)
        {
            if (fsbDefault == null)
                fsbDefault = new StringBuilder();
            this.UpdateValue(fsbDefault, field, value);
        }
        public void UpdateValueFromText(string field, string value)
        {
            if (fsbDefault == null)
                fsbDefault = new StringBuilder();
            this.UpdateValueFromText(fsbDefault, field, value);
        }
        public void UpdateValueFromText(StringBuilder fsb, string field, string value)
        {
            value = value.TrimEnd();
            var o = Util.GetProperty(this, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            if (o == null && value is string && !((string)value).HasValue())
                return;
            if (value == null && o is string && !((string)o).HasValue())
                return;
            fsb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            Util.SetPropertyFromText(this, field, value);
        }
        public void LogChanges(CMSDataContext Db, int PeopleId, int UserPeopleId)
        {
            if (fsbDefault != null)
                LogChanges(Db, fsbDefault.ToString(), PeopleId, UserPeopleId);
        }

        public void LogChanges(CMSDataContext Db, string changes, int PeopleId)
        {
            LogChanges(Db, changes, PeopleId, Util.UserPeopleId ?? 0);
        }

        public void LogChanges(CMSDataContext Db, string changes, int PeopleId, int UserPeopleId)
        {
            if (changes.HasValue())
            {
                var c = new ChangeLog
                {
                    FamilyId = FamilyId,
                    UserPeopleId = UserPeopleId,
                    PeopleId = PeopleId,
                    Field = "Family",
                    Data = "<table>\n{0}</table>".Fmt(changes),
                    Created = Util.Now
                };
                Db.ChangeLogs.InsertOnSubmit(c);
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
                Data = "<table>\n<tr><td>Picture</td><td></td><td>(new upload)</td></tr>\n</table>",
                Created = Util.Now
            };
            Db.ChangeLogs.InsertOnSubmit(c);
        }
        public void UploadPicture(CMSDataContext db, System.IO.Stream stream, int PeopleId)
        {
            if (Picture == null)
                Picture = new Picture();
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
                return;
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
                return "";
			if (e.StrValue.HasValue())
				return e.StrValue;
			if (e.Data.HasValue())
				return e.Data;
			if (e.DateValue.HasValue)
				return e.DateValue.FormatDate();
			if (e.IntValue.HasValue)
				return e.IntValue.ToString();
			return e.BitValue.ToString();
        }
        public FamilyExtra GetExtraValue(string field)
        {
			if (!field.HasValue())
				field = "blank";
			field = field.Replace(",", "_");
			var ev = FamilyExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase:true) == 0);
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
        public void RemoveExtraValue(CMSDataContext Db, string field)
        {
			var ev = FamilyExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase:true) == 0);
			if (ev != null)
				Db.FamilyExtras.DeleteOnSubmit(ev);
        }
        public void AddEditExtraValue(string field, string value)
        {
			if (!field.HasValue())
				return;
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.StrValue = value;
        }
        public void AddEditExtraDate(string field, DateTime? value)
        {
			if (!value.HasValue)
				return;
            var ev = GetExtraValue(field);
            ev.DateValue = value;
        }
        public void AddEditExtraData(string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
			ev.Data = value;
        }
        public void AddToExtraData(string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
			if (ev.Data.HasValue())
				ev.Data = value + "\n" + ev.Data;
			else
				ev.Data = value;
        }
        public void AddEditExtraInt(string field, int value)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
        }
        public void AddEditExtraBool(string field, bool tf)
        {
			if (!field.HasValue())
				return;
            var ev = GetExtraValue(field);
            ev.BitValue = tf;
        }
    }
}
