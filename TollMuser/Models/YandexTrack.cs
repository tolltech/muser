using System;

namespace Tolltech.Muser.Models
{
    public class YandexTrack
    {
        public YandexTrack()
        {
            Artists = Array.Empty<string>();
        }

        public string[] Artists { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public string AlbumId { get; set; }
    }
}