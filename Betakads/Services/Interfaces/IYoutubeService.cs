namespace Betakads.Services.Interfaces
{
    public interface IYoutubeService
    {
        Task<String> GetVideoCaptions(string videoUrl);

        Task<YoutubeMetadata> GetVideoMetadata(string videoUrl);
    }
}
