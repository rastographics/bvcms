using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter;
using CmsWeb.Models.Api;

namespace CmsWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.MapODataServiceRoute(
                routeName: "ODataApiRoot",
                routePrefix: "api",
                model: MapAllODataModels("CmsWeb.Models.Api").GetEdmModel());

            config.MapODataServiceRoute(
                routeName: "ODataApiLookupRoute",
                routePrefix: "api/lookup",
                model: MapAllODataModels("CmsWeb.Models.Api.Lookup").GetEdmModel());

            config.Filters.Add(new DeveloperAuthorizeAttribute());
            config.MessageHandlers.Add(new ApiMessageLoggingHandler());

            // fix for XML support (use Accept: application/xml)
            var formatters = ODataMediaTypeFormatters.Create();
            config.Formatters.InsertRange(0, formatters);
        }

        private static ODataConventionModelBuilder MapAllODataModels(string namespaceForTypes)
        {
            var builder = new ODataConventionModelBuilder();

            var types =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => string.Equals(x.Namespace, namespaceForTypes, StringComparison.Ordinal));

            foreach (var type in types)
            {
                var name = type.GetCustomAttribute(typeof (ApiMapNameAttribute)) as ApiMapNameAttribute;
                if (name == null) continue;

                var entityType = builder.AddEntityType(type);
                builder.AddEntitySet(name.Name, entityType);
            }

            return builder;
        }
    }
}
