using Etoile.Discord.Bot.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etoile.Discord.Bot
{
    class Program
    {
        static void Main(string[] args) => new DiscordBot().RunBotAsync().GetAwaiter().GetResult();
    }
}
