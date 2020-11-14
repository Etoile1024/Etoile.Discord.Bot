using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etoile.Discord.Bot.Holders
{
    public static class KeywordHolder
    {
        public static string[] KeywordList;

        public static void LoadKeywordList()
        {
            KeywordList = File.ReadAllLines("keywords.txt", Encoding.UTF8);
        }
    }
}
