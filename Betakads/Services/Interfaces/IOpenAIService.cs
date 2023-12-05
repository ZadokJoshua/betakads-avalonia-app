using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betakads.Services.Interfaces;

interface IOpenAIService
{
    Task<string> ConvertTextToCardsList(string promptPayload, int numberOfCards);
}
