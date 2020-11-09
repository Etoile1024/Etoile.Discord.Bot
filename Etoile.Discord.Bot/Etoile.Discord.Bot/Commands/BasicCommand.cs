using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etoile.Discord.Bot.Commands
{
    public class BasicCommand : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("指令說明書")
                .AddField("?help", "睇下有咩指令")
                .AddField("?join", "入你間房")
                .AddField("?leave", "唔中意咪quit左佢囉")
                .AddField("?play 關鍵字", "唱歌比你聽")
                .AddField("?pause", "停左唱緊嘅歌")
                .AddField("?resume", "叫佢唱翻停左嘅歌")
                .AddField("?np", "睇下唱緊咩歌")
                .AddField("?list", "列下有咩歌會唱")
                .AddField("?skip", "quit左首歌佢囉！")
                .AddField("?volume 數值", "set佢有幾嘈");

            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}
