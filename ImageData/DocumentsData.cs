using ImageData;
using System.IO;
using System.Web;

namespace ImageData
{
    public class DocumentsData
    {
        public static int StoreImageFromDocument(CMSImageDataContext db, HttpPostedFileBase file)
        {
            Image i = GetImageFromFile(db, file);
            return i.Id;
        }

        private static Image GetImageFromFile(CMSImageDataContext db, HttpPostedFileBase file)
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
            var mimetype = UtilityExtensions.MimeTypes.ShortTypeFromMimeType(MimeMapping.GetMimeMapping(file.FileName));
            return Image.CreateImageFromType(data, mimetype, db);
        }
    }
}
