﻿print (str([
    model.docusign.ApiClient() is None,
    model.docusign.Configuration() is None,
    model.docusign.Document() is None,
    model.docusign.EnvelopeDefinition() is None,
    model.docusign.EnvelopesApi() is None,
    model.docusign.EnvelopesApi("https://localhost/") is None,
    model.docusign.ListStatusChangesOptions() is None,
    model.docusign.Recipients() is None,
    model.docusign.RecipientViewRequest() is None,
    model.docusign.Signer() is None,
    model.docusign.SignHere() is None,
    model.docusign.TemplateRole() is None,
    ]))
