using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Dialog.Models
{
    public class CheckImageModel
    {
        public int ImageId { get; set; }

        public byte[] checkImageBytes { get; set; }
        public CheckImageModel() { }
    }


}
