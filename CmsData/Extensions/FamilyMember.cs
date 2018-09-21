using System;
using System.Collections.Generic;
using System.Linq;
using ImageData;
using UtilityExtensions;
using System.Text;
using CmsData.Classes.GoogleCloudMessaging;
using CmsData.Codes;
using Community.CsharpSqlite;

namespace CmsData.View
{
    public partial class FamilyMember
    {
        public int? AgeDisplay => Person.AgeDisplay(Age, Id);
    }
}
