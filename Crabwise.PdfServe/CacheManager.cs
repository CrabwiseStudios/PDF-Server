namespace Crabwise.PdfServe
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class CacheManager
    {
        private readonly SortedSet<string> documentDirectoryPaths = new SortedSet<string>(new IgnoreCaseComparer());

        public CacheManager()
        {

        }

        public CacheManager(TimeSpan defaultDocumentLifespan)
        {

        }

        public DocumentCache InitializeCache(string documentDirectoryPath, string templateDirectoryPath)
        {
            // Normalize the paths for comparison purposes.
            var normalizedDocumentDirectoryPath = Path.GetFullPath(documentDirectoryPath).TrimEnd('\\');
            var normalizedTemplateDirectoryPath = Path.GetFullPath(templateDirectoryPath).TrimEnd('\\');

            // Only allow one DocumentCache per document directory.
            if (documentDirectoryPaths.Contains(documentDirectoryPath))
            {
                throw new InvalidOperationException("There is already a DocumentCache for this document directory.");
            }

            var documentCache = new DocumentCache(normalizedDocumentDirectoryPath, normalizedTemplateDirectoryPath);
            documentDirectoryPaths.Add(normalizedDocumentDirectoryPath);

            return documentCache;
        }
    }
}
