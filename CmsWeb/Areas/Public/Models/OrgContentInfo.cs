using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;
using Image = ImageData.Image;

namespace CmsWeb.Models
{
    public class OrgContentInfo
    {
        public int OrgId { get; set; }
        public string error { get; set; }
        public string OrgName { get; set; }
        public bool Inactive { get; set; }
        public bool IsMember { get; set; }
        public bool IsLeader { get; set; }
        public bool NotAuthenticated { get; set; }
        public OrgContent oc { get; set; }

        public bool CanEdit => ((Util.IsInRole("ContentEdit") || Util.IsInRole("Edit")) && IsLeader) ||  Util.IsInRole("Admin");

        public string Html
        {
            get
            {
                if (oc == null)
                    return "<h2>" + OrgName + "</h2>";
                return Image.Content(oc.ImageId ?? 0);
            }
            set
            {
                if (oc == null)
                {
                    oc = new OrgContent { OrgId = OrgId, Landing = true };
                    DbUtil.Db.OrgContents.InsertOnSubmit(oc);
                }
                var i = ImageData.DbUtil.Db.Images.SingleOrDefault(ii => ii.Id == oc.ImageId);
                if (i != null)
                    i.SetText(value);
                else
                    oc.ImageId = Image.NewTextFromString(value).Id;
                DbUtil.Db.SubmitChanges();
            }
        }

        public bool HideBanner => Html?.Contains("<span class=\"hide-banner\"/>") ?? false;

        public Image image
        {
            get
            {
                if (oc == null || !IsMember)
                {
                    var i = new Image();
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
                return ImageData.DbUtil.Db.Images.SingleOrDefault(ii => ii.Id == oc.ImageId);
            }
        }

        public static OrgContentInfo Get(int id)
        {
            var q = from oo in DbUtil.Db.Organizations
                    where oo.OrganizationId == id
                    let om = oo.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == Util.UserPeopleId)
                    let oc = DbUtil.Db.OrgContents.SingleOrDefault(cc => cc.OrgId == id && cc.Landing == true)
                    let MemberLeaderType = om.MemberType.AttendanceTypeId
                    select new OrgContentInfo
                    {
                        OrgId = oo.OrganizationId,
                        OrgName = oo.OrganizationName,
                        Inactive = oo.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Inactive,
                        IsMember = om != null && om.MemberTypeId != CmsData.Codes.MemberTypeCode.InActive,
                        IsLeader = (MemberLeaderType ?? 0) == CmsData.Codes.AttendTypeCode.Leader,
                        oc = oc,
                        NotAuthenticated = !Util.UserPeopleId.HasValue
                    };
            var o = q.SingleOrDefault();
            if (o != null && !o.IsMember)
            {
                var oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(o.OrgId))
                    return o;
                o.NotAuthenticated = false;
                o.IsMember = true;
                o.IsLeader = true;
            }
            return o;
        }

        public static OrgContentInfo GetOc(int id)
        {
            var q = from oo in DbUtil.Db.Organizations
                    let oc = DbUtil.Db.OrgContents.SingleOrDefault(cc => cc.Id == id)
                    where oo.OrganizationId == oc.OrgId
                    let om = oo.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == Util.UserPeopleId)
                    let MemberLeaderType = om.MemberType.AttendanceTypeId
                    select new OrgContentInfo
                    {
                        OrgId = oo.OrganizationId,
                        OrgName = oo.OrganizationName,
                        Inactive = oo.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Inactive,
                        IsMember = om != null && om.MemberTypeId != CmsData.Codes.MemberTypeCode.InActive,
                        IsLeader = (MemberLeaderType ?? 0) == CmsData.Codes.AttendTypeCode.Leader,
                        oc = oc,
                        NotAuthenticated = !Util.UserPeopleId.HasValue
                    };
            var o = q.SingleOrDefault();
            if (o != null && !o.IsMember)
            {
                var oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(o.OrgId))
                    return o;
                o.NotAuthenticated = false;
                o.IsMember = true;
                o.IsLeader = true;
            }
            return o;
        }

        public List<OrganizationMember> GetMemberList()
        {
            var org = DbUtil.Db.LoadOrganizationById(OrgId);
            return org.OrganizationMembers.ToList();
        }
    }
}
