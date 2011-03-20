namespace Crabwise.PdfServe
{
    using System;
    using System.ComponentModel.Composition;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DocumentTemplateMetadataAttribute : ExportAttribute
    {
        private readonly string name;

        public DocumentTemplateMetadataAttribute(string name)
            : base(typeof(IDocumentTemplateMetadata))
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}
