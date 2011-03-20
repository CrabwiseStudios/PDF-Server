namespace Crabwise.PdfServe
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    public interface IDocumentTemplate
    {
        Document CreateDocument();
        void WriteDocument(Document document, dynamic templateData);
    }
}
