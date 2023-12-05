using Betakads.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betakads.Services.Interfaces
{
    public interface IYoutubeService
    {
        Task<String> GetVideoCaptions(string videoUrl);

        Task<YoutubeMetadata> GetVideoMetadata(string videoUrl);
    }
}
