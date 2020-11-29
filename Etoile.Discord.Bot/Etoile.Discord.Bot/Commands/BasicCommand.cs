using Discord;
using Discord.Commands;
using Etoile.Discord.Bot.Holders;
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
            embedBuilder.WithTitle(LanguageHolder.GetTranslation("HELP_TEXT"))
                .AddField("?help", LanguageHolder.GetTranslation("HELP_DESC"))
                .AddField("?join", LanguageHolder.GetTranslation("JOIN_DESC"))
                .AddField("?leave", LanguageHolder.GetTranslation("LEAVE_DESC"))
                .AddField("?play " + LanguageHolder.GetTranslation("KEYWORD_TEXT"), LanguageHolder.GetTranslation("PLAY_DESC"))
                .AddField("?pause", LanguageHolder.GetTranslation("PAUSE_DESC"))
                .AddField("?resume", LanguageHolder.GetTranslation("RESUME_DESC"))
                .AddField("?np", LanguageHolder.GetTranslation("NOW_PLAYING_DESC"))
                .AddField("?list", LanguageHolder.GetTranslation("LIST_DESC"))
                .AddField("?skip", LanguageHolder.GetTranslation("SKIP_DESC"))
                .AddField("?volume " + LanguageHolder.GetTranslation("VALUE_TEXT"), LanguageHolder.GetTranslation("VOLUME_DESC"))
                .AddField("?dice " + LanguageHolder.GetTranslation("DICE_PARMS_TEXT"), LanguageHolder.GetTranslation("DICE_DESC"))
                .AddField("?mod a n", LanguageHolder.GetTranslation("MOD_DESC"))
                .AddField("?floor " + LanguageHolder.GetTranslation("DECIMAL_TEXT"), LanguageHolder.GetTranslation("FLOOR_DESC"))
                .AddField("?ceiling " + LanguageHolder.GetTranslation("DECIMAL_TEXT"), LanguageHolder.GetTranslation("CEILING_DESC"));

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
                string success_img = "https://cdn.discordapp.com/attachments/773869047150149642/781911359347884052/0aae7fc675bb6018.png";//決定的成功
                string failure_img = "https://cdn.discordapp.com/attachments/773869047150149642/781911285196783646/b9e4cf4ff89d3a53.png";//致命的失敗
                if (count < 1)
                {
                    embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("DICE_MUST_POSITIVE_INT")).WithColor(Color.Red);
                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
                if (max < 2)
                {
                    embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("DICE_MORE_THAN_2")).WithColor(Color.Red);
                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
                if (count > 500)
                {
                    embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("DICE_LESS_THAN_500")).WithColor(Color.Red);
                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
                if (max > 100)
                {
                    embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("DICE_MAX_100")).WithColor(Color.Red);
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
                string message = LanguageHolder.GetTranslation("DICE_MSG", diceArray, sum);
                await ReplyAsync(message);
                if (count == 1 && max == 100) //1d100 checking
                {
                    if (sum >= 96 && sum <= 100)
                        await ReplyAsync(failure_img);
                    if (sum >= 1 && sum <= 5)
                        await ReplyAsync(success_img);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error on dice command:\r\n{0}", ex);
                embedBuilder.WithTitle(LanguageHolder.GetTranslation("ERROR")).WithDescription(LanguageHolder.GetTranslation("DICE_FAIL")).WithColor(Color.Red);
                await ReplyAsync("", false, embedBuilder.Build());
            }
        }
    }
}
