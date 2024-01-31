namespace Betakads.Services.Interfaces
{
    public interface IPdfService
    {
        Task<string> ExtractTxtFromPdf(string pdfFilePath);
    }
}
