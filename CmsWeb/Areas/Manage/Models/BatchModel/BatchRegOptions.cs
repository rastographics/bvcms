using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.BatchModel
{
    public class BatchRegOptions
    {
        private CsvReader csv;
        private Settings rs;

        public static void Update(string text)
        {
            new BatchRegOptions().DoUpdate(text);
        }
        private void DoUpdate(string text)
        {
            text = text.Trim();
            csv = new CsvReader(new StringReader(text), true, '\t');
            if (csv.GetFieldIndex("OrganizationId") == -1)
            {
                throw new UserInputException("Missing required OrganizationId column");
            }

            while (csv.ReadNextRecord())
            {
                var oid = csv["OrganizationId"].ToInt();
                var o = DbUtil.Db.LoadOrganizationById(oid);
                rs = DbUtil.Db.CreateRegistrationSettings(oid);

                string name;
                if (FindColumnString("OrganizationName", out name))
                {
                    if (name.HasValue())
                    {
                        o.OrganizationName = name;
                    }
                }

                UpdateAsk("AskAllergies");
                UpdateAsk("AnswersNotRequired");
                UpdateAsk("AskChurch");
                UpdateAsk("AskCoaching");
                UpdateAsk("AskDoctor");
                UpdateAsk("AnswersNotRequired");
                UpdateAsk("AskChurch");
                UpdateAsk("AskCoaching");
                UpdateAsk("AskDoctor");
                UpdateAsk("AskEmContact");
                UpdateAsk("AskInsurance");
                UpdateAsk("AskParents");
                UpdateAsk("AskSMS");
                UpdateAsk("AskTylenolEtc");
                UpdateAsk("AskSuggestedFee");
                UpdateAskLabel("AskRequest");
                UpdateAskLabel("AskTickets");

                bool? b = null;

                if (FindColumnBool("NoReqBirthYear", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NoReqBirthYear = b.Value;
                    }
                }

                if (FindColumnBool("NotReqDOB", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NotReqDOB = b.Value;
                    }
                }

                if (FindColumnBool("NotReqAddr", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NotReqAddr = b.Value;
                    }
                }

                if (FindColumnBool("NotReqGender", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NotReqGender = b.Value;
                    }
                }

                if (FindColumnBool("NotReqMarital", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NotReqMarital = b.Value;
                    }
                }

                if (FindColumnBool("NotReqCampus", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NotReqCampus = b.Value;
                    }
                }

                if (FindColumnBool("NotReqPhone", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NotReqPhone = b.Value;
                    }
                }

                if (FindColumnBool("NotReqZip", out b))
                {
                    if (b.HasValue)
                    {
                        rs.NotReqZip = b.Value;
                    }
                }

                if (FindColumnBool("AllowOnlyOne", out b))
                {
                    if (b.HasValue)
                    {
                        rs.AllowOnlyOne = b.Value;
                    }
                }

                if (FindColumnBool("MemberOnly", out b))
                {
                    if (b.HasValue)
                    {
                        rs.MemberOnly = b.Value;
                    }
                }

                if (FindColumnBool("AddAsProspect", out b))
                {
                    if (b.HasValue)
                    {
                        rs.AddAsProspect = b.Value;
                    }
                }

                //                if (FindColumnBool("TargetExtraValues", out b))
                //                    if (b.HasValue)
                //                        rs.TargetExtraValues = b.Value;

                if (FindColumnBool("AllowReRegister", out b))
                {
                    if (b.HasValue)
                    {
                        rs.AllowReRegister = b.Value;
                    }
                }

                if (FindColumnBool("AllowSaveProgress", out b))
                {
                    if (b.HasValue)
                    {
                        rs.AllowSaveProgress = b.Value;
                    }
                }

                if (FindColumnBool("DisallowAnonymous", out b))
                {
                    if (b.HasValue)
                    {
                        rs.DisallowAnonymous = b.Value;
                    }
                }

                if (FindColumnBool("ApplyMaxToOtheFees", out b))
                {
                    if (b.HasValue)
                    {
                        rs.ApplyMaxToOtherFees = b.Value;
                    }
                }

                if (FindColumnBool("IncludeOtherFeesWithDeposit", out b))
                {
                    if (b.HasValue)
                    {
                        rs.IncludeOtherFeesWithDeposit = b.Value;
                    }
                }

                if (FindColumnBool("OtherFeesAddedToOrgFee", out b))
                {
                    if (b.HasValue)
                    {
                        rs.OtherFeesAddedToOrgFee = b.Value;
                    }
                }

                if (FindColumnBool("AskDonation", out b))
                {
                    if (b.HasValue)
                    {
                        rs.AskDonation = b.Value;
                    }
                }

                string s;

                if (FindColumnString("ConfirmationTrackingCode", out s))
                {
                    rs.ConfirmationTrackingCode = s;
                }

                if (FindColumnString("ValidateOrgs", out s))
                {
                    rs.ValidateOrgs = s;
                }

                if (FindColumnString("Shell", out s))
                {
                    rs.Shell = s;
                }

                if (FindColumnString("ShellBs", out s))
                {
                    rs.ShellBs = s;
                }

                if (FindColumnString("FinishRegistrationButton", out s))
                {
                    rs.FinishRegistrationButton = s;
                }

                if (FindColumnString("SpecialScript", out s))
                {
                    rs.SpecialScript = s;
                }

                if (FindColumnString("OnEnrollScript", out s))
                {
                    rs.OnEnrollScript = s;
                }

                if (FindColumnString("GroupToJoin", out s))
                {
                    rs.GroupToJoin = s;
                }

                if (FindColumnString("TimeOut", out s))
                {
                    rs.TimeOut = s.ToInt2();
                }

                if (FindColumnString("Fee", out s))
                {
                    rs.Fee = s.ToDecimal();
                }

                if (FindColumnString("MaximumFee", out s))
                {
                    rs.MaximumFee = s.ToDecimal();
                }

                if (FindColumnString("ExtraFee", out s))
                {
                    rs.ExtraFee = s.ToDecimal();
                }

                if (FindColumnString("Deposit", out s))
                {
                    rs.Deposit = s.ToDecimal();
                }

                if (FindColumnString("AccountingCode", out s))
                {
                    rs.AccountingCode = s;
                }

                if (FindColumnString("ExtraValueFeeName", out s))
                {
                    rs.ExtraValueFeeName = s;
                }

                if (FindColumnString("DonationLabel", out s))
                {
                    rs.DonationLabel = s;
                }

                if (FindColumnString("DonationFundId", out s))
                {
                    rs.DonationFundId = s.ToInt();
                }

                o.UpdateRegSetting(rs);
                DbUtil.Db.SubmitChanges();
            }
        }

        private bool FindColumnBool(string col, out bool? ret)
        {
            var i = csv.GetFieldIndex(col);
            ret = i >= 0 ? csv[i].ToBool2() : null;
            return i >= 0;
        }
        private bool FindColumnString(string col, out string ret)
        {
            var i = csv.GetFieldIndex(col);
            ret = i >= 0 ? csv[i] : null;
            return i >= 0;
        }
        private void UpdateAskLabel(string name)
        {
            string val;
            if (!FindColumnString(name, out val))
            {
                return;
            }

            if (!val.HasValue())
            {
                return;
            }

            var askrequest = rs.AskItem(name) as AskRequest;
            if (askrequest == null)
            {
                rs.AskItems.Add(new AskRequest { Label = val });
            }
            else
            {
                askrequest.Label = val;
            }
        }

        private void UpdateAsk(string name)
        {
            bool? b;
            if (FindColumnBool(name, out b))
            {
                var a = rs.AskItem(name);
                if (a == null && b == true)
                {
                    rs.AskItems.Add(new Ask(name));
                }
                else if (a != null && b == false)
                {
                    rs.AskItems.Remove(a);
                }
            }
        }
    }
}
