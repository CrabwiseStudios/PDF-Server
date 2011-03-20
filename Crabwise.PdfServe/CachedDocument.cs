namespace Crabwise.PdfServe
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using iTextSharp.text.pdf;

    internal class CachedDocument
    {
        private readonly CountdownEvent deletionCountdown = new CountdownEvent(1);
        private readonly string path = null;
        private readonly object deletionLock = new object();
        private readonly object creationLock = new object();
        private readonly IDocumentTemplate template;
        private readonly IDictionary<string, object> templateData;
        private bool created = false;

        public CachedDocument(string path, IDocumentTemplate template, IDictionary<string, object> templateData)
        {
            this.path = path;
            this.CreationDate = new DateTime();
            this.template = template;
            this.templateData = templateData;
        }

        public DateTime CreationDate { get; private set; }

        public bool MarkedForDeletion { get; private set; }

        public bool Deleted { get; private set; }

        public void MarkForDeletion()
        {
            if (!this.MarkedForDeletion)
            {
                lock (deletionLock)
                {
                    if (!this.MarkedForDeletion)
                    {
                        this.MarkedForDeletion = true;

                        Task.Factory.StartNew(() =>
                        {
                            deletionCountdown.Signal();
                            deletionCountdown.Wait();
                            if (this.path != null)
                            {
                                File.Delete(this.path);
                            }

                            this.Deleted = true;
                        });
                    }
                }
            }
        }

        public Stream GetContentStream()
        {
            bool deletionLock = deletionCountdown.TryAddCount();
            if (!deletionLock)
            {
                throw new FileNotFoundException("The cached file has been deleted.");
            }
            if (!created)
            {
                lock (creationLock)
                {
                    if (!created)
                    {
                        var dynamicTemplateData = new DynamicTemplateData(templateData);
                        using (var document = template.CreateDocument())
                        {
                            using (var fileStream = File.Create(path))
                            {
                                using (var pdfWriter = PdfWriter.GetInstance(document, fileStream))
                                {
                                    document.Open();
                                    template.WriteDocument(document, dynamicTemplateData);
                                    document.Close();
                                }
                            }
                        }

                        this.CreationDate = File.GetCreationTimeUtc(path);
                        this.created = true;
                    }
                }
            }

            return File.OpenRead(path);
        }
    }
}
