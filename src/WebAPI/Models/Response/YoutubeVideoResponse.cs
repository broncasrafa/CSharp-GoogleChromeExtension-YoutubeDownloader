using System.Collections.Generic;
using Yt.Entity.Auxs;

namespace WebAPI.Models.Response
{
    public class YoutubeVideoResponse
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public IList<Thumbnail> Thumbnails { get; set; }
        public IList<VideoFormat> VideoFormats { get; set; }
    }
}
