using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using CmsData;
using CmsData.Properties;

public class CategoryClass2
{
    public string Title { get; set; }
    public string Name
    {
        get
        {
            return Title.Replace(" ", "");
        }
    }

    //public string Name { get; set; }

    public IEnumerable<FieldClass> Fields { get; set; }

    public static List<CategoryClass> Categories
    {
        get
        {
            var categories = (List<CategoryClass>)HttpRuntime.Cache["FieldCategories2"];
            if (categories == null)
            {
                var xdoc = XDocument.Parse(Resources.FieldMap3);
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

    private static string Attr(XElement e, string name)
    {
        var a = e.Attribute(name);
        if (a != null)
            return a.Value;
        return null;
    }
}
