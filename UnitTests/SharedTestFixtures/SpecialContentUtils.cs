using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsData;
using UtilityExtensions;
using Xunit;

namespace SharedTestFixtures
{
    [Collection(Collections.Database)]
    public static class SpecialContentUtils
    {
        public static Content CreateSpecialContent(int newType, string newName, int? newRole)
        {
            var content = new Content();
            content.Name = newName;
            content.TypeID = newType;
            content.RoleID = newRole ?? 0;
            content.Title = newName;
            content.Body = "";
            content.DateCreated = DateTime.Now;

            DbUtil.Db = CMSDataContext.Create(Util.Host);
            DbUtil.Db.Contents.InsertOnSubmit(content);
            DbUtil.Db.SubmitChanges();

            return content;
        }

        public static void UpdateSpecialContent(int id, string name, string title, string body, bool? snippet, int? roleid, string contentKeyWords, string stayaftersave = null)
        {
            var content = DbUtil.Db.Contents.SingleOrDefault(c => c.Id == id);
            content.Name = name;
            content.Title = string.IsNullOrWhiteSpace(title) ? name : title;
            content.Body = body;
            content.RemoveGrammarly();
            content.RoleID = roleid ?? 0;
            content.Snippet = snippet;
            content.SetKeyWords(DbUtil.Db, contentKeyWords.SplitStr(",").Select(vv => vv.Trim()).ToArray());
        }

        public static void DeleteSpecialContent(int id)
        {
            DbUtil.Db.ExecuteCommand("DELETE FROM dbo.ContentKeywords WHERE Id = {0}", id);
            DbUtil.Db.ExecuteCommand("DELETE FROM dbo.Content WHERE Id = {0}", id);
        }
    }
}
