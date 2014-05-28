using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Linq;
using UtilityExtensions;
using System.Data.Linq;

namespace CmsData.API
{
    public class APIMeeting
    {
        private CMSDataContext Db;

        public APIMeeting()
        {
            Db = new CMSDataContext("Data Source=.;Initial Catalog=CMS_bellevue;Integrated Security=True");
        }
        public APIMeeting(CMSDataContext Db)
        {
            this.Db = Db;
        }
        public string DeleteExtraValue(int meetingid, string field)
        {
            try
            {
                var q = from v in Db.MeetingExtras
                        where v.Field == field
                        where v.MeetingId == meetingid
                        select v;
                Db.MeetingExtras.DeleteAllOnSubmit(q);
                Db.SubmitChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string ExtraValues(int meetingid, string fields)
        {
            try
            {
                var a = (fields ?? "").Split(',');
                var nofields = !fields.HasValue();
                var q = from v in Db.MeetingExtras
                        where nofields || a.Contains(v.Field)
                        where v.MeetingId == meetingid
                        select v;
                var w = new APIWriter();
                w.Start("ExtraMeetingValues");
                w.Attr("Id", meetingid);
                foreach (var v in q)
                    w.Add(v.Field, v.Data);
                w.End();
                return w.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string AddEditExtraValue(int meetingid, string field, string value)
        {
            try
            {
                var q = from v in Db.MeetingExtras
                        where v.Field == field
                        where v.MeetingId == meetingid
                        select v;
                var ev = q.SingleOrDefault();
                if (ev == null)
                {
                    ev = new MeetingExtra
                    {
                        MeetingId = meetingid,
                        Field = field,
                    };
                    Db.MeetingExtras.InsertOnSubmit(ev);
                }
                ev.Data = value;
                Db.SubmitChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
