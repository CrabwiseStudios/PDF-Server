using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.IO;
using System.Reflection;

namespace Crabwise.PdfServe.Web.Services
{
    public class Global : HttpApplication
    {
        private static DocumentCache pdfDocumentCache;

        public static DocumentCache PdfDocumentCache
        {
            get
            {
                return pdfDocumentCache;
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();

            var cacheManager = new CacheManager();
            var documentsDirectory = Server.MapPath("~/Documents");
            var templatesDirectory = Server.MapPath("~/Templates");

            //if (Directory.Exists(documentsDirectory))
            //{
            //    Directory.Delete(documentsDirectory, true);
            //}
            //Directory.CreateDirectory(documentsDirectory);

            //if (!Directory.Exists(templatesDirectory))
            //{
            //    Directory.CreateDirectory(templatesDirectory);
            //}

            pdfDocumentCache = cacheManager.CreateCache(documentsDirectory, templatesDirectory);
        }

        private void RegisterRoutes()
        {
            // Edit the base address of Service1 by replacing the "Service1" string below
            RouteTable.Routes.Add(new ServiceRoute("", new WebServiceHostFactory(), typeof(PdfServe)));
        }
    }
}
