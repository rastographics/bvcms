using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.Codes;
using CmsData.ExtraValue;
using UtilityExtensions;

namespace CmsData
{
    public partial class Contact : ITableWithExtraValues
    {
        public static int AddContact(Guid qid, int? MakerId = null)
        {
            var q = DbUtil.Db.PeopleQuery(qid);
            if (!q.Any())
                return 0;
            var c = new Contact 
			{ 
				ContactDate = DateTime.Now.Date, 
				CreatedBy = Util.UserId1,
	            CreatedDate = DateTime.Now,
			};
            foreach (var p in q)
                c.contactees.Add(new Contactee { PeopleId = p.PeopleId });
            if (MakerId.HasValue)
                c.contactsMakers.Add(new Contactor {PeopleId = MakerId.Value});
            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();
            return c.ContactId;
        }
        public static Contact AddFamilyContact(CMSDataContext db, int familyId, int? makerId = null)
        {
            var c = new Contact 
			{ 
				ContactDate = DateTime.Now.Date, 
				CreatedBy = Util.UserId1,
	            CreatedDate = DateTime.Now,
			};
            var primaryorchild = new[] {PositionInFamily.PrimaryAdult, PositionInFamily.Child};
            var fmembers = (from p in db.People
                            where p.FamilyId == familyId
                            where primaryorchild.Contains(p.PositionInFamilyId)
                            select p.PeopleId).ToList();

            foreach (var pid in fmembers)
                c.contactees.Add(new Contactee { PeopleId = pid });
            if (makerId.HasValue)
                c.contactsMakers.Add(new Contactor {PeopleId = makerId.Value});
            db.Contacts.InsertOnSubmit(c);
            db.SubmitChanges();
            return c;
        }
        public static Contact AddContact(CMSDataContext Db, int contacteeid, DateTime? date, string comments, int? contactmakerid = null)
        {
            var c = new Contact 
			{ 
				ContactDate = date ?? DateTime.Parse("1/1/1900"), 
				CreatedBy = Util.UserPeopleId ?? Util.UserId1,
	            CreatedDate = DateTime.Now,
				Comments = comments
			};
            c.contactees.Add(new Contactee { PeopleId = contacteeid });
            if(contactmakerid.HasValue)
                c.contactsMakers.Add(new Contactor { PeopleId = contactmakerid.Value });
            Db.Contacts.InsertOnSubmit(c);
            Db.SubmitChanges();
			return c;
        }
        public static Contact AddOrgContact(CMSDataContext db, int orgId, DateTime? date, string comments, int contactmakerid)
        {
            var c = new Contact 
			{ 
                OrganizationId = orgId,
				ContactDate = date ?? DateTime.Parse("1/1/1900"), 
	            CreatedDate = DateTime.Now,
				Comments = comments
			};
            c.contactsMakers.Add(new Contactor { PeopleId = contactmakerid });
            db.Contacts.InsertOnSubmit(c);
            db.SubmitChanges();
			return c;
        }
		public static ContactType FetchOrCreateContactType(CMSDataContext Db, string type)
		{
			var ct = Db.ContactTypes.SingleOrDefault(pp => pp.Description == type);
			if (ct == null)
			{
				var max = Db.ContactTypes.Max(mm => mm.Id) + 10;
				if (max < 1000)
					max = 1010;
				ct = new ContactType { Id = max, Description = type, Code = type.Truncate(20) };
				Db.ContactTypes.InsertOnSubmit(ct);
				Db.SubmitChanges();
			}
			return ct;
		}
		public static Ministry FetchOrCreateMinistry(CMSDataContext Db, string name)
		{
			var m = Db.Ministries.SingleOrDefault(pp => pp.MinistryName == name);
			if (m == null)
			{
				m = new Ministry { MinistryName = name };
				Db.Ministries.InsertOnSubmit(m);
				Db.SubmitChanges();
			}
			return m;
		}


        public static ContactReason FetchOrCreateContactReason(CMSDataContext db, string reason)
        {
            var r = db.ContactReasons.SingleOrDefault(pp => pp.Description == reason);
            if (r == null)
            {
                var max = db.ContactReasons.Max(mm => mm.Id) + 10;
                if (max < 1000)
                    max = 1010;
                r = new ContactReason { Id = max, Description = reason, Code = reason.Truncate(20) };
                db.ContactReasons.InsertOnSubmit(r);
                db.SubmitChanges();
            }
            return r;
        }

        public void AddEditExtraCode(string field, string value, string location = null)
        {
            if (!field.HasValue())
                return;
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.StrValue = value;
            ev.TransactionTime = DateTime.Now;
            ev.Metadata = RetrieveMetadata(field, value, location);
        }

        public void AddEditExtraText(string field, string value, DateTime? dt = null)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.Data = value;
            ev.TransactionTime = dt ?? DateTime.Now;
        }

        public void AddEditExtraDate(string field, DateTime? value)
        {
            if (!value.HasValue)
                return;
            var ev = GetExtraValue(field);
            ev.DateValue = value;
            ev.TransactionTime = DateTime.Now;
        }

        public void AddToExtraText(string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.DataType = "text";
            if (ev.Data.HasValue())
                ev.Data = value + "\n" + ev.Data;
            else
                ev.Data = value;
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
                return;
            var ev = GetExtraValue(field);
            ev.BitValue = tf;
            ev.TransactionTime = DateTime.Now;
            ev.Metadata = RetrieveMetadata(name, field, location);
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
            var ev = ContactExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase: true) == 0);
            if (ev != null)
            {
                db.ContactExtras.DeleteOnSubmit(ev);
                ev.TransactionTime = DateTime.Now;
            }
        }

        public void LogExtraValue(string op, string field)
        {
            DbUtil.LogActivity($"EVContact {op}:{field}", ContactId);
        }

        public ContactExtra GetExtraValue(string field)
        {
            field = field.Trim();
            var ev = ContactExtras.AsEnumerable().FirstOrDefault(ee => ee.Field == field);
            if (ev == null)
            {
                ev = new ContactExtra()
                {
                    ContactId = ContactId,
                    Field = field,

                };
                ContactExtras.Add(ev);
            }
            return ev;
        }

        private static string RetrieveMetadata(string name, string value, string location)
        {
            var extraValues = Views.GetStandardExtraValues(DbUtil.Db, "Contact", false, location);
            var ev = extraValues.SingleOrDefault(x => x.Name == name);

            return ev?.Codes.SingleOrDefault(x => x.Text == value)?.Metadata;
        }
    }
}
