using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betakads.Services.Interfaces
{
    public interface IPdfService
    {
        string ExtractTxtFromPdf(string pdfFilePath);
    }
}
