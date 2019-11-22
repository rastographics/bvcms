using System.Net;

namespace CmsData
{
    public partial class PythonModel
    {
        ApiDocuSign _apiDocuSign;
        public ApiDocuSign docusign => _apiDocuSign ?? (_apiDocuSign = new ApiDocuSign());

        public class ApiDocuSign
        {
            public DocuSign.eSign.Client.ApiClient ApiClient(string basePath = "https://www.docusign.net/restapi", WebProxy proxy = null)
            {
                return new DocuSign.eSign.Client.ApiClient(basePath, proxy);
            }

            public DocuSign.eSign.Client.Configuration Configuration(DocuSign.eSign.Client.ApiClient apiClient = null)
            {
                return new DocuSign.eSign.Client.Configuration(apiClient);
            }

            public DocuSign.eSign.Model.EnvelopeDefinition EnvelopeDefinition()
            {
                return new DocuSign.eSign.Model.EnvelopeDefinition();
            }

            public DocuSign.eSign.Api.EnvelopesApi EnvelopesApi(DocuSign.eSign.Client.Configuration configuration = null)
            {
                return new DocuSign.eSign.Api.EnvelopesApi(configuration);
            }

            public DocuSign.eSign.Api.EnvelopesApi EnvelopesApi(string basePath)
            {
                return new DocuSign.eSign.Api.EnvelopesApi(basePath);
            }

            public DocuSign.eSign.Model.Document Document()
            {
                return new DocuSign.eSign.Model.Document();
            }

            public DocuSign.eSign.Api.EnvelopesApi.ListStatusChangesOptions ListStatusChangesOptions()
            {
                return new DocuSign.eSign.Api.EnvelopesApi.ListStatusChangesOptions();
            }

            public DocuSign.eSign.Model.Recipients Recipients()
            {
                return new DocuSign.eSign.Model.Recipients();
            }

            public DocuSign.eSign.Model.RecipientViewRequest RecipientViewRequest()
            {
                return new DocuSign.eSign.Model.RecipientViewRequest();
            }

            public DocuSign.eSign.Model.Signer Signer()
            {
                return new DocuSign.eSign.Model.Signer();
            }

            public DocuSign.eSign.Model.SignHere SignHere()
            {
                return new DocuSign.eSign.Model.SignHere();
            }

            public DocuSign.eSign.Model.Tabs Tabs()
            {
                return new DocuSign.eSign.Model.Tabs();
            }

            public DocuSign.eSign.Model.TemplateRole TemplateRole()
            {
                return new DocuSign.eSign.Model.TemplateRole();
            }

            public DocuSign.eSign.Model.TemplateTabs TemplateTabs()
            {
                return new DocuSign.eSign.Model.TemplateTabs();
            }

            public DocuSign.eSign.Model.Text Text()
            {
                return new DocuSign.eSign.Model.Text();
            }
        }
    }
}
