using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;

namespace Crabwise.PdfServe.Web.Services
{
    [ServiceContract]
    public interface IPdfService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "{templateName}", Method = "POST")]
        Stream CreatePdf(string templateName, IDictionary<string, object> templateData);
    }
}
