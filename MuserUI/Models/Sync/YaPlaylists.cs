using System;

namespace Tolltech.MuserUI.Models.Sync
{
    public class YaPlaylists
    {
        public YaPlaylists()
        {
            Playlists = Array.Empty<YaPlaylist>();
        }

        public YaPlaylist[] Playlists { get; set; }
    }
}