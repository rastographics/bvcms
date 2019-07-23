using ImageData;
using System.IO;
using System.Web;

namespace CMSImage
{
    public class DocumentsData
    {
        public static int StoreImageFromDocument(HttpPostedFileBase file)
        {
            Image i = GetImageFromFile(file);
            DbUtil.Db.Images.InsertOnSubmit(i);
            DbUtil.Db.SubmitChanges();
            return i.Id;
        }

        private static Image GetImageFromFile(HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return Image.CreateImageFromType(data, MimeMapping.GetMimeMapping(file.FileName));
        }
    }
}
