using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etoile.Discord.Bot.Holders
{
    public static class LanguageHolder
    {
        private static Dictionary<string, string> LanguageList = new Dictionary<string, string>();

        public static void LoadLanguageList()
        {
            string[] content = File.ReadAllLines("language.txt", Encoding.UTF8);
            foreach (var current in content)
            {
                if (!current.StartsWith("#")) //comment signal
                {
                    var segment = current.Split('=');
                    string key = segment[0];
                    string text = string.Empty;
                    for (int i = 1; i < segment.Length; i++)
                    {
                        text = text == string.Empty ? segment[i] : string.Format("{0}={1}", text, segment[i]);
                    }
                    LanguageList.Add(key, text);
                }
            }
        }

        public static string GetTranslation(string key, params object[] parameter)
        {
            if(LanguageList.TryGetValue(key, out var text))
            {
                return string.Format(text, parameter);
            }
            Log.Warning("Missing translation: {0}", key);
            return key;
        }
    }
}
