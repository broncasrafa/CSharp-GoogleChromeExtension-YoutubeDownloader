using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Response;
using Yt;
using Yt.Entity;

namespace WebAPI.Services
{
    public class VideoService
    {
        private readonly YoutubeProvider _YoutubeProvider;

        public VideoService()
        {
            _YoutubeProvider = new YoutubeProvider();
        }




        private void SetMp3DataFormat(YoutubeVideoResponse videoResponse)
        {
            string videoUrl = videoResponse.VideoFormats.Where(c => c.QualityLabel == "720p")?.FirstOrDefault().Url;

            if (!string.IsNullOrWhiteSpace(videoUrl))
            {
                string base64StringMp3 = Mp3Service.ConvertMp4VideoToMp3(videoUrl, videoResponse.Title);

                if (!string.IsNullOrWhiteSpace(base64StringMp3))
                {
                    videoResponse.VideoFormats.Add(new Yt.Entity.Auxs.VideoFormat
                    {
                        Url = $"data:audio/mp3;base64,{base64StringMp3}",
                        QualityLabel = "mp3"
                    });
                }
            }            
        }



        public async Task<List<YoutubeVideoResponse>> GetVideoData(string videoId)
        {
            List<YoutubeVideoResponse> videos = new List<YoutubeVideoResponse>();           

            var video = await _YoutubeProvider.Videos.GetVideoAsync(videoId);
            if (video.VideoId == null)
                return null;

            videos.Add(new YoutubeVideoResponse
            {
                VideoId = video.VideoId,
                Title = video.Title,
                Category = video.Category,
                Thumbnails = video.Thumbnails,
                VideoFormats = video.StreamingVideoData.Formats
            });            

            string videoUrl = video.StreamingVideoData.Formats.Where(c => c.QualityLabel == "720p")?.FirstOrDefault().Url;

            if (string.IsNullOrWhiteSpace(videoUrl))
                return null;

            //SetMp3DataFormat(videos.FirstOrDefault());

            return videos;
        }

        public async Task<List<YoutubeVideoResponse>> GetPlaylistVideosData(string playlistId )
        {
            List<YoutubeVideoResponse> videos = new List<YoutubeVideoResponse>();

            YoutubePlaylist playlist = await _YoutubeProvider.Playlists.GetPlaylistAsync(playlistId);
            if (playlist.PlaylistId == null)
                return null;

            foreach (var item in playlist.Contents)
            {
                var videoInfo = await _YoutubeProvider.Videos.GetVideoAsync(item.VideoId);

                YoutubeVideoResponse videoResponse = new YoutubeVideoResponse
                {
                    VideoId = item.VideoId,
                    Title = item.Title,
                    Thumbnails = item.Thumbnails,
                    Category = videoInfo.Category,
                    VideoFormats = videoInfo.StreamingVideoData.Formats
                };

                //SetMp3DataFormat(videoResponse);
                
                videos.Add(videoResponse);
            }

            return videos;
        }

        
    }
}
