using System.Data.Linq;
using System.Linq;
using System.Text;

namespace ImageData
{
    public partial class CMSImageDataContext : DataContext
    {
        public Image UpdateImageFromBits(int imageID, byte[] bits)
        {
            Image image;
            var images = from t in Images
                            where t.Id == imageID
                            select t;

            image = images.FirstOrDefault();
            if (image != null)
            {
                image.LoadImageFromBits(bits);
                SubmitChanges();
            }
            return image;
        }

        public void DeleteOnSubmit(int? imageid)
        {
            var i = Images.SingleOrDefault(img => img.Id == imageid);
            if (i != null)
            {
                Images.DeleteOnSubmit(i);
            }
        }

        public string Content(int id)
        {
            var img = Images.SingleOrDefault(i => i.Id == id);
            if (img?.Mimetype != "text/plain")
            {
                return null;
            }

            return Encoding.ASCII.GetString(img.Bits);
        }
    }
}
