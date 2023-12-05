using Betakads.Services.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Betakads.Services;

public class PdfService : IPdfService
{
    private const int MaximumWords = 1000;
    public string ExtractTxtFromPdf(string pdfFilePath)
    {
        StringBuilder extractedText = new();
        int numberOfWords = 0;

        using PdfDocument document = PdfDocument.Open(pdfFilePath);
        foreach (Page page in document.GetPages())
        {
            extractedText.Append(page.Text);
            numberOfWords++;

            if (numberOfWords >= MaximumWords) break;
        }

        return extractedText.ToString();
    }
}
