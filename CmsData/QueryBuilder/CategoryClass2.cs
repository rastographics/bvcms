/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.Caching;

namespace CmsData
{
    public class CategoryClass2
    {
        public string Title { get; set; }
        public string TopMessage { get; set; }
        public string Name { get; set; }
        public IEnumerable<FieldClass2> Fields { get; set; }

        public static List<CategoryClass2> Categories
        {
            get
            {
                var categories = (List<CategoryClass2>)HttpRuntime.Cache["CategoryClass2List"];
                if (categories == null)
                {
                    var xdoc = XDocument.Parse(Properties.Resources.FieldMap3);

                    var q = from c in xdoc.Root?.Elements()
                            select new CategoryClass2
                            {
                                Name = c.Name.LocalName,
                                Title = c.Attribute("Title") != null ? c.Attribute("Title").Value : c.Name.LocalName,
                                TopMessage = c.Attribute("TopMessage") != null ? c.Attribute("TopMessage").Value : "",
                                Fields = (from f in c.Elements()
                                          select new FieldClass2
                                          {
                                              CategoryTitle = Attr(c, "Title"),
                                              Name = f.Name.LocalName,
                                              Title = Attr(f, "Title"),
                                              QuartersTitle = Attr(f, "QuartersLabel"),
                                              Type = FieldClass2.Convert(Attr(f, "Type")),
                                              Params = Attr(f, "Params"),
                                              DataSource = Attr(f, "DataSource"),
                                              DataValueField = Attr(f, "DataValueField"),
                                              Description = f.Value,
                                          }).ToList()
                            };
                    categories = q.ToList();
                    HttpRuntime.Cache.Insert("CategoryClass2List", categories, null,
                        DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
#if DEBUG2
                    foreach(var e in Enum.GetValues(typeof(QueryType)).Cast<QueryType>())
                        if(!categories.Any(cc => cc.Fields.Any(ff => ff.Name == e.ToString())))
                            Debug.WriteLine(e.ToString());
#endif
                }
                return categories;
            }
        }
        private static string Attr(XElement e, string name)
        {
            var a = e.Attribute(name);
            return a?.Value;
        }
    }
}
