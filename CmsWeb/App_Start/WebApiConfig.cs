using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter;
using CmsWeb.Models.Api;
using CmsWeb.Models.Api.Lookup;
using Newtonsoft.Json.Serialization;

namespace CmsWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<ApiPerson>("People");
            builder.EntitySet<ApiContribution>("Contributions");
            builder.EntitySet<ApiContributionFund>("Funds");
            builder.EntitySet<ApiOrganization>("Organizations");
            builder.EntitySet<ApiOrganizationMember>("OrganizationMembers");
            builder.EntitySet<ApiChAiPerson>("ChAiPeople");
            builder.EntitySet<ApiChAiGift>("ChAiGifts");

            config.MapODataServiceRoute(
                routeName: "ODataApiRoot",
                routePrefix: "api",
                model: builder.GetEdmModel());

            var builderlookup = new ODataConventionModelBuilder();
            builderlookup.EntitySet<ApiLookup>("Campuses");
            builderlookup.EntitySet<ApiLookup>("Genders");
            builderlookup.EntitySet<ApiLookup>("MaritalStatuses");
            builderlookup.EntitySet<ApiLookup>("FamilyPositions");
            builderlookup.EntitySet<ApiLookup>("ContributionTypes");
            builderlookup.EntitySet<ApiLookup>("BundleHeaderTypes");

            config.MapODataServiceRoute(
                routeName: "ODataApiLookupRoute",
                routePrefix: "api/lookup",
                model: builderlookup.GetEdmModel());
            config.Filters.Add(new ApiAuthorizeAttribute());
            config.MessageHandlers.Add(new ApiMessageLoggingHandler());

            // fix for XML support (use Accept: application/xml)
            var formatters = ODataMediaTypeFormatters.Create();
            config.Formatters.InsertRange(0, formatters);
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        }
    }
}
