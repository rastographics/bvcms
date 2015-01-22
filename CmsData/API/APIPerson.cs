using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using UtilityExtensions;
using System.Data.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Net;

namespace CmsData.API
{
	public class APIPerson
	{
		private CMSDataContext Db;

		public APIPerson(CMSDataContext Db)
		{
			this.Db = Db;
		}
		[Serializable]
		public class People
		{
			[XmlElement("Person")]
			public Person[] List { get; set; }
		}
		[Serializable]
		public class Address
		{
			[XmlElementAttribute(IsNullable = true)]
			public string AddressLineOne { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string AddressLineTwo { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string CityName { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string StateCode { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string ZipCode { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string CountryName { get; set; }
			public DateTime? AddressFromDate { get; set; }
			public DateTime? AddressToDate { get; set; }
			public bool? BadAddressFlag { get; set; }
		}
		[Serializable]
		public class AddrItem
		{
			public static AddrItem New(XElement e)
			{
				return new AddrItem
				{
					Text = e != null ? e.Value : ""
				};
			}
			[XmlAttribute]
			public string Error { get; set; }
			[XmlText]
			public string Text { get; set; }
		}
		[Serializable]
		public class Address2
		{
			public static Address2 New(XElement e, int id, string country)
			{
				return new Address2
				{
					Id = id,
					type = e.Name.ToString(),
					AddressLineOne = AddrItem.New(e.Element("AddressLineOne")),
					AddressLineTwo = AddrItem.New(e.Element("AddressLineTwo")),
					CityName = AddrItem.New(e.Element("CityName")),
					StateCode = AddrItem.New(e.Element("StateCode")),
					ZipCode = AddrItem.New(e.Element("ZipCode")),
					CountryName = country
				};
			}
			[XmlAttribute()]
			public int Id { get; set; }
			[XmlAttribute()]
			public string type { get; set; }
			[XmlAttribute()]
			public string Error { get; set; }
			public AddrItem AddressLineOne { get; set; }
			public AddrItem AddressLineTwo { get; set; }
			public AddrItem CityName { get; set; }
			public AddrItem StateCode { get; set; }
			public AddrItem ZipCode { get; set; }
			public string CountryName { get; set; }
		}
		[Serializable]
		public class Person
		{
			[XmlAttribute]
			public int PeopleId { get; set; }
			public int FamilyId { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string NickName { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string TitleCode { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string FirstName { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string MiddleName { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string LastName { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string Name { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string SuffixCode { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string AltName { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string MaidenName { get; set; }
			public int GenderId { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string HomePhone { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string CellPhone { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string WorkPhone { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string EmailAddress { get; set; }
			public bool? SendEmailAddress1 { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string EmailAddress2 { get; set; }
			public bool? SendEmailAddress2 { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string SchoolOther { get; set; }
			public int? Grade { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string EmployerOther { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string OccupationOther { get; set; }
			public int? MaritalStatusId { get; set; }
			public DateTime? WeddingDate { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string DOB { get; set; }
			public bool? DoNotCallFlag { get; set; }
			public bool? DoNotMailFlag { get; set; }
			public bool? DoNotVisitFlag { get; set; }
			public int PositionInFamilyId { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string SpouseName { get; set; }
			public int? CampusId { get; set; }
			public DateTime? DeceasedDate { get; set; }
			public int? MemberStatusId { get; set; }
			public DateTime? JoinDate { get; set; }
			public int? DecisionTypeId { get; set; }
			public DateTime? DecisionDate { get; set; }
			public int? BaptismTypeId { get; set; }
			public DateTime? BaptismDate { get; set; }
			public DateTime? BaptismSchedDate { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string OtherPreviousChurch { get; set; }
			public int? JoinCodeId { get; set; }
			public int? DropCodeId { get; set; }
			public DateTime? DropDate { get; set; }
			[XmlElementAttribute(IsNullable = true)]
			public string OtherNewChurch { get; set; }
			public string EmContact { get; set; }
			public string EmPhone { get; set; }
			public int? NewMemberClassStatusId { get; set; }
			public DateTime? NewMemberClassDate { get; set; }
			public Address FamilyAddress { get; set; }
			public Address PersonalAddress { get; set; }
			public int AddressTypeId { get; set; }
			public List<string> Usernames { get; set; }
		}
		public Person GetPersonData(int id)
		{
			return GetPeopleData(id).SingleOrDefault();
		}
		public IEnumerable<Person> GetPeopleData(int? id, int? famid = null, string first = null, string last = null)
		{
			var q = from p in Db.People
					where id == null || id == p.PeopleId
					where famid == null || famid == p.FamilyId
					where first == null || (first == p.FirstName || first == p.NickName)
					where last == null || (last == p.LastName || last == p.MaidenName)
					let sp = p.Family.People.SingleOrDefault(pp => pp.PeopleId == p.SpouseId)
                    let rr = p.RecRegs.FirstOrDefault()
					select new Person
					{
						PeopleId = p.PeopleId,
						FamilyId = p.FamilyId,
						NickName = p.NickName,
						TitleCode = p.TitleCode,
						FirstName = p.FirstName,
						MiddleName = p.MiddleName,
						LastName = p.LastName,
                        Name = p.Name,
						SuffixCode = p.SuffixCode,
						AltName = p.AltName,
						MaidenName = p.MaidenName,
						GenderId = p.GenderId,
						HomePhone = p.HomePhone,
						CellPhone = p.CellPhone,
						WorkPhone = p.WorkPhone,
						EmailAddress = p.EmailAddress,
						SendEmailAddress1 = p.SendEmailAddress1,
						EmailAddress2 = p.EmailAddress2,
						SendEmailAddress2 = p.SendEmailAddress2,
						SchoolOther = p.SchoolOther,
						Grade = p.Grade,
						EmployerOther = p.EmployerOther,
						OccupationOther = p.OccupationOther,
						MaritalStatusId = p.MaritalStatusId,
						WeddingDate = p.WeddingDate,
						DOB = p.DOB,
						DoNotCallFlag = p.DoNotCallFlag,
						DoNotMailFlag = p.DoNotMailFlag,
						DoNotVisitFlag = p.DoNotVisitFlag,
						PositionInFamilyId = p.PositionInFamilyId,
						SpouseName = sp.Name,
						CampusId = p.CampusId,
						DeceasedDate = p.DeceasedDate,
						MemberStatusId = p.MemberStatusId,
						JoinDate = p.JoinDate,
						BaptismDate = p.BaptismDate,
						BaptismSchedDate = p.BaptismSchedDate,
						BaptismTypeId = p.BaptismTypeId,
						DecisionDate = p.DecisionDate,
						DecisionTypeId = p.DecisionTypeId,
						DropCodeId = p.DropCodeId,
						DropDate = p.DropDate,
						JoinCodeId = p.JoinCodeId,
						NewMemberClassDate = p.NewMemberClassDate,
						NewMemberClassStatusId = p.NewMemberClassStatusId,
						OtherNewChurch = p.OtherNewChurch,
						OtherPreviousChurch = p.OtherPreviousChurch,
                        EmContact = rr.Emcontact,
                        EmPhone = rr.Emphone,
						AddressTypeId = p.AddressTypeId,
						FamilyAddress = new Address
						{
							AddressLineOne = p.Family.AddressLineOne,
							AddressLineTwo = p.Family.AddressLineTwo,
							CityName = p.Family.CityName,
							StateCode = p.Family.StateCode,
							ZipCode = p.Family.ZipCode,
							CountryName = p.Family.CountryName,
							BadAddressFlag = p.Family.BadAddressFlag,
							AddressFromDate = p.Family.AddressFromDate,
							AddressToDate = p.Family.AddressToDate,
						},
						PersonalAddress = new Address
						{
							AddressLineOne = p.AddressLineOne,
							AddressLineTwo = p.AddressLineTwo,
							CityName = p.CityName,
							StateCode = p.StateCode,
							ZipCode = p.ZipCode,
							CountryName = p.CountryName,
							BadAddressFlag = p.BadAddressFlag,
							AddressFromDate = p.AddressFromDate,
							AddressToDate = p.AddressToDate
						},
						Usernames = (from u in p.Users
									 select u.Username).ToList(),

					};
			return q.Take(100);
		}
		public string GetPeopleXml(int? id, int? famid, string first, string last)
		{
			var xs = new XmlSerializer(typeof(People));
			var sw = new StringWriter();
			var a = new People { List = GetPeopleData(id, famid, first, last).ToArray() };
			xs.Serialize(sw, a);
			return sw.ToString();
		}
		public string GetPersonXml(int id)
		{
			var xs = new XmlSerializer(typeof(Person));
			var sw = new StringWriter();
			var p = GetPersonData(id);
			if (p == null)
				return "<Person error=\"Not found\" />";
			xs.Serialize(sw, p);
			return sw.ToString();
		}
		public string UpdatePersonXml(string xml)
		{
			var x = XDocument.Parse(xml);
			var p = Db.LoadPersonById(x.Root.Attribute("PeopleId").Value.ToInt());
			var u = new APIPerson.Update(p);

			Address2 addr = null;
			foreach (var e in x.Root.Elements())
				switch (e.Name.ToString())
				{
					case "PersonalAddress":
						foreach (var pa in e.Elements())
							u.UpdatePerson(pa);
						addr = Address2.New(e, p.PeopleId, p.CountryName);
						addr = ValidateAddress(addr);
						if (addr.Error.HasValue())
							return serialize(addr);
						break;
					case "FamilyAddress":
						foreach (var fa in e.Elements())
							u.UpdateFamily(fa);
						addr = Address2.New(e, p.PeopleId, p.Family.CountryName);
						addr = ValidateAddress(addr);
						if (addr.Error.HasValue())
							return serialize(addr);
						break;
                    case "EmContact":
				        p.SetRecReg().Emcontact = e.Value;
				        break;
                    case "EmPhone":
				        p.SetRecReg().Emphone = e.Value;
				        break;
					default:
						break;
				}
			foreach (var e in x.Root.Elements())
				switch (e.Name.ToString())
				{
					case "PersonalAddress":
					case "FamilyAddress":
					case "SpouseName":
					case "FamilyId":
					case "EmContact":
					case "EmPhone":
						break;
					case "HomePhone":
						u.UpdateFamily(e);
						break;
					default:
						u.UpdatePerson(e);
						break;
				}
			p.LogChanges(Db, u.sb);
			p.Family.LogChanges(Db, u.fsb, p.PeopleId, Util.UserPeopleId.Value);
			Db.SubmitChanges();
			return "<Success />";
		}
		private Address2 ValidateAddress(Address2 a)
		{
			var sw = new StringWriter();
			bool addrok = a.CityName.Text.HasValue() && a.StateCode.Text.HasValue();
			if (!addrok && a.ZipCode.Text.HasValue())
				addrok = true;
			if (!addrok && !a.CityName.Text.HasValue() && !a.StateCode.Text.HasValue() && !a.ZipCode.Text.HasValue())
				addrok = true;
			if (!addrok)
			{
				a.Error = "city/state required or zip required (or \"na\" in all)";
				return a;
			}
			if (a.AddressLineOne.Text.HasValue() && (a.CityName.Text.HasValue() || a.StateCode.Text.HasValue() || a.ZipCode.Text.HasValue())
				&& (a.CountryName == "United States" || !a.CountryName.HasValue()))
			{
				var r = AddressVerify.LookupAddress(a.AddressLineOne.Text, a.AddressLineTwo.Text, a.CityName.Text, a.StateCode.Text, a.ZipCode.Text);
				if (r.Line1 == "error")
				{
					a.Error = r.address;
					return a;
				}
				if (r.found == false)
				{
					a.Error = r.address + ", if your address will not validate, change the country to 'USA, Not Validated'";
					return a;
				}
				if (r.Line1 != a.AddressLineOne.Text)
				{
					a.AddressLineOne.Error = "address changed from '{0}'".Fmt(a.AddressLineOne.Text);
					a.Error = "changes";
					a.AddressLineOne.Text = r.Line1;
				}
				if (r.Line2 != (a.AddressLineTwo.Text ?? ""))
				{
					a.AddressLineTwo.Error = "address2 changed from '{0}'".Fmt(a.AddressLineTwo.Text);
					a.Error = "changes";
					a.AddressLineTwo.Text = r.Line2;
				}
				if (r.City != (a.CityName.Text ?? ""))
				{
					a.CityName.Error = "city changed from '{0}'".Fmt(a.CityName.Text);
					a.Error = "changes";
					a.CityName.Text = r.City;
				}
				if (r.State != (a.StateCode.Text ?? ""))
				{
					a.StateCode.Error = "state changed from '{0}'".Fmt(a.StateCode.Text);
					a.Error = "changes";
					a.StateCode.Text = r.State;
				}
				if (r.Zip != (a.ZipCode.Text ?? ""))
				{
					a.ZipCode.Error = "zip changed from '{0}'".Fmt(a.ZipCode.Text);
					a.Error = "changes";
					a.ZipCode.Text = r.Zip;
				}
			}
			return a;
		}
		private string serialize(Address2 addr)
		{
			var sw = new StringWriter();
			new XmlSerializer(typeof(Address2)).Serialize(sw, addr);
			return sw.ToString();
		}
		private class Update
		{
			private CmsData.Person person;
			private CmsData.Family family;
			public List<ChangeDetail> sb { get; set; }
			public List<ChangeDetail> fsb { get; set; }
			private XNamespace xsi = "xsi";
			public Update(CmsData.Person p)
			{
				person = p;
				family = p.Family;
				sb = new List<ChangeDetail>();
				fsb = new List<ChangeDetail>();
			}
			public void UpdatePerson(XElement e)
			{
				var nil = e.Attribute(xsi + "nil");
				var name = e.Name.ToString();
				if (nil != null && nil.Value == "true")
					person.UpdateValue(sb, name, null);
				else if (name == "CellPhone" || name == "WorkPhone")
					person.UpdateValueFromText(fsb, name, e.Value.GetDigits());
				else
					person.UpdateValueFromText(sb, name, e.Value);
			}
			public void UpdateFamily(XElement e)
			{
				var nil = e.Attribute(xsi + "nil");
				var name = e.Name.ToString();
				if (nil != null && nil.Value == "true")
					family.UpdateValue(fsb, name, null);
				else if (name == "HomePhone")
					family.UpdateValueFromText(fsb, name, e.Value.GetDigits());
				else
					family.UpdateValueFromText(fsb, name, e.Value);
			}
		}
	}
}
