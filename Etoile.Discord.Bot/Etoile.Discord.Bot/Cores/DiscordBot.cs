using Discord;
using Discord.Commands;
using Discord.WebSocket;
using static Etoile.Discord.Bot.Holders.AudioHolder;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lavalink4NET.Tracking;
using Etoile.Discord.Bot.Holders;

namespace Etoile.Discord.Bot.Cores
{
    public class DiscordBot
    {
        private DiscordSocketClient discordClient;
        private DiscordClientWrapper clientWrapper;
        private CommandService commandService;
        private IServiceProvider services;
        private string commandPrefix = "?";


        public async Task RunBotAsync()
        {
            //Init log
            var dates = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log";
            Log.Logger = new LoggerConfiguration()
                     .MinimumLevel.Debug()
                     .WriteTo.Console()
                     .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@".\Logs\Info\" + dates))
#if DEBUG
                     .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@".\Logs\Debug\" + dates))
                     .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Verbose).WriteTo.File(@".\Logs\Verbose\" + dates))
#endif
                     .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@".\Logs\Warning\" + dates))
                     .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@".\Logs\Error\" + dates))
                     .CreateLogger();

            discordClient = new DiscordSocketClient();
            clientWrapper = new DiscordClientWrapper(discordClient);
            commandService = new CommandService();
            services = new ServiceCollection().AddSingleton(discordClient).AddSingleton(commandService).BuildServiceProvider();

            //Init audio service
            string lavalinkIP = ConfigurationManager.AppSettings["lavalinkIP"];
            string lavalinkPort = ConfigurationManager.AppSettings["lavalinkPort"];
            string lavalinkPw = ConfigurationManager.AppSettings["lavalinkPassword"];


            audioService = new LavalinkNode(new LavalinkNodeOptions
            {
                RestUri = $"http://{lavalinkIP}:{lavalinkPort}/",
                WebSocketUri = $"ws://{lavalinkIP}:{lavalinkPort}/",
                Password = lavalinkPw,
                DisconnectOnStop = false
            }, clientWrapper);


            discordClient.Log += LogMessage;
            discordClient.Ready += () => audioService.InitializeAsync();

            bool enableInactiveCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["enableInactiveCheck"]);
            if (enableInactiveCheck)
            {
                int disconnectDelay = Convert.ToInt32(ConfigurationManager.AppSettings["disconnectDelay"]);
                int pollInterval = Convert.ToInt32(ConfigurationManager.AppSettings["pollInterval"]);
                var trackingService = new InactivityTrackingService(
                    audioService,
                    clientWrapper,
                    new InactivityTrackingOptions
                    {
                        DisconnectDelay = TimeSpan.FromSeconds(disconnectDelay),
                        PollInterval = TimeSpan.FromSeconds(pollInterval),
                        TrackInactivity = false
                    });

                // Start tracking inactive players
                trackingService.BeginTracking();
            }

            KeywordHolder.LoadKeywordList();
            LanguageHolder.LoadLanguageList();

            await RegisterCommandsAsync();
            await discordClient.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["BotToken"]);
            await discordClient.StartAsync();
            await Task.Delay(-1);
        }
        public Task LogMessage(LogMessage arg)
        {            
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    Log.Error(arg.Message);
                    break;
#if DEBUG
                case LogSeverity.Debug:
                    Log.Debug(arg.Message);
                    break;
                case LogSeverity.Verbose:
                    Log.Verbose(arg.Message);
                    break;
#endif
                case LogSeverity.Error:
                    Log.Error(arg.Message);
                    break;
                case LogSeverity.Info:
                    Log.Information(arg.Message);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(arg.Message);
                    break;
            }
            return Task.CompletedTask;
        }
        private async Task RegisterCommandsAsync()
        {
            discordClient.MessageReceived += HandleCommandAsync;
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;
            int argPos = 0;
            if (message.HasStringPrefix(commandPrefix, ref argPos))
            {
                var context = new SocketCommandContext(discordClient, message);
                var result = await commandService.ExecuteAsync(context, argPos, services);
                if (!result.IsSuccess)
                {
                    Log.Error(result.ErrorReason);
                }
                else
                {
                    string user = message.Author.Username;
                    Log.Information("User [{0}] use command {1}.", user, message.Content);
                }
            }
            if (message.MentionedUsers.Any(a => a.Id == discordClient.CurrentUser.Id) && KeywordHolder.KeywordList.Length > 0)
            {
                var context = new SocketCommandContext(discordClient, message);
                string user = message.Author.Username;
                string keyword = KeywordHolder.KeywordList[new Random().Next(0, KeywordHolder.KeywordList.Length)];
                await message.Channel.SendMessageAsync(keyword);
                Log.Information("User [{0}] mention bot.", user);
                
            }
        }

    }
}
