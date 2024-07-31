using Betakads.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betakads.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<IOpenAIService, OpenAIService>();
        collection.AddTransient<IPdfService, PdfService>();
        collection.AddTransient<IYoutubeService, YoutubeService>();
        collection.AddTransient<MainViewModel>();
    }
}
