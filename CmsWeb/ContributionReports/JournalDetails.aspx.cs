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

namespace CmsWeb.Contributions.Reports
{
    public partial class JournalDetails : System.Web.UI.Page
    {
        BundleModel ctl = new BundleModel();
        protected void Page_Load(object sender, EventArgs e)
        {
            FromDt.Text = Request.QueryString["dt1"];
            ToDt.Text = Request.QueryString["dt2"];
        }
        protected void ListView1_DataBound(object sender, EventArgs e)
        {
            var c = ListView1.FindControl("Count") as Label;
            var t = ListView1.FindControl("Total") as Label;
            if (ctl.JournalTotal.Count.HasValue)
            {
                c.Text = ctl.JournalTotal.Count.ToString2("n0");
                t.Text = ctl.JournalTotal.Total.ToString2("c");
            }
            FundName.Text = ctl.JournalTotal.FundName;
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
