/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Community.CsharpSqlite;
using UtilityExtensions;
using System.Web.Caching;

namespace CmsData
{
    public class CategoryClass
    {
        public string Title { get; set; }
        public string Name
        {
            get
            {
                return Title.Replace(" ", "");
            }
        }
        public IEnumerable<FieldClass> Fields { get; set; }

        public static List<CategoryClass> Categories
        {
            get
            {
                if (DbUtil.Db.UserPreference("NewCategories", "false") == "false")
                {
                    var categories = (List<CategoryClass>)HttpRuntime.Cache["FieldCategories"];
                    if (categories == null)
                    {
                        var xdoc = XDocument.Parse(Properties.Resources.FieldMap2);
                        var q = from c in xdoc.Descendants("Category")
                                select new CategoryClass
                                {
                                    Title = c.Attribute("Title").Value,
                                    Fields = from f in c.Descendants("Field")
                                             select new FieldClass
                                             {
                                                 CategoryTitle = (string)c.Attribute("Title").Value,
                                                 Name = (string)f.Attribute("Name"),
                                                 Title = (string)f.Attribute("Title"),
                                                 QuartersTitle = (string)f.Attribute("QuartersLabel"),
                                                 DisplayAs = (string)f.Attribute("DisplayAs"),
                                                 Type = FieldClass.Convert((string)f.Attribute("Type")),
                                                 Params = (string)f.Attribute("Params"),
                                                 DataSource = (string)f.Attribute("DataSource"),
                                                 DataValueField = (string)f.Attribute("DataValueField"),
                                                 Description = f.Value,
                                             }
                                };
                        categories = q.ToList();
                        HttpRuntime.Cache.Insert("FieldCategories", categories, null,
                            DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                    }
                    return categories;
                }
                else
                {
                    var categories = (List<CategoryClass>)HttpRuntime.Cache["FieldCategories2"];
                    if (categories == null)
                    {
                        var xdoc = XDocument.Parse(Properties.Resources.FieldMap3);
                        var q = from c in xdoc.Root.Elements()
                                select new CategoryClass
                                {
                                    Title = c.Attribute("Title") != null ? c.Attribute("Title").Value : c.Name.LocalName,
                                    Fields = (from f in c.Elements()
                                              select new FieldClass
                                              {
                                                  CategoryTitle = Attr(c, "Title"),
                                                  Name = f.Name.LocalName,
                                                  Title = Attr(f, "Title"),
                                                  QuartersTitle = Attr(f, "QuartersLabel"),
                                                  DisplayAs = Attr(f, "DisplayAs"),
                                                  Type = FieldClass.Convert(Attr(f, "Type")),
                                                  Params = Attr(f, "Params"),
                                                  DataSource = Attr(f, "DataSource"),
                                                  DataValueField = Attr(f, "DataValueField"),
                                                  Description = f.Value,
                                              }).ToList()
                                };
                        categories = q.ToList();
                        HttpRuntime.Cache.Insert("FieldCategories2", categories, null,
                            DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                    }
                    return categories;
                }
            }
        }
        private static string Attr(XElement e, string name)
        {
            var a = e.Attribute(name);
            if (a != null)
                return a.Value;
            return null;
        }
    }
}
