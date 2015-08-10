using System;

namespace CmsWeb.Models.Api
{
    public class ApiMapNameAttribute : Attribute
    {
        public string Name { get; }

        public ApiMapNameAttribute(string name)
        {
            Name = name;
        }
    }
}
