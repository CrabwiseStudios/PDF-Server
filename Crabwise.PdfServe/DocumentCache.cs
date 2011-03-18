namespace Crabwise.PdfServe.Web.Services
{
    using System.IO;

    public static class DocumentCache
    {
        public static void Initialize(string documentStoragePath, string templateStoragePath)
        {
            if (!Directory.Exists(documentStoragePath))
            {
                throw new DirectoryNotFoundException("Could not find the document storage directory.");
            }

            if (!Directory.Exists(templateStoragePath))
            {
                throw new DirectoryNotFoundException("Could not find the template storage directory.");
            }
        }
    }
}