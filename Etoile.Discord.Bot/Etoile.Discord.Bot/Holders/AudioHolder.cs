using Etoile.Discord.Bot.Structuring;
using Lavalink4NET;
using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etoile.Discord.Bot.Holders
{
    public static class AudioHolder
    {
        public static LavalinkNode audioService;

        public static List<SongTrack> SongList = new List<SongTrack>();
    }
}
