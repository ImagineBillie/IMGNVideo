using System.Collections.Generic;
using System.Threading.Tasks;
using IMGNVideo.Models;

namespace IMGNVideo.Services
{
    public interface IYouTubeService
    {
        Task<IEnumerable<YouTubeVideo>> SearchVideosAsync(string query, string uploadDate, string type, string duration, string sortBy);
        Task<IEnumerable<YouTubeVideo>> GetRandomVideosAsync();

        //Task<List<YouTubeVideo>> GetVideosAsync();
        Task<List<YouTubeVideo>> GetTrendingVideosAsync();

    }



}
