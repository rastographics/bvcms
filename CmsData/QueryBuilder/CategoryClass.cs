/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using CsvHelper;
using UtilityExtensions;

namespace CmsData
{
    public class CategoryClass
    {
        private const string CategoryClassListKey = "CategoryClassList";
        public string Title => Name.SpaceCamelCase();
        public string Name { get; set; }
        public List<FieldClass> Fields { get; set; }

        public CategoryClass(string name)
        {
            Name = name;
            Fields = new List<FieldClass>();
        }
        public static List<CategoryClass> Categories
        {
            get
            {
                var categories = HttpRuntime.Cache[CategoryClassListKey] as List<CategoryClass>;
                if (categories != null)
                    return categories;

                var cats = new[] { "Grouping",
                    "Personal", "Family", "Address", "ContactInfo",
                    "Ministry", "Membership",
                    "Enrollments", "EnrollmentHistory", "CurrentOrg", "Volunteer",
                    "RecentAttendance", "AttendanceDates",
                    "ExtraValues", "Contributions", "Miscellaneous", "Admin",
                };
                categories = cats.Select(s => new CategoryClass(s)).ToList();

                var text = Properties.Resources.FieldMap;
                var csv = new CsvReader(new StringReader(text));

                foreach (var cf in csv.GetRecords<ConditionConfig>())
                    FieldClass.AddFieldClass(cf, categories);

                HttpRuntime.Cache.Insert(CategoryClassListKey, categories, null,
                    Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0));
                return categories;
            }
        }
    }
}
