using System;
using System.Linq;
using CmsData.View;

namespace CmsWeb.MobileAPI
{
    public class MobileTask
    {
        public static int TYPE_PENDING = 0;
        public static int TYPE_ACTIVE = 1;
        public static int TYPE_DELEGATED = 2;

        public int id = 0;
        public int type = 0;

        public string status = "";
        public int statusID = 0;

        public DateTime created = DateTime.Now;
        public DateTime? due = null;

        public bool completeWithContact = false;

        public string owner = "";
        public int ownerID = 0;

        public string delegated = "";
        public int delegatedID = 0;

        public string about = "";
        public int aboutID = 0;

        public string desc = "";
        public string notes = "";
        public string declineReason = "";

        public string picture = "";
        public int pictureX = 0;
        public int pictureY = 0;

        public MobileTask populate(IncompleteTask task, int currentPeopleID)
        {
            id = task.Id;

            status = task.Status;
            statusID = task.StatusId ?? 0;

            created = task.CreatedOn;
            due = task.Due;

            completeWithContact = task.ForceCompleteWContact ?? false;

            owner = task.Owner;
            ownerID = task.OwnerId;

            delegated = task.DelegatedTo;
            delegatedID = task.CoOwnerId ?? 0;

            about = task.About ?? "";
            aboutID = task.WhoId ?? 0;

            desc = task.Description;
            notes = task.Notes;
            declineReason = task.DeclineReason;

            if (task.AboutPictureId != null)
            {
                var image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == task.AboutPictureId);

                if (image != null)
                {
                    picture = Convert.ToBase64String(image.Bits);
                    pictureX = task.PictureX ?? 0;
                    pictureY = task.PictureY ?? 0;
                }
            }

            if (delegatedID > 0 && delegatedID != currentPeopleID)
            {
                type = MobileTask.TYPE_DELEGATED;
            }
            else if (statusID == 10)
            {
                type = MobileTask.TYPE_ACTIVE;
            }
            else
            {
                type = MobileTask.TYPE_PENDING;
            }

            return this;
        }
    }
}