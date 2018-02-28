using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmsData;
using CsvHelper;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class UploadAddressesModel
    {
        private readonly CMSDataContext db;
        private readonly int userpeopleid;
        public Dictionary<string, int> NcoaCols;
        private List<ChangeDetail> fsb;

        public UploadAddressesModel(CMSDataContext db, int peopleId)
        {
            this.db = db;
            userpeopleid = peopleId;
            var spec = db.Setting("NcoaColumns", "PeopleId=1,Addr1=4,Addr2=5,City=6,State=7,Zip=8,MoveDate=10");
            NcoaCols = spec.Split(',').Select(vv => vv.Split('=')).ToDictionary(vv => vv[0], vv => vv[1].ToInt()-1);
        }

        public void DoUpload(string text, bool testing = false)
        {
            var rt = db.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            rt.Count = text.SplitLines(noblanks: true).Length - 1;
            db.SubmitChanges();
            var csv = new CsvReader(new StringReader(text));
            csv.Configuration.Delimiter = "\t";

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var pid = csv[NcoaCols["PeopleId"]].ToInt();
                var p = db.LoadPersonById(pid);
                fsb = new List<ChangeDetail>();

                var f = p.Family;
                f.UpdateValue(fsb, "AddressLineOne", csv[NcoaCols["Addr1"]]);
                f.UpdateValue(fsb ,"AddressLineTwo", csv[NcoaCols["Addr2"]]);
                f.UpdateValue(fsb, "CityName", csv[NcoaCols["City"]]);
                f.UpdateValue(fsb, "StateCode", csv[NcoaCols["State"]]);
                f.UpdateValue(fsb, "ZipCode", csv[NcoaCols["Zip"]]);

                p.Family.LogChanges(db, fsb, p.PeopleId, userpeopleid);
                if(NcoaCols.ContainsKey("MoveDate"))
                    if(csv.Context.HeaderRecord.Contains("MoveEffectiveDate", StringComparer.OrdinalIgnoreCase))
                        p.AddEditExtraDate("MoveEffectiveDate", 
                            csv[NcoaCols["MoveDate"]].ToDate() ?? DateTime.Today);
                rt.Processed++;
                db.SubmitChanges();
            }
            rt.Completed = DateTime.Now;
            db.SubmitChanges();
        }
    }
}
