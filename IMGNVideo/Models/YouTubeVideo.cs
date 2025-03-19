using System;

namespace IMGNVideo.Models
{
    public class YouTubeVideo
    {
        public string? Snippet { get; set; }
        public string? VideoId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ChannelName { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long ViewCount { get; set; }
    }

}
