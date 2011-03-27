namespace Crabwise.PdfServe
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    public class DocumentCache
    {
        private readonly string documentDirectoryPath;
        private readonly string templateDirectoryPath;
        private readonly ConcurrentDictionary<byte[], CachedDocument> cachedDocuments = new ConcurrentDictionary<byte[], CachedDocument>();
        private readonly ReaderWriterLockSlim documentLifespanLock = new ReaderWriterLockSlim();

        private TimeSpan documentLifespan;

        [ImportMany(typeof(IDocumentTemplate))]
        public Lazy<IDocumentTemplate, IDocumentTemplateMetadata>[] DocumentTemplates { get; private set; }

        internal DocumentCache(string documentDirectoryPath, string templateDirectoryPath)
            : this(documentDirectoryPath, templateDirectoryPath, TimeSpan.FromDays(7))
        {
        }

        internal DocumentCache(string documentDirectoryPath, string templateDirectoryPath, TimeSpan documentLifespan)
        {
            if (!Directory.Exists(documentDirectoryPath))
            {
                throw new DirectoryNotFoundException("Could not find the document directory at \"" + documentDirectoryPath + "\".");
            }

            if (!Directory.Exists(templateDirectoryPath))
            {
                throw new DirectoryNotFoundException("Could not find the template directory at \"" + templateDirectoryPath + "\".");
            }

            this.documentDirectoryPath = documentDirectoryPath;
            this.templateDirectoryPath = templateDirectoryPath;
            this.DocumentLifespan = documentLifespan;

            var catalog = new DirectoryCatalog(templateDirectoryPath);
            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);

            if (this.DocumentTemplates.Length == 0)
            {
                throw new ArgumentException("Could not find any templates in the provided directory.", "templateDirectoryPath");
            }

            var pdfDocuments = Directory.GetFiles(documentDirectoryPath, "*.pdf");
            foreach (var document in pdfDocuments)
            {
                var base64Hash = Path.GetFileNameWithoutExtension(document);
                var hash = GetHashArray(base64Hash);
                this.cachedDocuments.TryAdd(hash, new CachedDocument(document));
            }
        }

        public string DocumentDirectoryPath
        {
            get
            {
                return this.documentDirectoryPath;
            }
        }

        public string TemplateDirectoryPath
        {
            get
            {
                return this.templateDirectoryPath;
            }
        }

        public TimeSpan DocumentLifespan
        {
            get
            {
                try
                {
                    this.documentLifespanLock.EnterReadLock();
                    return this.documentLifespan;
                }
                finally
                {
                    this.documentLifespanLock.ExitReadLock();
                }
            }

            set
            {
                try
                {
                    this.documentLifespanLock.EnterUpgradeableReadLock();
                    if (this.documentLifespan != value)
                    {
                        try
                        {
                            this.documentLifespanLock.EnterWriteLock();

                            this.documentLifespan = value;
                        }
                        finally
                        {
                            this.documentLifespanLock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    this.documentLifespanLock.ExitUpgradeableReadLock();
                }
            }
        }

        public Stream GetOrCreateDocument(string templateName, IDictionary<string, object> templateData)
        {
            byte[] hash = ComputeHash(templateName, templateData);
            var base64Hash = GetHashString(hash);
            var path = Path.Combine(this.documentDirectoryPath, base64Hash + ".pdf");

            IDocumentTemplate documentTemplate = null;
            foreach (var template in this.DocumentTemplates)
            {
                if (template.Metadata.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase))
                {
                    documentTemplate = template.Value;
                    break;
                }
            }

            if (documentTemplate == null)
            {
                throw new ArgumentException(string.Format("Could not find a template by the name of \"{0\".", templateName), "templateName");
            }

            var cachedDocument = this.cachedDocuments.GetOrAdd(hash, new CachedDocument(path, documentTemplate, templateData));
            return cachedDocument.GetContentStream();
        }

        public void Cleanup()
        {
            foreach (var cachedDocument in this.cachedDocuments.Values)
            {
                if (cachedDocument.CreationDate + this.documentLifespan < DateTime.UtcNow)
                {
                    cachedDocument.MarkForDeletion();
                }
            }
        }

        private static string GetHashString(byte[] hash)
        {
            return Convert.ToBase64String(hash).Replace('/', '-');
        }

        private static byte[] GetHashArray(string hash)
        {
            return Convert.FromBase64String(hash.Replace('-', '/'));
        }

        private static byte[] ComputeHash(string templateName, IDictionary<string, object> templateData)
        {
            string identifier = templateName;
            foreach (var keyValue in templateData)
            {
                identifier += keyValue.Key + keyValue.Value;
            }

            byte[] hash;
            using (var sha512 = new SHA512Managed())
            {
                hash = sha512.ComputeHash(Encoding.Unicode.GetBytes(identifier));
            }

            return hash;
        }
    }
}