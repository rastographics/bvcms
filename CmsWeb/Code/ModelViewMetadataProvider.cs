using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.Code
{
    public class ModelViewMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        readonly Regex camelCaseRegex = new Regex(@"\B\p{Lu}\p{Ll}", RegexOptions.Compiled);

        protected override ModelMetadata GetMetadataForProperty(
           Func<object> modelAccessor,
           Type containerType,
           PropertyDescriptor propertyDescriptor)
        {
            var metadata = base.GetMetadataForProperty(modelAccessor, containerType, propertyDescriptor);
            if (metadata.DisplayName == null)
                metadata.DisplayName = DisplayNameFromCamelCase(metadata.GetDisplayName());
            return metadata;
        }

        string DisplayNameFromCamelCase(string name)
        {
            name = camelCaseRegex.Replace(name, " $0");
            if (name.EndsWith(" Id"))
                name = name.Substring(0, name.Length - 3);
            return name;
        }
    }
}