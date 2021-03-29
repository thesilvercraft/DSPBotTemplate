using DSharpPlus.Entities;
using DSPBotTemplate.Attributes;

namespace DSPBotTemplate.Configs
{
    public class XmlConfigFormat
    {
        [XmlDescription("Array of prefixes the bot will respond to")]
        public string[] Prefixes { get; set; } = { "st!", "st" };

        [XmlDescription("The Discord token, can be got from https://discord.com/developers/")]
        public string Token { get; set; } = "Discord_Token_Here";

        [XmlDescription("The current config version, don't change unless told by the bot or silverdimond")]
        public ulong? ConfigVer { get; set; } = null;

        [XmlDescription("DiscordActivities the bot will use")]
        public SCActivity[] Activities { get; set; } = { new("Developers music video", ActivityType.ListeningTo), new("Around the world", ActivityType.ListeningTo) };

        [XmlDescription("Interval for how fast the Activities will change, use null for no changes")]
        public ulong? MsInterval { get; set; } = 1800000;

        //TODO: ADD MORE STUFF HERE
    }
}