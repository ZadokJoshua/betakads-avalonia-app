using YoutubeExplode;
using YoutubeExplode.Common;

namespace Betakads.Services
{
    public class YoutubeService : IYoutubeService
    {
        private readonly YoutubeClient _youtubeClient;
        public YoutubeService()
        {
            _youtubeClient = new();
        }

        public async Task<string> GetVideoCaptions(string videoUrl)
        {
            StringBuilder videoSubtitleText = new();

            var trackManifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoUrl);

            var trackInfo = trackManifest.GetByLanguage("en");
            var track = await _youtubeClient.Videos.ClosedCaptions.GetAsync(trackInfo);

            foreach (var caption in track.Captions)
            {
                videoSubtitleText.Append(caption);
            }

            return videoSubtitleText.ToString();
        }

        public async Task<YoutubeMetadata> GetVideoMetadata(string videoUrl)
        {
            var video = await _youtubeClient.Videos.GetAsync(videoUrl);
            return new YoutubeMetadata(video.Title, video.Author.ChannelTitle, video.Duration);
        }
    }
}
