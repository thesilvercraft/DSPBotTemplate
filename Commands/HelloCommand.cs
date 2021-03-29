using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DSPBotTemplate.Commands
{
    partial class HelloCommand : BaseCommandModule
    {
        [Command("hi")]
        [Description("Hello fellow human! beep boop")]
        public async Task GreetCommand(CommandContext ctx)
        {
            await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                                             .WithContent(string.Format(HelloCommandStrings.HelloString, ctx.Member.Mention))
                                             .SendAsync(ctx.Channel);
        }
    }
}