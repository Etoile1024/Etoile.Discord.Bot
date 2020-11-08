using static Etoile.Discord.Bot.Holders.AudioHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavalink4NET.Player;
using Discord;

namespace Etoile.Discord.Bot.Cores
{
    public static class AudioManager
    {
        public static async void PlayerControl(ulong guildid, ITextChannel text)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            var player = audioService.GetPlayer<LavalinkPlayer>(guildid);
            while (SongList.Count > 0)
            {
                if (player.State == PlayerState.NotPlaying)
                {
                    var track = SongList.FirstOrDefault();
                    await player.PlayAsync(track);
                    embedBuilder.WithTitle("エトワール").WithDescription(string.Format("依家開始播緊{0} [{1}]！", track.Title, track.Duration));
                    await text.SendMessageAsync("", false, embedBuilder.Build());
                    SongList.Remove(track);
                }
                await Task.Delay(100);
            }
        }
    }
}
