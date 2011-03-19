namespace Crabwise.PdfServe
{
    using System;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.ComponentModel.Composition.Hosting;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class DocumentCache
    {
        private readonly string documentDirectoryPath;
        private readonly string templateDirectoryPath;

        private TimeSpan documentLifespan;

        [ImportMany]
        public Lazy<IDocumentTemplate, IDocumentTemplateMetadata>[] DocumentTemplates { get; private set; }

        internal DocumentCache(string documentDirectoryPath, string templateDirectoryPath)
            : this(documentDirectoryPath, templateDirectoryPath, TimeSpan.FromDays(7))
        {
        }

        internal DocumentCache(string documentDirectoryPath, string templateDirectoryPath, TimeSpan documentLifespan)
        {
            if (!Directory.Exists(documentDirectoryPath))
            {
                throw new DirectoryNotFoundException("Could not find the document storage directory.");
            }

            if (!Directory.Exists(templateDirectoryPath))
            {
                throw new DirectoryNotFoundException("Could not find the template storage directory.");
            }

            this.documentDirectoryPath = documentDirectoryPath;
            this.templateDirectoryPath = templateDirectoryPath;
            this.documentLifespan = documentLifespan;

            var catalog = new DirectoryCatalog(templateDirectoryPath);
            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);

            if (this.DocumentTemplates.Length == 0)
            {
                throw new ArgumentException("Could not find any templates in the provided directory.", "templateDirectoryPath");
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
                return this.documentLifespan;
            }

            set
            {
                this.documentLifespan = value;
            }
        }

        public Stream GetOrCreateDocument(string templateName, IDictionary<string, object> templateData)
        {
            throw new NotImplementedException();
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public void Rebuild()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DocumentCache other = (DocumentCache)obj;
            var documentStoragePathsEqual = this.documentDirectoryPath.Equals(other.documentDirectoryPath, StringComparison.OrdinalIgnoreCase);
            var templateStoragePathsEqual = this.templateDirectoryPath.Equals(other.templateDirectoryPath, StringComparison.OrdinalIgnoreCase);
            return documentStoragePathsEqual && templateStoragePathsEqual;
        }

        public override int GetHashCode()
        {
            return this.documentDirectoryPath.GetHashCode() ^ this.templateDirectoryPath.GetHashCode();
        }
    }
}