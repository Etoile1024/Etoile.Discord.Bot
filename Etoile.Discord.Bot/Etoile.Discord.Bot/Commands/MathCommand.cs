using Discord.Commands;
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
            await ReplyAsync(string.Format("答案係{0}, 同學check下啱唔啱！", ans));
        }

        [Command("floor")]
        public async Task FloorCmd(double input)
        {
            int ans = (int)Math.Floor(input);
            await ReplyAsync(string.Format("答案係{0}, 同學check下啱唔啱！", ans));
        }
    }
}
