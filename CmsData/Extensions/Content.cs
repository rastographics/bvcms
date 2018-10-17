using System.Collections.Generic;
using UtilityExtensions;
using System.Linq;

namespace CmsData
{
    public partial class Content
    {
        public string ThumbUrl => $"/Image/{ThumbID}?v={DateCreated?.Ticks ?? 0}";

        public void RemoveGrammarly()
        {
            Body = Body.RemoveGrammarly();
        }
        public void SetKeyWords(CMSDataContext db, string[] keywords)
        {
            if (keywords == null || keywords.Length == 0)
            {
                db.ContentKeyWords.DeleteAllOnSubmit(ContentKeyWords);
                db.SubmitChanges();
                return;
            }
            var deletes = (from kw in ContentKeyWords
                           where !keywords.Contains(kw.Word)
                           select kw).ToList();
            db.ContentKeyWords.DeleteAllOnSubmit(deletes);
            var addlist = (from s in keywords
                     join r in ContentKeyWords on s equals r.Word into g
                     from t in g.DefaultIfEmpty()
                     where t == null
                     select s).ToList();
            foreach (var s in addlist)
                ContentKeyWords.Add(new ContentKeyWord {Id = Id, Word = s});
        }
    }
}
