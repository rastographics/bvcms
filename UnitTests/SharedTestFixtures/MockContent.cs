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
            if (name == null)
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
        public static Content CreateHTMLContent(CMSDataContext db, string name = null, int? TypeID = null, int? ThumbID = 0, int? RoleID = 0, int? OwnerID = 0)
        {
            if (name == null)
            {
                name = "My Test HTML Content";
            }
            if (TypeID == null)
            {
                TypeID = 1;
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
        public static Content CreateSqlContent(CMSDataContext db, string name = null, int? TypeID = null, int? ThumbID = 0, int? RoleID = 0, int? OwnerID = 0)
        {
            if (name == null)
            {
                name = "My Test SQL Content";
            }
            if (TypeID == null)
            {
                TypeID = 4;
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
        public static void DeleteHTMLContent(CMSDataContext db, Content content)
        {
            db.Contents.DeleteOnSubmit(content);
            db.SubmitChanges();
        }
        public static void DeleteSqlContent(CMSDataContext db, Content content)
        {
            db.Contents.DeleteOnSubmit(content);
            db.SubmitChanges();
        }
    }
}
