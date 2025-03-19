document.addEventListener("DOMContentLoaded", function () {
    const showMoreBtn = document.getElementById("show-more-btn");
    const videoContainer = document.getElementById("video-container");

    let currentIndex = 0;
    const videosToShow = 6; // Load 6 videos per click

    if (showMoreBtn && remainingVideos && remainingVideos.length) {
        showMoreBtn.addEventListener("click", function () {
            // Load next set of videos
            for (let i = 0; i < videosToShow && currentIndex < remainingVideos.length; i++, currentIndex++) {
                const video = remainingVideos[currentIndex];
                const videoElement = `
                     <div class="col video-item">
                        <div class="card h-100 d-flex flex-column">
                            <img src="${video.thumbnailUrl}" class="card-img-top" alt="${video.title}" />
                            <div class="card-body d-flex flex-column">
                                <h5 class="card-title">${video.title}</h5>
                                <small style="color:gray">${video.channelName}</small>
                                <div class="d-flex mb-2">
                                    <small class="text-end" style="color:gray">
                                        ${formatViewCount(video.viewCount)}views
                                    </small>
                                    &nbsp;
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="#808080" class="bi bi-dot mt-1" viewBox="0 0 16 16">
                                        <path d="M8 9.5a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3" />
                                    </svg>
                                    <small style="color:gray">${getRelativeTime(video.publishedAt)}</small>
                                </div>
                                <p class="card-text flex-grow-1" style="color:gray">${limitDescription(video.description, 10)}</p>
                                <a href="https://www.youtube.com/watch?v=${video.videoId}"
                                   target="_blank"
                                   class="btn btn-primary btn-sm mt-auto">
                                    Watch Video
                                </a>
                            </div>
                        </div>
                    </div>

                `;
                videoContainer.insertAdjacentHTML("beforeend", videoElement);
            }

            // Hide the button if no more videos
            if (currentIndex >= remainingVideos.length) {
                showMoreBtn.style.display = "none";
            }
        });
    }

    // Helper functions for the dynamically added videos (similar to your C# helpers)
    function getRelativeTime(dateString) {
        const publishedDate = new Date(dateString);
        const timeSpan = Date.now() - publishedDate;
        const seconds = Math.floor(timeSpan / 1000);
        const minutes = Math.floor(seconds / 60);
        const hours = Math.floor(minutes / 60);
        const days = Math.floor(hours / 24);
        const months = Math.floor(days / 30);
        const years = Math.floor(days / 365);

        if (years > 0) return `${years} year${years > 1 ? "s" : ""} ago`;
        if (months > 0) return `${months} month${months > 1 ? "s" : ""} ago`;
        if (days > 0) return `${days} day${days > 1 ? "s" : ""} ago`;
        if (hours > 0) return `${hours} hour${hours > 1 ? "s" : ""} ago`;
        if (minutes > 0) return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
        return "just now";
    }

    function limitDescription(description, wordLimit) {
        if (!description) return "";
        const words = description.split(" ");
        return words.length > wordLimit ? words.slice(0, wordLimit).join(" ") + "..." : description;
    }

    function formatViewCount(viewCount) {
        if (viewCount < 1000) {
            return viewCount.toString();
        } else if (viewCount < 1000000) {
            let thousands = viewCount / 1000;
            // Remove trailing .0 if present
            return thousands.toFixed(1).replace(/\.0$/, '') + "K";
        } else if (viewCount < 1000000000) {
            let millions = viewCount / 1000000;
            return millions.toFixed(1).replace(/\.0$/, '') + "M";
        } else {
            let billions = viewCount / 1000000000;
            return billions.toFixed(1).replace(/\.0$/, '') + "B";
        }
    }

});
//@functions {
//    public string GetRelativeTime(DateTime publishedDate)
//    {
//        var timeSpan = DateTime.Now - publishedDate;
//        if (timeSpan.TotalDays >= 365)
//            return $"{(int)(timeSpan.TotalDays / 365)} year{(timeSpan.TotalDays / 365 >= 2 ? "s" : "")} ago";
//        if (timeSpan.TotalDays >= 30)
//            return $"{(int)(timeSpan.TotalDays / 30)} month{(timeSpan.TotalDays / 30 >= 2 ? "s" : "")} ago";
//        if (timeSpan.TotalDays >= 1)
//            return $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";
//        if (timeSpan.TotalHours >= 1)
//            return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";
//        if (timeSpan.TotalMinutes >= 1)
//            return $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago";
//        return "just now";
//    }

//    // Limits the description to a specified number of words (default is 20)
//    public string GetLimitedDescription(string description, int wordLimit = 20)
//    {
//        if (string.IsNullOrWhiteSpace(description))
//            return "";
//        var words = description.Split(' ');
//        if (words.Length <= wordLimit)
//            return description;
//        return string.Join(" ", words.Take(wordLimit)) + "...";
//    }
//}