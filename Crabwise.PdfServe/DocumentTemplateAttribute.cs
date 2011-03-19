namespace Crabwise.PdfServe
{
    using System;
    using System.ComponentModel.Composition;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DocumentTemplateAttribute : ExportAttribute
    {
        private readonly string name;

        public DocumentTemplateAttribute(string name)
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
