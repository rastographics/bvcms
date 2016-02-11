using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.CheckInAPI
{
    public class CheckInFamily
    {
        public int id = 0;
        public string name = "";

        public string picture = "";

        public bool locked = false;

        public List<CheckInFamilyMember> members = new List<CheckInFamilyMember>();

        public CheckInFamily(int id, string name, bool locked)
        {
            this.id = id;
            this.name = name;
            this.locked = locked;

            Family family = DbUtil.Db.Families.SingleOrDefault(f => f.FamilyId == id);

            if (family.Picture != null)
            {
                var image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == family.Picture.SmallId);

                if (image != null)
                {
                    picture = Convert.ToBase64String(image.Bits);
                }
            }
        }

        public void addMember(CmsData.View.CheckinFamilyMember newMember, int day, int tzOffset)
        {
            if (members.Count == 0)
            {
                members.Add(new CheckInFamilyMember(newMember, day, tzOffset));
            }
            else
            {
                foreach (var member in members)
                {
                    if (member.id == newMember.Id)
                    {
                        member.addOrg(newMember, day, tzOffset);
                        return;
                    }
                }

                members.Add(new CheckInFamilyMember(newMember, day, tzOffset));
            }
        }
    }
}