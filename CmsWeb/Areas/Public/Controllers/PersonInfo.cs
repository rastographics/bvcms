using System;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class PersonInfo
    {
        public string addr { get; set; }
        public string zip { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string goesby { get; set; }
        public string dob { get; set; } // Date of Birth m/d/yyyy
        public DateTime? Birthdate
        {
            get
            {
                if(Util.IsCultureUS())
                    return dob.ToDate();

                DateTime dt;
                if(dob.DateTryParseUS(out dt))
                    return dt;
                return null;
            }
        }

        public string email { get; set; }
        public string cell { get; set; }
        public string home { get; set; } // home phone
        public string allergies { get; set; } // single line of allergies
        public string grade { get; set; } // grade -1 = preschool, 0 = kindergarten, 1 = first etc. 99 = special
        public string parent { get; set; } // name of parent
        public string emfriend { get; set; } // name of person bringing
        public string emphone { get; set; } // cell phone number of person bringing
        public string churchname { get; set; } // what church they go to
        public int marital { get; set; } // marital status (see lookup table for codes)
        public int gender { get; set; } // see lookup table for codes
        public int campusid { get; set; } // see lookup table for codes
        public string activeother { get; set; } // active in another church (true / false)
        public bool AskChurch { get; set; } // whether they were asked this
        public bool AskChurchName { get; set; } // or this
        public bool AskGrade { get; set; } // or this
        public bool AskEmFriend { get; set; } // or this
    }
}
