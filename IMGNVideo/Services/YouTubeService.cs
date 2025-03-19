using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IMGNVideo.Models;

namespace IMGNVideo.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public YouTubeService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["YouTubeApi:ApiKey"];
            _baseUrl = configuration["YouTubeApi:BaseUrl"];
        }

        // Updated method signature to include uploadDate, duration, and sortBy parameters.
        public async Task<IEnumerable<YouTubeVideo>> SearchVideosAsync(string query, string uploadDate, string type, string duration, string sortBy)
        {
            // 1) Call /search endpoint to get snippet data
            string searchUrl = $"{_baseUrl}/search?part=snippet&type=video&q={Uri.EscapeDataString(query)}&maxResults=50&key={_apiKey}";
            var searchResponse = await _httpClient.GetAsync(searchUrl);
            if (!searchResponse.IsSuccessStatusCode)
            {
                // Optionally log the error
                return new List<YouTubeVideo>();
            }

            var searchJson = await searchResponse.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<YouTubeSearchResponse>(
                searchJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            // Build initial list from search results
            var videos = new List<YouTubeVideo>();
            if (searchResult?.Items != null)
            {
                foreach (var item in searchResult.Items)
                {
                    // Make sure we're dealing with "video" results
                    if (item.Id.Kind == "youtube#video")
                    {
                        videos.Add(new YouTubeVideo
                        {
                            VideoId = item.Id.VideoId,
                            Title = item.Snippet.Title,
                            Description = item.Snippet.Description,
                            ChannelName = item.Snippet.ChannelTitle,
                            PublishedAt = item.Snippet.PublishedAt,
                            ThumbnailUrl = item.Snippet.Thumbnails?.High?.Url,
                            ViewCount = 0 // Will update after the second call
                        });
                    }
                }
            }

            // 2) Call /videos endpoint to get statistics (viewCount, etc.)
            if (videos.Any())
            {
                // Build a comma-separated list of video IDs
                string videoIds = string.Join(",", videos.Select(v => v.VideoId));
                string videosUrl = $"{_baseUrl}/videos?part=statistics&id={videoIds}&key={_apiKey}";
                var videosResponse = await _httpClient.GetAsync(videosUrl);
                if (videosResponse.IsSuccessStatusCode)
                {
                    var videosJson = await videosResponse.Content.ReadAsStringAsync();
                    var detailResult = JsonSerializer.Deserialize<YouTubeVideosDetailResponse>(
                        videosJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    // Match statistics back to each video
                    if (detailResult?.Items != null)
                    {
                        foreach (var detailItem in detailResult.Items)
                        {
                            var match = videos.FirstOrDefault(v => v.VideoId == detailItem.Id);
                            if (match != null && detailItem.Statistics != null)
                            {
                                match.ViewCount = Convert.ToInt64(detailItem.Statistics.ViewCount);
                            }
                        }
                    }
                }
            }

            // 3) Apply optional sorting
            switch (sortBy?.ToLower())
            {
                case "latest":
                    videos = videos.OrderByDescending(v => v.PublishedAt).ToList();
                    break;
                case "oldest":
                    videos = videos.OrderBy(v => v.PublishedAt).ToList();
                    break;
                case "most-viewed":
                    videos = videos.OrderByDescending(v => v.ViewCount).ToList();
                    break;
                default:
                    // No sorting applied if sortBy is not recognized.
                    break;
            }

            return videos;
        }


        public async Task<IEnumerable<YouTubeVideo>> GetRandomVideosAsync()
        {
            // Use the mostPopular chart endpoint.
            // You can set regionCode (e.g., US) if desired.
            string videosUrl = $"{_baseUrl}/videos?part=snippet,statistics&chart=mostPopular&maxResults=50&regionCode=US&key={_apiKey}";
            var response = await _httpClient.GetAsync(videosUrl);
            if (!response.IsSuccessStatusCode)
            {
                return new List<YouTubeVideo>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            // Deserialize using a model that matches the /videos endpoint response.
            var detailResult = JsonSerializer.Deserialize<YouTubeVideosDetailResponse>(
                jsonString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var videos = new List<YouTubeVideo>();
            if (detailResult?.Items != null)
            {
                foreach (var item in detailResult.Items)
                {
                    videos.Add(new YouTubeVideo
                    {
                        VideoId = item.Id,
                        Title = item.Snippet.Title,
                        Description = item.Snippet.Description,
                        ChannelName = item.Snippet.ChannelTitle,
                        PublishedAt = item.Snippet.PublishedAt,
                        ThumbnailUrl = item.Snippet.Thumbnails?.High?.Url,
                        ViewCount = item.Statistics != null ? Convert.ToInt64(item.Statistics.ViewCount) : 0
                    });
                }
            }

            // Randomly shuffle
            var randomVideos = videos.OrderBy(v => Guid.NewGuid()).Take(50).ToList();
            return randomVideos;
        }

        public async Task<List<YouTubeVideo>> GetTrendingVideosAsync()
        {
            string url = $"{_baseUrl}/videos?part=snippet,statistics&chart=mostPopular&maxResults=50&regionCode=US&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<YouTubeVideo>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<YouTubeVideoResponse>(
                jsonString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var videos = apiResponse?.Items?.Select(item => new YouTubeVideo
            {
                VideoId = item.Id,
                Title = item.Snippet.Title,
                Description = item.Snippet.Description,
                ChannelName = item.Snippet.ChannelTitle,
                PublishedAt = item.Snippet.PublishedAt,
                ThumbnailUrl = item.Snippet.Thumbnails?.High?.Url,
                ViewCount = item.Statistics != null ? Convert.ToInt64(item.Statistics.ViewCount) : 0
            }).ToList() ?? new List<YouTubeVideo>();

            return videos;
        }

        //public async Task<List<YouTubeVideo>> GetVideosAsync()
        //{
        //    // 1) Build your request URL
        //    string url = $"{_baseUrl}/search?part=snippet&maxResults=50&type=video&q=cats&key={_apiKey}";

        //    // 2) Call the YouTube API
        //    var response = await _httpClient.GetAsync(url);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        // If the API fails, return an empty list
        //        return new List<YouTubeVideo>();
        //    }

        //    // 3) Deserialize the JSON
        //    var jsonString = await response.Content.ReadAsStringAsync();
        //    var searchResponse = JsonSerializer.Deserialize<YouTubeSearchResponse>(
        //        jsonString,
        //        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        //    );

        //    // 4) Map each item to your YouTubeVideo model
        //    var videos = new List<YouTubeVideo>();
        //    if (searchResponse?.Items != null)
        //    {
        //        foreach (var item in searchResponse.Items)
        //        {
        //            // Make sure item.Id.VideoId is valid (type=video)
        //            if (item.Id.Kind == "youtube#video")
        //            {
        //                videos.Add(new YouTubeVideo
        //                {
        //                    VideoId = item.Id.VideoId,
        //                    Title = item.Snippet.Title,
        //                    Description = item.Snippet.Description,
        //                    ChannelName = item.Snippet.ChannelTitle,
        //                    PublishedAt = item.Snippet.PublishedAt,
        //                    ThumbnailUrl = item.Snippet.Thumbnails?.High?.Url,
        //                    ViewCount = 0 // If you want actual viewCount, call /videos?part=statistics
        //                });
        //            }
        //        }
        //    }
        //    return videos;
        //}

    }
}
