using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
	public partial class OnlineRegController
	{
		public ActionResult SpecialRegistration( string id )
		{
			if( !id.HasValue() )
				return Content( "Invalid organization ID" );

			SpecialRegModel m = null;

			var td = TempData["ps"];

			if( td != null )
			{
				m = new SpecialRegModel {orgID = id.ToInt(), peopleID = td.ToInt()};
				TempData["ps"] = m.peopleID;
			}

			if( m == null )
			{
				var guid = id.ToGuid();

				if( guid == null )
					return Content( "Invalid link" );

				var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault( oo => oo.Id == guid.Value );
				if( ot == null )
					return Content( "Invalid link" );

				if( ot.Expires.HasValue && ot.Expires < DateTime.Now )
					return Content( "Link has expired" );

				var a = ot.Querystring.Split( ',' );

				m = new SpecialRegModel {orgID = a[0].ToInt(), peopleID = a[1].ToInt()};
				TempData["ps"] = m.peopleID;

				id = a[0];

				ot.Used = true;

				DbUtil.Db.SubmitChanges();
			}

			SetHeaders( id.ToInt() );
			DbUtil.LogActivity( "Special Registration: {0} ({1})".Fmt( m.Org.OrganizationName, m.Person.Name ) );

			var shell = DbUtil.Content( m.Regsettings.SpecialScript );

			if( shell != null )
			{
				string body = shell.Body;

				if( body != null )
				{
					body = body.Replace( "[action]", "/OnlineReg/SpecialRegistrationResults/" + m.orgID );
					return Content( body );
				}
			}

			return Content( "Shell not found" );
		}

		public ActionResult SpecialRegistrationResults( int id )
		{
			var td = TempData["ps"];
			if( td == null ) return Content( "Invalid registration information." );

			var peopleId = td.ToInt();

			var org = ( from e in DbUtil.Db.Organizations
				where e.OrganizationId == id
				select e ).FirstOrDefault();

			var person = ( from e in DbUtil.Db.People
				where e.PeopleId == peopleId
				select e ).FirstOrDefault();

			if( person == null ) return Content( "Person not found" );
			if( org == null ) return Content( "Organization not found" );
			
			var summary = "";

			foreach( var item in Request.Form.AllKeys )
			{
				if( item.StartsWith( "EV_" ) )
				{
					var evName = item.Substring( 3 );
					person.AddEditExtraValue( evName, Request.Form[item] );
					summary += evName + ": " + Request.Form[item] + "<br>";
				}
			}

			DbUtil.Db.SubmitChanges();

			List<Person> staffList = DbUtil.Db.StaffPeopleForOrg(id);
			var staff = staffList[0];

			var regSettings = new Settings( org.RegSetting, DbUtil.Db, id );

			var subject = Util.PickFirst( regSettings.Subject, "No subject" );
			var body = Util.PickFirst(regSettings.Body, "confirmation email body not found");

			subject = subject.Replace("{org}", org.OrganizationName);

			body = body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
			body = body.Replace("{name}", person.Name, ignoreCase: true);
			body = body.Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true);
			body = body.Replace("{email}", person.EmailAddress, ignoreCase: true);
			body = body.Replace("{phone}", person.HomePhone.FmtFone(), ignoreCase: true);
			body = body.Replace("{contact}", staff.Name, ignoreCase: true);
			body = body.Replace("{contactemail}", staff.EmailAddress, ignoreCase: true);
			body = body.Replace("{contactphone}", org.PhoneNumber.FmtFone(), ignoreCase: true);
			body = body.Replace("{details}", summary, ignoreCase: true);

			DbUtil.Db.Email(staff.FromEmail, person, subject, body);
			DbUtil.Db.Email(person.FromEmail, staff, "Registration completed for {0}".Fmt(org.OrganizationName), "{0} completed {1}<br/><br/>{2}".Fmt(person.Name, org.OrganizationName, summary));

			return View();
		}
	}
}