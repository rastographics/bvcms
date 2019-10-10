namespace UtilityExtensions
{
    public class MimeTypes
    {
        public static string ShortTypeFromMimeType(string mimeType)
        {
            string shortType = "unknow";
            switch (mimeType)
            {
                case "application/msword":
                    shortType = ".doc";
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    shortType = ".docx";
                    break;
                case "text/csv":
                    shortType = ".csv";
                    break;
                case "image/jpeg":
                    shortType = ".jpg";
                    break;
                case "application/pdf":
                    shortType = ".pdf";
                    break;
                case "application/vnd.ms-excel":
                    shortType = ".xls";
                    break;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    shortType = ".xlsx";
                    break;
            }
            return shortType;
        }

        public static string MimeTypeFromShortType(string shortType)
        {
            string mimeType = "unknow";
            switch (shortType)
            {
                case ".doc":
                    mimeType = "application/msword";
                    break;
                case ".docx":
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".csv":
                    mimeType = "text/csv";
                    break;
                case ".jpg":
                    mimeType = "image/jpeg";
                    break;
                case ".pdf":
                    mimeType = "application/pdf";
                    break;
                case ".xls":
                    mimeType = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
            }
            return mimeType;
        }
    }
}
