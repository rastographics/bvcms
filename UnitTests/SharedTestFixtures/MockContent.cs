using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockContent
    {
        public static Content CreatePythonContent(CMSDataContext db, string name = null, int? TypeID = null, int? ThumbID = 0, int? RoleID = 0, int? OwnerID = 0)
        {
            if(name == null)
            {
                name = "My Test Python Content";
            }
            if (TypeID == null)
            {
                TypeID = 5;
            }
            if (ThumbID == null)
            {
                ThumbID = 0;
            }
            if (RoleID == null)
            {
                RoleID = 0;
            }
            if (OwnerID == null)
            {
                OwnerID = 0;
            }
            var content = new Content()
            {
                Name = name,
                TypeID = (int)TypeID,
                ThumbID = (int)ThumbID,
                RoleID = (int)RoleID,
                OwnerID = (int)OwnerID
            };

            db.Contents.InsertOnSubmit(content);
            db.SubmitChanges();

            return content;
        }
        public static void DeletePythonContent(CMSDataContext db, Content content)
        {
            db.Contents.DeleteOnSubmit(content);
            db.SubmitChanges();
        }
    }
}
