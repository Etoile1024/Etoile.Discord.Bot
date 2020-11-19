using static Etoile.Discord.Bot.Holders.AudioHolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavalink4NET.Player;
using Discord;
using Etoile.Discord.Bot.Holders;

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
                    var song = SongList.FirstOrDefault(f => f.Guild.Id == guildid);
                    await player.PlayAsync(song.Track);
                    embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("START_PLAYING", song.Track.Title, song.Track.Duration)).WithColor(Color.Blue);
                    await text.SendMessageAsync("", false, embedBuilder.Build());
                    SongList.Remove(song);
                }
                await Task.Delay(100);
            }
        }
    }
}
