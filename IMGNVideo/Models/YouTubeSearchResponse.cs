using System;
using System.Collections.Generic;

namespace IMGNVideo.Models
{
    public class YouTubeSearchResponse
    {
        public List<YouTubeSearchItem>? Items { get; set; }
    }

    public class YouTubeSearchItem
    {
        public required YouTubeItemId Id { get; set; }
        public YouTubeSnippet? Snippet { get; set; }
    }

    public class YouTubeItemId
    {
        public string? Kind { get; set; }
        public string? VideoId { get; set; } 
    }

    public class YouTubeSnippet
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ChannelTitle { get; set; }
        public DateTime PublishedAt { get; set; }
        public YouTubeThumbnails? Thumbnails { get; set; }
    }

    public class YouTubeThumbnails
    {
        public YouTubeThumbnail? High { get; set; }//default, medium or high
    }

    public class YouTubeThumbnail
    {
        public string? Url { get; set; }
    }

    public class YouTubeVideosDetailResponse
    {
        public List<YouTubeVideoDetailItem> Items { get; set; }
    }

    public class YouTubeVideoDetailItem
    {
        public string Id { get; set; }
        public YouTubeVideoStatistics? Statistics { get; set; }
        public YouTubeSnippet Snippet { get; set; }
    }

    public class YouTubeVideoStatistics
    {
        public string? ViewCount { get; set; }
        public string? LikeCount { get; set; }
        public string? DislikeCount { get; set; }
        public string? FavoriteCount { get; set; }
        public string? CommentCount { get; set; }
    }

    public class YouTubeVideoResponse
    {
        public List<YouTubeVideoDetailItem> Items { get; set; }
    }

}
