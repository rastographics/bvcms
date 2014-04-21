/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Reports
{
    public partial class TotalsByFundRange : System.Web.UI.Page
    {
        BundleModel ctl = new BundleModel();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var pledged = Page.QueryString<string>("pledged");
                if (pledged == "true")
                    Label1.Text = "Pledge Totals by Range for Fund";
                if (pledged == "both")
                    Label1.Text = "Pledge Totals by Range (pledges included)";
                var from = this.QueryString<DateTime?>("from");
                var today = Util.Now.Date;
                var first = new DateTime(today.Year, today.Month, 1);
                if (today.Day < 8)
                    first = first.AddMonths(-1);
                if (!from.HasValue)
                    from = first;
                FromDate.Text = from.Value.ToString("d");
                ToDate.Text = from.Value.AddMonths(1).AddDays(-1).ToString("d");
            }
        }

        protected void ListView1_DataBound(object sender, EventArgs e)
        {
			var donorlabel = ListView1.FindControl("DonorCount") as Label;
			var countlabel = ListView1.FindControl("Count") as Label;
			var totallabel = ListView1.FindControl("Total") as Label;
            if (donorlabel == null)
                return;
			donorlabel.Text = (ctl.RangeTotal.DonorCount ?? 0).ToString("n0");
            countlabel.Text = (ctl.RangeTotal.Count ?? 0).ToString("n0");
            totallabel.Text = "&nbsp;&nbsp;" + (ctl.RangeTotal.Total ?? 0).ToString("c0");
        }

        protected void ObjectDataSource1_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = ctl;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ListView1.Visible = true;
        }
    }
}
