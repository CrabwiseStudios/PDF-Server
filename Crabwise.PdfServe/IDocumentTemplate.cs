namespace Crabwise.PdfServe
{
    using iTextSharp.text;

    public interface IDocumentTemplate
    {
        Document CreateDocument(dynamic templateData);
    }
}
