using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DSPBotTemplate.Configs
{
    public class SCActivity
    {
        public SCActivity()
        {
        }

        public SCActivity(string name, ActivityType type)
        {
            Name = name;
            ActivityType = type;
        }

        public string Name { get; set; }

        public string StreamUrl { get; set; } = null;

        public ActivityType ActivityType { get; set; }

        public DiscordActivity ToDiscordActivity()
        {
            return new(Name, ActivityType) { StreamUrl = StreamUrl };
        }
    }
}