using Discord;
using Discord.Commands;
using Serilog;
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
                .AddField("?volume 數值", "set佢有幾嘈")
                .AddField("?dice 擲骰子次數d骰子最大值", "擲骰子");

            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("dice")]
        public async Task DiceCmd(string request)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            try
            {
                request = request.ToLower();
                int count = Convert.ToInt32(request.Split('d')[0]);
                int max = Convert.ToInt32(request.Split('d')[1]);
                if (count < 1)
                {
                    embedBuilder.WithTitle("エラー").WithDescription("擲骰次數一定要係正整數。").WithColor(Color.Red);
                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
                if (max < 2)
                {
                    embedBuilder.WithTitle("エラー").WithDescription("骰子最大值一定要係2或者以上。").WithColor(Color.Red);
                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
                if (count > 500)
                {
                    embedBuilder.WithTitle("エラー").WithDescription("最多只可以擲500次骰。").WithColor(Color.Red);
                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
                if (max > 100)
                {
                    embedBuilder.WithTitle("エラー").WithDescription("骰子最大值最多只可以係100。").WithColor(Color.Red);
                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
                string diceArray = string.Empty;
                int sum = 0;
                for (int i = 1; i <= count; i++)
                {
                    int result = new Random(Guid.NewGuid().GetHashCode()).Next(1, max + 1);
                    sum += result;
                    diceArray = diceArray == string.Empty ? result.ToString() : string.Format("{0}, {1}", diceArray, result);
                }
                string message = string.Format("過程：`[{0}]` 結果：{1}", diceArray, sum);
                await ReplyAsync(message);
            }
            catch (Exception ex)
            {
                Log.Error("Error on dice command:\r\n{0}", ex);
                embedBuilder.WithTitle("エラー").WithDescription("擲骰子失敗！請檢查你所輸入嘅指令係咪啱！").WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
            }
        }
    }
}
