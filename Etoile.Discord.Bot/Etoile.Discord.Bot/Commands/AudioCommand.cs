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
                embedBuilder.WithTitle("エラー").WithDescription("屌你都未book房，我點入黎同你hehe啊？").WithColor(Color.Red);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，你就趕我走，你有撚病啊？").WithColor(Color.Red);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = await audioService.GetTrackAsync(keyword, SearchMode.YouTube);
            if (track == null)
            {
                embedBuilder.WithTitle("エラー").WithDescription("すみません、搵唔到你想佢唱嘅歌wor").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State != PlayerState.Playing)
            {
                await player.PlayAsync(track);

                embedBuilder.WithTitle("エトワール").WithDescription(string.Format("依家開始唱緊\"{0}\" [{1}]！", track.Title, track.Duration)).WithColor(Color.Blue);
                await ReplyAsync("", false, embedBuilder.Build());
            }
            else
            {
                SongList.Add(track);
                ITextChannel text = Context.Channel as ITextChannel;
                if (SongList.Count == 1) //Player control thread cannot duplicate run
                    await Task.Run(() => AudioManager.PlayerControl(guild.Id, text));

                embedBuilder.WithTitle("エトワール").WithDescription(string.Format("目前加左\"{0}\"入個list到，排到你隊果陣時就會唱！", track.Title)).WithColor(Color.Blue);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.Paused)
            {
                embedBuilder.WithTitle("エラー").WithDescription("首歌咪停左囉！你做乜鳩姐").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle("エラー").WithDescription("你都冇叫個BOT唱歌，你停乜鳩姐").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            await player.PauseAsync();

            embedBuilder.WithTitle("エトワール").WithDescription("已經停左你想叫我唱嘅歌，你滿意啦！").WithColor(Color.Blue);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle("エラー").WithDescription("你都冇叫個BOT唱歌，你停乜鳩姐").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State != PlayerState.Paused)
            {
                embedBuilder.WithTitle("エラー").WithDescription("首歌都冇停，你做乜鳩姐").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = player.CurrentTrack;
            await player.ResumeAsync();

            embedBuilder.WithTitle("エトワール").WithDescription(string.Format("依家開始播翻\"{0}\"", track.Title)).WithColor(Color.Blue);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle("エラー").WithDescription("個BOT都冇唱歌，你叫佢查乜鳩姐").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = player.CurrentTrack;
            embedBuilder.WithTitle("エトワール").WithDescription(string.Format("依家個BOT唱緊\"{0}\" [{1}/{2}]", track.Title, player.TrackPosition.ToString("hh\\:mm\\:ss"), track.Duration)).WithColor(Color.Blue);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle("エラー").WithDescription("個BOT都冇唱歌，點quit左佢啊 大佬").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            var track = player.CurrentTrack;
            await player.StopAsync();

            embedBuilder.WithTitle("エトワール").WithDescription(string.Format("依家咪同你quit左\"{0}\"佢囉！", track.Title)).WithColor(Color.Blue);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            SongList.Clear();

            embedBuilder.WithTitle("エトワール").WithDescription("已經清曬list裏面d歌了。").WithColor(Color.Blue);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (player.State == PlayerState.NotPlaying)
            {
                embedBuilder.WithTitle("エラー").WithDescription("個BOT都冇唱歌，點較佢有幾嘈啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            if (value < 1 && value > 1000)
            {
                embedBuilder.WithTitle("エラー").WithDescription("個數值可能太大或者太細，自己再試下啦！").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            await player.SetVolumeAsync(value / 100f);

            embedBuilder.WithTitle("エトワール").WithDescription(string.Format("已經將佢把口嘅音量set到{0}%", value)).WithColor(Color.Blue);
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
                embedBuilder.WithTitle("エラー").WithDescription("你都冇同我開房，我點同你做野啊？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            string context = string.Empty;
            var track = player.CurrentTrack;
            if (track != null)
                context += string.Format("**依家唱緊嘅係：{0} [{1}/{2}]**\r\n", track.Title, player.TrackPosition.ToString("hh\\:mm\\:ss"), track.Duration);
            else
            {
                embedBuilder.WithTitle("エラー").WithDescription("個BOT都冇歌唱緊，有閪野list咩？").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            int count = SongList.Count;
            if (count > 0)
            {
                for (int i = 0; i < (count > 15 ? 15 : count); i++)
                {
                    var songtrack = SongList[i];
                    context += string.Format("{0} [{1}]\r\n", songtrack.Title, songtrack.Duration);
                }
                if (count > 15)
                    context += "仲有其他歌... ...";
            }
            embedBuilder.WithTitle("エトワール").AddField("列表", context).WithColor(Color.Blue);
            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}
