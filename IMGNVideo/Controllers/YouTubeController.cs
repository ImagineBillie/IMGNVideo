using Microsoft.AspNetCore.Mvc;
using IMGNVideo.Services;
using System.Threading.Tasks;
using IMGNVideo.Models;

namespace IMGNVideo.Controllers
{
    public class YouTubeController : Controller
    {
        private readonly IYouTubeService _youTubeService;

        public YouTubeController(IYouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
        }

        public IActionResult Search()
        {
            return View();
        }


        //public async Task<IActionResult> Results(string query, string uploadDate, string duration, string sortBy)
        public async Task<IActionResult> Results(string query, string uploadDate, string type, string duration, string sortBy)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.Error = "Please enter a search term.";
                return View("Results", new List<YouTubeVideo>());
            }

            var videos = await _youTubeService.SearchVideosAsync(query, uploadDate, type, duration, sortBy);
            return View(videos);
        }

    }
}
