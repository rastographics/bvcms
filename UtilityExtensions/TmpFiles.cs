using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UtilityExtensions
{
    public class TmpFiles
    {
        public static string CreateTmpFile(HttpPostedFileBase file)
        {
            string fileName = Path.GetTempFileName();
            file.SaveAs(fileName);
            return fileName;
        }
    }
}
