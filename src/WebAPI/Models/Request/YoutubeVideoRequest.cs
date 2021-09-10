namespace WebAPI.Models.Request
{
    /// <summary>
    /// objeto de parametro na requisição de download de video
    /// </summary>
    public class YoutubeVideoRequest
    {
        /// <summary>
        /// id do video ou playlist
        /// </summary>
        public string IdResource { get; set; }

        /// <summary>
        /// informa se o id do recurso é de uma playlist ou um video
        /// </summary>
        public bool IsPlaylist { get; set; }
    }
}
