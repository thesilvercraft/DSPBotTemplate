using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSPBotTemplate.Configs;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace DSPBotTemplate
{
    internal class Program
    {
        private static Logger log;
        private static ServiceProvider serviceProvider;
        private static DiscordClient discord;

        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static HttpClient NewhttpClientWithUserAgent()
        {
            HttpClient e = new();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd(Assembly.GetExecutingAssembly().GetName().Name);
            return e;
        }

        private static async Task MainAsync()
        {
            log = new LoggerConfiguration()
                         .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                            .CreateLogger();
            IConfig config = new XmlConfig();
            if (!await config.StartUp(log))
            {
                log.Error("Error while reading config");
                Environment.Exit(725);
            }
            log.Information("Making a DiscordClient");
            discord = new DiscordClient(new DiscordConfiguration()
            {
                LoggerFactory = new LoggerFactory().AddSerilog(log),
                Token = await config.DiscordToken(),
                TokenType = TokenType.Bot
            });
            log.Information("Initialising Interactivity for the DiscordClient");
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            ServiceCollection services = new();
            services.AddSingleton(NewhttpClientWithUserAgent());
            services.AddSingleton(config);
            //TODO ADD THINGS BOT WILL USE
            serviceProvider = services.BuildServiceProvider();
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = await config.Prefixes(),
                Services = serviceProvider
            });
            log.Information("Regisitring Commands");
            var exnspc = Assembly.GetExecutingAssembly();
            string nspace = $"{exnspc.GetName().Name}.Commands";
            var q = from t in exnspc.GetTypes()
                    where t.IsClass && !t.IsAbstract && t.BaseType == typeof(BaseCommandModule) && t.Namespace == nspace
                    select t;
            foreach (var t in q)
            {
                commands.RegisterCommands(t);
            }

            log.Information("Connecting to discord");
            await discord.ConnectAsync(new("console logs while booting up", ActivityType.Watching));
            log.Information("Waiting 3s");
            await Task.Delay(3000);
            while (true)
            {
                log.Debug("Updating the status to a random one");
                var ac = await config.Activity();
                await discord.UpdateStatusAsync(ac.Item1);
                log.Debug($"Waiting {((TimeSpan)ac.Item2).Humanize(precision: 5)}");
                await Task.Delay((TimeSpan)ac.Item2);
            }
        }
    }
}