using System;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using CmsData;
using net.openstack.Providers.Rackspace.Objects.Databases;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
	public class MobileAuthentication
	{
		private User user;
		private Error error = Error.UNKNOWN;

		private string username = "";
		private string password = "";

		public void authenticate( string instanceID )
		{
			if( string.IsNullOrEmpty( HttpContext.Current.Request.Headers["Authorization"] ) ) {
				error = Error.NO_HEADER;

				return;
			}

			string authHeader = HttpContext.Current.Request.Headers["Authorization"];
			string[] headerParts = authHeader.SplitStr( " " );

			if( headerParts.Length != 2 ) {
				error = Error.INVALID_HEADER;

				return;
			}

			switch( headerParts[0].ToLower() ) {
				case "basic": {
					error = validateBasic( headerParts[1] );

					break;
				}

				case "pin": {
					error = validatePIN( headerParts[1], instanceID );

					break;
				}

				default: {
					error = Error.INVALID_HEADER_TYPE;

					break;
				}
			}
		}

		private Error validateBasic( string value )
		{
			string credentials;
			bool userFound;

			try {
				credentials = Encoding.ASCII.GetString( Convert.FromBase64String( value ) );
			} catch( Exception ) {
				return Error.MALFORMED_BASE64;
			}

			string[] userAndPassword = credentials.SplitStr( ":" );

			if( userAndPassword.Length != 2 ) {
				return Error.INVALID_HEADER;
			}

			if( string.IsNullOrEmpty( userAndPassword[0] ) || string.IsNullOrEmpty( userAndPassword[1] ) ) {
				return Error.MISSING_CREDENTIALS;
			}

			NetworkCredential networkCredential = new NetworkCredential( userAndPassword[0], userAndPassword[1] );
			username = networkCredential.UserName;
			password = networkCredential.Password;

			bool impersonating = password == DbUtil.Db.Setting( "ImpersonatePassword", Guid.NewGuid().ToString() );

			IQueryable<User> userQuery = DbUtil.Db.Users.Where( uu => uu.Username == username || uu.Person.EmailAddress == username || uu.Person.EmailAddress2 == username );

			try {
				userFound = userQuery.Any();
			} catch( Exception ex ) {
				return Error.DATABASE_ERROR;
			}

			foreach( var foundUser in userQuery.ToList() ) {
				if( !Membership.Provider.ValidateUser( username, password ) ) continue;

				DbUtil.Db.Refresh( RefreshMode.OverwriteCurrentValues, foundUser );
				user = foundUser;

				break;
			}

			return checkUser( userFound, impersonating );
		}

		public void setPIN( int device, string instance, string key, string pin )
		{
			byte[] bytes = Encoding.UTF8.GetBytes( $"{instance}:{getUsername()}:{pin}" );

			SHA256Managed hashstring = new SHA256Managed();
			byte[] hash = hashstring.ComputeHash( bytes );

			string hashString = "";

			foreach( byte x in hash ) {
				hashString += String.Format( "{0:x2}", x );
			}

			MobileAppDevice appDevice = DbUtil.Db.MobileAppDevices.FirstOrDefault( d => d.InstanceID == instance );

			if( appDevice != null ) {
				appDevice.Authentication = hashString;
			} else {
				appDevice = new MobileAppDevice()
				{
					Created = DateTime.Now,
					LastSeen = DateTime.Now,
					DeviceTypeID = device,
					InstanceID = instance,
					NotificationID = key,
					UserID = user.UserId,
					PeopleID = user.PeopleId,
					Authentication = hashString
				};

				DbUtil.Db.MobileAppDevices.InsertOnSubmit( appDevice );
			}

			DbUtil.Db.SubmitChanges();
		}

		private Error validatePIN( string value, string instance )
		{
			string credentials;

			try {
				credentials = Encoding.ASCII.GetString( Convert.FromBase64String( value ) );
			} catch( Exception ) {
				return Error.MALFORMED_BASE64;
			}

			string[] userAndPassword = credentials.SplitStr( ":" );

			if( userAndPassword.Length != 2 ) {
				return Error.INVALID_HEADER;
			}

			if( string.IsNullOrEmpty( userAndPassword[0] ) || string.IsNullOrEmpty( userAndPassword[1] ) ) {
				return Error.MISSING_CREDENTIALS;
			}

			byte[] bytes = Encoding.UTF8.GetBytes( $"{instance}:{userAndPassword[0]}:{userAndPassword[1]}" );

			SHA256Managed hashstring = new SHA256Managed();
			byte[] hash = hashstring.ComputeHash( bytes );

			string hashString = "";

			foreach( byte x in hash ) {
				hashString += String.Format( "{0:x2}", x );
			}

			MobileAppDevice appDevice = DbUtil.Db.MobileAppDevices.FirstOrDefault( d => d.InstanceID == instance && d.Authentication == hashString );

			if( appDevice == null ) {
				return Error.INVALID_PASSWORD;
			}

			user = appDevice.User;

			return checkUser( true, false );
		}

		private Error checkUser( bool userFound, bool impersonating )
		{
			if( user == null && userFound ) {
				DbUtil.LogActivity( $"Mobile: Failed password by {username}" );

				return Error.INVALID_PASSWORD;
			}

			if( user == null ) {
				DbUtil.LogActivity( $"Mobile: Attempt to login by unknown user {username}" );

				return Error.USER_NOT_FOUND;
			}

			if( user.IsLockedOut ) {
				DbUtil.LogActivity( $"Mobile: Attempt to login by locked out user {username}" );

				return Error.USER_LOCKED_OUT;
			}

			if( !user.IsApproved ) {
				DbUtil.LogActivity( $"Mobile: Attempt to login by unapproved user {username}" );

				return Error.USER_NOT_APPROVED;
			}

			if( impersonating && user.Roles.Contains( "Finance" ) ) {
				DbUtil.LogActivity( $"Mobile: Attempt to impersonate finance by {username}" );

				return Error.CANNOT_IMPERSONATE_FINANCE;
			}

			if( user.Roles.Contains( "APIOnly" ) ) {
				return Error.CANNOT_USE_API_ONLY;
			}

			return Error.AUTHENTICATED;
		}

		public bool hasError()
		{
			return error != Error.AUTHENTICATED;
		}

		public int getError()
		{
			return (int) error;
		}

		public string getErrorMessage()
		{
			return ERROR_MESSAGES[Math.Abs( (int) error )];
		}

		public User getUser()
		{
			return user;
		}

		public string getUsername()
		{
			return username;
		}

		public enum Error : int
		{
			AUTHENTICATED = 0,
			UNKNOWN = -1,
			NO_HEADER = -2,
			INVALID_HEADER = -3,
			INVALID_HEADER_TYPE = -4,
			MALFORMED_BASE64 = -5,
			MISSING_CREDENTIALS = -6,
			DATABASE_ERROR = -7,
			USER_NOT_FOUND = -8,
			USER_LOCKED_OUT = -9,
			USER_NOT_APPROVED = -10,
			INVALID_PASSWORD = -11,
			CANNOT_IMPERSONATE_FINANCE = -12,
			CANNOT_USE_API_ONLY = -13
		};

		private static readonly string[] ERROR_MESSAGES =
		{
			"Authenticated", // 0
			"Unknown Error", // -1
			"No authentication header", // -2
			"Invalid authentication header", // -3
			"Invalid authentication header type", // -4
			"Malformed Base64 in header", // -5
			"Missing credentials", // -6
			"Database error", // -7
			"User not found", // -8
			"User locked out", // -9
			"User not approved", // -10
			"Invalid password", // -11
			"Cannot impersonate finance user", // -12
			"Cannot access with API only user" // -13
		};
	}
}