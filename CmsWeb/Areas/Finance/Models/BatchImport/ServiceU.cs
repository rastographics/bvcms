/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using CmsData;
using CmsData.Codes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace CmsWeb.Models
{
    public partial class BatchImportContributions
    {
        public static int? BatchProcessServiceU(CsvReader csv, DateTime date)
        {
            var cols = csv.GetFieldHeaders();
            var now = DateTime.Now;

            var bh = GetBundleHeader(date, now);

            while (csv.ReadNextRecord())
            {
                string ac = null, oth = null, first = null, last = null, addr = null, name = null, email = null;
                var dt = date;
                for (var c = 1; c < csv.FieldCount; c++)
                {
                    var col = cols[c].Trim();
                    switch (col)
                    {
                        case "Date Entered":
                            dt = csv[c].ToDate() ?? date;
                            break;
                        case "ProfileID":
                            ac = csv[c];
                            break;
                        case "First Name":
                            first = csv[c];
                            break;
                        case "Last Name":
                            last = csv[c];
                            break;
                        case "Full Name":
                            name = csv[c];
                            break;
                        case "Address":
                            addr = csv[c];
                            break;
                        case "Email Address":
                            email = csv[c];
                            break;
                        case "Designation for &quot;Other&quot;":
                            oth = csv[c];
                            break;
                    }
                }
                if (ac.ToInt() == 0)
                    ac = email;
                var eac = Util.Encrypt(ac);
                var q = from kc in DbUtil.Db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                var pid = q.SingleOrDefault();
                string bankac = null;
                ExtraDatum ed = null;
                if (pid == null)
                {
                    bankac = eac;
                    string person;
                    if (last.HasValue())
                        person = "{1}, {0}; {2}".Fmt(first, last, addr);
                    else
                        person = "{0}; {1}".Fmt(name, addr);
                    ed = new ExtraDatum { Data = person, Stamp = Util.Now };
                }
                CmsData.BundleDetail bd = null;
                var defaultfundid = DbUtil.Db.Setting("DefaultFundId", "1").ToInt();
                for (var c = 0; c < csv.FieldCount; c++)
                {
                    var col = cols[c].Trim();
                    if (col != "Amount" && !col.Contains("Comment") && csv[c].StartsWith("$") && csv[c].GetAmount() > 0)
                    {
                        var fundid = FindFund(col);
                        bd = CreateContribution(date, fundid ?? defaultfundid);
                        bd.Contribution.ContributionAmount = csv[c].GetAmount();
                        if (col == "Other")
                            col = oth;
                        if (!fundid.HasValue)
                            bd.Contribution.ContributionDesc = col;
                        if (ac.HasValue())
                            bd.Contribution.BankAccount = bankac;
                        bd.Contribution.PeopleId = pid;
                        bh.BundleDetails.Add(bd);
                        if (ed != null)
                            bd.Contribution.ExtraDatum = ed;
                    }
                }
            }
            FinishBundle(bh);
            return bh.BundleHeaderId;
        }
    }
}
