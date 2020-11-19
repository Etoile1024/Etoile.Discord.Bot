using Discord.Commands;
using Etoile.Discord.Bot.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etoile.Discord.Bot.Commands
{
    public class MathCommand : ModuleBase<SocketCommandContext>
    {
        [Command("mod")]
        public async Task ModCmd(int a, int n)
        {
            int ans = a % n;
            await ReplyAsync(LanguageHolder.GetTranslation("MATH_ANS", ans));
        }

        [Command("floor")]
        public async Task FloorCmd(double input)
        {
            int ans = (int)Math.Floor(input);
            await ReplyAsync(LanguageHolder.GetTranslation("MATH_ANS", ans));
        }

        [Command("ceiling")]
        public async Task CeilingCmd(double input)
        {
            int ans = (int)Math.Ceiling(input);
            await ReplyAsync(LanguageHolder.GetTranslation("MATH_ANS", ans));
        }
    }
}
