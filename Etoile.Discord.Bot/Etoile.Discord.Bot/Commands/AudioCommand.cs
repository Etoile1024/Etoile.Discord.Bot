using Discord;
using Discord.Commands;
using static Etoile.Discord.Bot.Holders.AudioHolder;
using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavalink4NET.Rest;
using Etoile.Discord.Bot.Cores;
using Etoile.Discord.Bot.Structuring;
using Etoile.Discord.Bot.Holders;

namespace Etoile.Discord.Bot.Commands
{
    public class AudioCommand : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        public async Task JoinCmd() 
        {
            IGuild guild = Context.Guild;
            IVoiceChannel voice = (Context.User as IGuildUser).VoiceChannel;
            EmbedBuilder embedBuilder = new EmbedBuilder();
            if (voice == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("JOIN_NEED")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            await audioService.JoinAsync(guild.Id, voice.Id);
        }
        [Command("leave")]
        public async Task LeaveCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("LEAVE_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            await player.DisconnectAsync();
        }
        [Command("play")]
        public async Task PlayCmd([Remainder]string keyword)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = await audioService.GetTrackAsync(keyword, SearchMode.YouTube);
            if (track == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("NO_SONG_PLAY")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State != PlayerState.Playing)
            {
                await player.PlayAsync(track);

                embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("START_PLAYING", track.Title, track.Duration)).WithColor(Color.Blue);
                await ReplyAsync("", false, embedBuilder.Build());
            }
            else
            {
                SongList.Add(new SongTrack() { Guild = guild, Track = track });
                ITextChannel text = Context.Channel as ITextChannel;
                if (SongList.Count(c => c.Guild.Id == guild.Id) == 1) //Player control thread cannot duplicate run
                    await Task.Run(() => AudioManager.PlayerControl(guild.Id, text));

                embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("SONG_QUEUE_UP", track.Title)).WithColor(Color.Blue);
                await ReplyAsync("", false, embedBuilder.Build());
            }
        }
        [Command("pause")]
        public async Task PauseCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.Paused)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("PAUSE_ALREADY")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("NOT_PLAYING")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            await player.PauseAsync();

            embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("PAUSED")).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("resume")]
        public async Task ResumeCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("NOT_PLAYING")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State != PlayerState.Paused)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("NOT_PAUSED")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = player.CurrentTrack;
            await player.ResumeAsync();

            embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("RESUMED", track.Title)).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("np")]
        public async Task NowPlayingCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("NOT_PLAYING")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = player.CurrentTrack;
            embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("NOW_PLAYING_MSG", track.Title, player.TrackPosition.ToString("hh\\:mm\\:ss"), track.Duration)).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("skip")]
        public async Task SkipCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("NOT_PLAYING")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = player.CurrentTrack;
            await player.StopAsync();

            embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("SKIP_MSG", track.Title)).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("clear")]
        public async Task ClearCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            SongList.RemoveAll(a => a.Guild.Id == guild.Id);

            embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("CLEAR_MSG")).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("volume")]
        public async Task VolumeCmd(int value)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("NOT_PLAYING")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (value < 1 && value > 1000)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("VOLUME_INVALID_VALUE")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            await player.SetVolumeAsync(value / 100f);

            embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).WithDescription(LanguageHolder.GetTranslation("VOLUME_MSG", value)).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("list")]
        public async Task ListCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            IGuild guild = Context.Guild;
            var player = audioService.GetPlayer<LavalinkPlayer>(guild.Id);
            if (player == null)
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("AUDIO_CMD_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            string context = string.Empty;
            var track = player.CurrentTrack;
            if (track != null)
                context += LanguageHolder.GetTranslation("LIST_NOW_PLAYING", track.Title, player.TrackPosition.ToString("hh\\:mm\\:ss"), track.Duration) + Environment.NewLine;
            else
            {
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("LIST_NULL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            int count = SongList.Count(c => c.Guild.Id == guild.Id);
            if (count > 0)
            {
                for (int i = 0; i < (count > 15 ? 15 : count); i++)
                {
                    var songtrack = SongList.Where(w => w.Guild.Id == guild.Id).ToArray()[i];
                    context += string.Format("{0} [{1}]\r\n", songtrack.Track.Title, songtrack.Track.Duration);
                }
                if (count > 15)
                    context += LanguageHolder.GetTranslation("LIST_MORE");
            }
            embedBuilder.WithTitle(LanguageHolder.GetTranslation("BOT_TITLE")).AddField(LanguageHolder.GetTranslation("LIST_TITLE"), context).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}
