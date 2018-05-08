/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using CmsData.View;
using System.Collections;
using UtilityExtensions;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.IO;
using System.Web.Configuration;
using System.Web;

namespace CmsData
{
	public class AddressVerify
	{
	    public class AddressResult
	    {
	        public bool? found { get; set; }
	        public string error { get; set; }
	        public string address { get; set; }
	        public string Line1 { get; set; }
	        public string Line2 { get; set; }
	        public string City { get; set; }
	        public string State { get; set; }
	        public string Zip { get; set; }
	        public int ExpireDays { get; set; }
	        public bool Changed (string addr1, string addr2, string city, string state, string zip )
            { 
                return Line1 != addr1 || (Line2 ?? "") != (addr2 ?? "") || City != city || State != state || (Zip ?? "") != (zip ?? "");
            }
	    }

	    public static AddressResult LookupAddress(string line1, string line2, string city, string st, string zip)
		{
			string url = ConfigurationManager.AppSettings["amiurl"];
			string password = ConfigurationManager.AppSettings["amipassword"];

	        if (!url.HasValue() || !password.HasValue())
	            return new AddressResult { Line1 = line1, Line2 = line2, City = city, State = st, Zip = zip };

			var wc = new MyWebClient();
			var coll = new NameValueCollection();
			coll.Add("line1", line1);
			coll.Add("line2", line2);
			coll.Add("csz", Util.FormatCSZ(city, st, zip));
			coll.Add("passcode", password);
			try
			{
				var resp = wc.UploadValues(url, "POST", coll);
				var s = Encoding.UTF8.GetString(resp);
                s = s.Replace("½", "1/2");
				var serializer = new XmlSerializer(typeof(AddressResult));
				var reader = new StringReader(s);
				var ret = (AddressResult)serializer.Deserialize(reader);
			    if (ret.found == null)
			        ret.found = false;
			    if (ret.found == true && ret.error.HasValue())
			        ret.found = false;
			    if (ret.error == "unauthorized")
			        ret.Line1 = "error";
			    if (ret.ExpireDays < 0 || ret.address.Trim() == ",")
			    {
			        ret.Line1 = "AMS Database expired";
			        ret.found = false;
			    }
			    return ret;
			}
			catch (Exception)
			{
				return new AddressResult { Line1 = "error" };
			}
		}
	class MyWebClient : WebClient
	{


		protected override WebRequest GetWebRequest(Uri uri)
		{
			WebRequest w = base.GetWebRequest(uri);
			w.Timeout = 1000;
			return w;
		}
	}
}
}
