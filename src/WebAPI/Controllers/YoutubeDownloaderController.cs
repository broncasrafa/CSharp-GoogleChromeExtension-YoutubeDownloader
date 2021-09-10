using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Models.Request;
using WebAPI.Models.Response;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/v1/yt")]
    [ApiController]
    public class YoutubeDownloaderController : ControllerBase
    {
        private readonly ILogger<YoutubeDownloaderController> _logger;
        private readonly VideoService _videoService;

        public YoutubeDownloaderController(ILogger<YoutubeDownloaderController> logger)
        {
            _logger = logger;
            _videoService = new VideoService();
        }

        /// <summary>
        /// obter as informações para download do video ou os videos da playlist
        /// </summary>
        /// <remarks>
        /// exemplo da requisição:
        ///
        ///     POST /YoutubeVideoRequest
        ///     {
        ///        "idResource": "Y89HH-5gXn8",
        ///        "isPlaylist": false
        ///     }
        ///
        /// </remarks>
        /// <param name="model">objeto de parametro da requisição informando o id do recurso (video ou playlist) e se é uma playlist. default false</param>
        /// <returns>as informações do video ou os videos da playlist</returns>
        [HttpPost]
        [Route("download")]
        public async Task<IActionResult> Download([FromBody]YoutubeVideoRequest model)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(model.IdResource))
                    return BadRequest(new { status = "error", message = "O link do video é obrigatório." });

                List<YoutubeVideoResponse> videos = new List<YoutubeVideoResponse>();

                if (model.IsPlaylist)
                {
                    string playlistId = model.IdResource;                    

                    videos = await _videoService.GetPlaylistVideosData(playlistId);

                    if (videos == null)
                        return BadRequest(new { status = "error", message = "Playlist unavailable" });
                }
                else
                {
                    string videoId = model.IdResource;

                    videos = await _videoService.GetVideoData(videoId);

                    if (videos == null)
                        return BadRequest(new { status = "error", message = "Video unavailable" });
                }

                return Ok(new { status = "ok", data = videos });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = "Ocorreu um erro ao tentar realizar a requisição." });
            }
        }


        /// <summary>
        /// obter a string base64 do video no formato mp3 para realizar o download
        /// </summary>
        /// <remarks>
        /// exemplo da requisição:
        ///
        ///     POST /YoutubeVideoRequest
        ///     {
        ///        "idResource": "Y89HH-5gXn8",
        ///        "isPlaylist": false
        ///     }
        ///
        /// </remarks>
        /// <param name="model">objeto de parametro da requisição informando o id do recurso (video). default false</param>
        /// <returns>a string base64 do video no formato mp3 para realizar o download</returns>
        [HttpPost]
        [Route("download/mp3")]
        public async Task<IActionResult> DownloadMp3([FromBody] YoutubeVideoRequest model)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(model.IdResource))
                    return BadRequest(new { status = "error", message = "O link do video é obrigatório." });

                string videoId = model.IdResource;

                var result = await _videoService.GetVideoData(videoId);

                if (result == null)
                    return BadRequest(new { status = "error", message = "Video unavailable" });

                string videoUrl = result.FirstOrDefault().VideoFormats.Where(c => c.QualityLabel == "720p")?.FirstOrDefault().Url;

                if (string.IsNullOrWhiteSpace(videoUrl))
                    return BadRequest(new { status = "error", message = "Recurso do vídeo não encontrado." });

                string base64String = Mp3Service.ConvertMp4VideoToMp3(videoUrl, result.FirstOrDefault().Title);

                return Ok(new { status = "ok", data = base64String });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = "Ocorreu um erro ao tentar realizar a requisição." });
            }
        }
    }
}
