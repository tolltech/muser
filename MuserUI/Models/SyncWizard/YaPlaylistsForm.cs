using System;
using Tolltech.MuserUI.Models.Sync;

namespace Tolltech.MuserUI.Models.SyncWizard
{
    public class YaPlaylistsForm
    {
        public YaPlaylistsForm()
        {
            Playlists = Array.Empty<YaPlaylist>();
        }

        public YaPlaylist[] Playlists { get; set; }
        public string SelectedPlaylistId { get; set; }
        public string Login { get; set; }
    }}