namespace IMGNVideo.Models
{
    public class VideoPaginationViewModel
    {
        public IEnumerable<YouTubeVideo> Videos { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
