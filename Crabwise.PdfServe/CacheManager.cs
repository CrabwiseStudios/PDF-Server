namespace Crabwise.PdfServe
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Dynamic;

    public class CacheManager
    {
        private readonly SortedSet<string> documentDirectoryPaths = new SortedSet<string>(new IgnoreCaseComparer());

        public CacheManager()
            : this(TimeSpan.Zero)
        {
        }

        public CacheManager(TimeSpan defaultDocumentLifespan)
        {
            this.DefaultDocumentLifespan = defaultDocumentLifespan;
        }

        public TimeSpan DefaultDocumentLifespan { get; private set; }

        public DocumentCache CreateCache(string documentDirectoryPath, string templateDirectoryPath)
        {
            // Normalize the paths for comparison purposes.
            var normalizedDocumentDirectoryPath = Path.GetFullPath(documentDirectoryPath).TrimEnd('\\');
            var normalizedTemplateDirectoryPath = Path.GetFullPath(templateDirectoryPath).TrimEnd('\\');

            // Only allow one DocumentCache per document directory.
            if (documentDirectoryPaths.Contains(documentDirectoryPath))
            {
                throw new InvalidOperationException("There is already a DocumentCache for this document directory.");
            }

            DocumentCache documentCache;
            if (this.DefaultDocumentLifespan == TimeSpan.Zero)
            {
                documentCache = new DocumentCache(normalizedDocumentDirectoryPath, normalizedTemplateDirectoryPath);
            }
            else
            {
                documentCache = new DocumentCache(normalizedDocumentDirectoryPath, normalizedTemplateDirectoryPath, this.DefaultDocumentLifespan);
            }

            documentDirectoryPaths.Add(normalizedDocumentDirectoryPath);

            return documentCache;
        }
    }
}
