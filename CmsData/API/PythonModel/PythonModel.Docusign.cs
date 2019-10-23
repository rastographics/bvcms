namespace CmsData
{
    public partial class PythonModel
    {
        ApiDocuSign _apiDocuSign;
        public ApiDocuSign docusign => _apiDocuSign ?? (_apiDocuSign = new ApiDocuSign());

        public class ApiDocuSign
        {
            public DocuSign.eSign.Model.Document Document()
            {
                return new DocuSign.eSign.Model.Document();
            }

            public DocuSign.eSign.Model.Signer Signer()
            {
                return new DocuSign.eSign.Model.Signer();
            }

            public DocuSign.eSign.Model.SignHere SignHere()
            {
                return new DocuSign.eSign.Model.SignHere();
            }

            public DocuSign.eSign.Model.Recipients Recipients()
            {
                return new DocuSign.eSign.Model.Recipients();
            }

            public DocuSign.eSign.Model.EnvelopeDefinition EnvelopeDefinition()
            {
                return new DocuSign.eSign.Model.EnvelopeDefinition();
            }

            public DocuSign.eSign.Model.RecipientViewRequest RecipientViewRequest()
            {
                return new DocuSign.eSign.Model.RecipientViewRequest();
            }

            public DocuSign.eSign.Model.RecipientViewRequest ListStatusChangesOptions()
            {
                return new DocuSign.eSign.Model.RecipientViewRequest();
            }

            public DocuSign.eSign.Client.ApiClient ApiClient()
            {
                return new DocuSign.eSign.Client.ApiClient();
            }

            public DocuSign.eSign.Api.EnvelopesApi EnvelopesApi()
            {
                return new DocuSign.eSign.Api.EnvelopesApi();
            }
        }
    }
}
