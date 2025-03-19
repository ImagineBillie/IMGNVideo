using Microsoft.AspNetCore.Mvc;
using IMGNVideo.Services;
using IMGNVideo.Models;
using System.Threading.Tasks;

namespace IMGNVideo.Controllers
{
    public class TrendingController : Controller
    {
        private readonly IYouTubeService _youTubeService;

        public TrendingController(IYouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
        }

        public async Task<IActionResult> Index()
        {
            // Get all trending videos (or a fixed subset if needed)
            var trendingVideos = await _youTubeService.GetTrendingVideosAsync();

            // If you want to limit the number, you could do:
            // trendingVideos = trendingVideos.Take(50).ToList();

            // Use your custom view located at Views/Home/Trending.cshtml
            return View("~/Views/Home/Trending.cshtml", trendingVideos);
        }
    }
}
