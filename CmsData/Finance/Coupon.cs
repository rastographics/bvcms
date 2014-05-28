using System;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Net.Mail;
using UtilityExtensions;

namespace CmsData
{
    public partial class Coupon
    {
		public void UseCoupon(int pid, Decimal amt)
		{
			PeopleId = pid;
			Used = DateTime.Now;
			RegAmount = amt;
		}
    }
}