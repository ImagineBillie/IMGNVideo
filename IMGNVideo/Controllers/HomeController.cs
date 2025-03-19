using Microsoft.AspNetCore.Mvc;
using IMGNVideo.Services;
using IMGNVideo.Models;
using System.Linq;
using System.Threading.Tasks;

namespace IMGNVideo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IYouTubeService _youTubeService;

        public HomeController(IYouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
        }

        public async Task<IActionResult> Index()
        {
            // Get random popular videos
            var videos = await _youTubeService.GetRandomVideosAsync();

            return View(videos);
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
