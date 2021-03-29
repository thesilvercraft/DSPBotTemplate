using DSharpPlus.Entities;
using Serilog.Core;
using System;
using System.Threading.Tasks;

namespace DSPBotTemplate.Configs
{
    internal interface IConfig
    {
        /// <summary>
        /// Is ran on startup, for example: can be used to read or write to files
        /// </summary>
        /// <returns>a bool that indicates if everything went ok, or something is burning down as we speak</returns>
        public Task<bool> StartUp(Logger logger);

        /// <summary>
        /// Gets the prefixes the bot should respond to
        /// </summary>
        /// <returns>an array of <see cref="string"/>s</returns>
        public Task<string[]> Prefixes();

        /// <summary>
        /// Gets the discord token
        /// </summary>
        /// <returns>a boring long ass string that we need to send to discord on login</returns>
        public Task<string> DiscordToken();

        /// <summary>
        /// Gets an activity and a timespan on how long to wait till asking for a new activity
        /// </summary>
        /// <returns>a <see cref="Tuple"/> with a <see cref="DiscordActivity"/> and a <see cref="TimeSpan?"/> which should be null if you dont want to change the activity till next launch</returns>

        public Task<Tuple<DiscordActivity, TimeSpan?>> Activity();

        //TODO: ADD MORE METHODS HERE
    }
}