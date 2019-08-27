using CmsData;
using CmsData.Classes.RoleChecker;
using CmsData.Codes;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using UtilityExtensions;
using ImageData;

namespace CmsWeb.Models
{
    public class OrgContentInfo
    {
        public CMSDataContext CurrentDatabase { get; set; }
        public int OrgId { get; set; }
        public string error { get; set; }
        public string OrgName { get; set; }
        public bool Inactive { get; set; }
        public bool IsMember { get; set; }
        public bool IsLeader { get; set; }
        public bool NotAuthenticated { get; set; }
        public OrgContent oc { get; set; }

        public bool CanEdit => ((Util.IsInRole("ContentEdit") || Util.IsInRole("Edit") || RoleChecker.HasSetting(SettingName.LeadersCanAlwaysEditOrgContent, false)) && IsLeader) || Util.IsInRole("Admin");

        private string html;
        public string Html
        {
            get
            {
                if (html.HasValue())
                {
                    return html;
                }

                if (oc == null)
                {
                    return "<h2>" + OrgName + "</h2>";
                }

                using (var idb = CMSImageDataContext.Create(HttpContextFactory.Current))
                {
                    html = idb.Content(oc.ImageId ?? 0);
                }
                return html;
            }
            set
            {
                if (oc == null)
                {
                    oc = new OrgContent { OrgId = OrgId, Landing = true };
                    CurrentDatabase.OrgContents.InsertOnSubmit(oc);
                }
                using (var imageDb = CMSImageDataContext.Create(CurrentDatabase.Host))
                {
                    var i = imageDb.Images.SingleOrDefault(ii => ii.Id == oc.ImageId);
                    if (i != null)
                    {
                        i.SetText(value);
                    }
                    else
                    {
                        oc.ImageId = ImageData.Image.NewTextFromString(value, imageDb).Id;
                    }

                    imageDb.SubmitChanges();
                }

                CurrentDatabase.SubmitChanges();
            }
        }

        public bool HideBanner => Html?.Contains("<span class=\"hide-banner\"") ?? false;

        public ImageData.Image image
        {
            get
            {
                if (oc == null || !IsMember)
                {
                    var i = new ImageData.Image();
                    var bmp = new Bitmap(200, 200, PixelFormat.Format24bppRgb);
                    var g = Graphics.FromImage(bmp);
                    g.Clear(Color.Bisque);
                    g.DrawString("No Image", new Font("Verdana", 22, FontStyle.Bold), SystemBrushes.WindowText, new PointF(2, 2));
                    i.Mimetype = "image/gif";
                    var ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Gif);
                    i.Bits = ms.ToArray();
                    return i;
                }
                var imageDb = CMSImageDataContext.Create(CurrentDatabase.Host);
                return imageDb.Images.SingleOrDefault(ii => ii.Id == oc.ImageId);
            }
        }

        public string Results { get; set; }

        public static OrgContentInfo Get(CMSDataContext db, int id)
        {
            var q = from oo in db.Organizations
                    where oo.OrganizationId == id
                    let om = oo.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == Util.UserPeopleId)
                    let oc = db.OrgContents.SingleOrDefault(cc => cc.OrgId == id && cc.Landing == true)
                    let memberLeaderType = om.MemberType.AttendanceTypeId
                    select new OrgContentInfo
                    {
                        OrgId = oo.OrganizationId,
                        OrgName = oo.OrganizationName,
                        Inactive = oo.OrganizationStatusId == OrgStatusCode.Inactive,
                        IsMember = om != null && !MemberTypeCode.ProspectInactive.Contains(om.MemberTypeId),
                        IsLeader = (memberLeaderType ?? 0) == AttendTypeCode.Leader,
                        oc = oc,
                        NotAuthenticated = !Util.UserPeopleId.HasValue
                    };
            var o = q.SingleOrDefault();
            if (o != null)
            {
                o.CurrentDatabase = db;
            }
            if (o != null && !o.IsMember)
            {
                var oids = db.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(o.OrgId))
                {
                    return o;
                }

                o.NotAuthenticated = false;
                o.IsMember = true;
                o.IsLeader = true;
            }
            return o;
        }

        public static OrgContentInfo GetOc(CMSDataContext db, int id)
        {
            var q = from oo in db.Organizations
                    let oc = db.OrgContents.SingleOrDefault(cc => cc.Id == id)
                    where oo.OrganizationId == oc.OrgId
                    let om = oo.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == Util.UserPeopleId)
                    let memberLeaderType = om.MemberType.AttendanceTypeId
                    select new OrgContentInfo
                    {
                        OrgId = oo.OrganizationId,
                        OrgName = oo.OrganizationName,
                        Inactive = oo.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Inactive,
                        IsMember = om != null && om.MemberTypeId != MemberTypeCode.InActive,
                        IsLeader = (memberLeaderType ?? 0) == CmsData.Codes.AttendTypeCode.Leader,
                        oc = oc,
                        NotAuthenticated = !Util.UserPeopleId.HasValue
                    };
            var o = q.SingleOrDefault();
            if (o != null)
            {
                o.CurrentDatabase = db;
            }
            if (o != null && !o.IsMember)
            {
                var oids = db.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(o.OrgId))
                {
                    return o;
                }

                o.NotAuthenticated = false;
                o.IsMember = true;
                o.IsLeader = true;
            }
            return o;
        }

        public IEnumerable<MemberInfo> GetMemberList()
        {
            return (from om in CurrentDatabase.OrganizationMembers
                    where om.OrganizationId == OrgId
                    where
                        om.MemberTypeId != MemberTypeCode.Prospect &&
                        om.MemberTypeId != MemberTypeCode.InActive
                    orderby om.Person.Name
                    select new MemberInfo
                    {
                        Name = om.Person.Name,
                        MemberType = om.MemberType.Description,
                        PeopleId = om.PeopleId
                    })
                    .ToList();
        }

        public class MemberInfo
        {
            public string Name { get; set; }
            public string MemberType { get; set; }
            public int PeopleId { get; set; }
        }

        public bool TryRunPython(CMSDataContext db, int pid)
        {
            var ev = Organization.GetExtra(db, OrgId, "OrgMembersPageScript");
            if (!ev.HasValue())
            {
                return false;
            }

            var script = db.ContentOfTypePythonScript(ev);
            if (!script.HasValue())
            {
                return false;
            }

            var pe = new PythonModel(db.Host);
            pe.Data.OrgId = OrgId;
            pe.Data.PeopleId = pid;
            Results = pe.RunScript(script);
            return true;
        }
    }
}
