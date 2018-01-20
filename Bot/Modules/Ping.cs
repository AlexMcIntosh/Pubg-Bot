namespace Bot.Modules
{
    using Discord;
    using Discord.Commands;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class Help : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Used to show commands available to users
        /// </summary>
        /// <returns></returns>
        [Command("ping")]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Commands")
                .AddField("Add Death", "-k <@username> <lobby>")
                .AddField("Show Deathboard", "-board <lobby>");

            await ReplyAsync("", false, builder.Build());

        }
    }
}
