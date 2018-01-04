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
        [Command("help")]
        public async Task HelpAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Commands")
                .AddField("Increase Death Count", "!kill <@username>")
                .AddField("Decrease Death Count", "!revive <@username>")
                .AddField("Show Deathboard", "!deathboard");

            await ReplyAsync("", false, builder.Build());

        }
    }
}
