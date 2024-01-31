using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Betakads.Services;

public class PdfService : IPdfService
{
    private const int MaximumWords = 1000;
    public async Task<string> ExtractTxtFromPdf(string pdfFilePath)
    {
        StringBuilder extractedText = new();
        int numberOfWords = 0;

        await Task.Run(() =>
        {
            using PdfDocument document = PdfDocument.Open(pdfFilePath);
            foreach (Page page in document.GetPages())
            {
                extractedText.Append(page.Text);
                numberOfWords++;

                if (numberOfWords >= MaximumWords) break;
            }
        });

        return extractedText.ToString();
    }
}
