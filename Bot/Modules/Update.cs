namespace Bot.Modules
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Update : ModuleBase<SocketCommandContext>
    {
        private readonly int AlexsPosition = 1;
        private readonly int TrevorPosition = 2;
        private readonly int ReisPosition = 3;

        /// <summary>
        /// Calls ModifyDeathCount(user, 1) to increase death count by 1
        /// </summary>
        /// <param name="user"></param>
        [Command("kill")]
        public async Task KillAsync(SocketGuildUser user)
        {
            await UpdateDeathCount(user, 1);
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("No Chicken For You")
                .WithDescription($"**{user.Username}** died first this round.")
                .WithColor(Color.Red);

            await ReplyAsync("", false, builder.Build());
            await ShowDeathboardAsync();
        }

        /// <summary>
        /// Calls ModifyDeathCount(user, -1) to decrease death count by 1
        /// Requires owner for security
        /// </summary>
        /// <param name="user"></param>
        [Command("revive"), RequireOwner]
        public async Task ReviveAsync(SocketGuildUser user)
        {
            await UpdateDeathCount(user, -1);
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Saved!")
                .WithDescription($"Removed one death from **{user.Username}**")
                .WithColor(Color.Blue);

            await ReplyAsync("", false, builder.Build());
            await ShowDeathboardAsync();
        }

        /// <summary>
        /// Updates the death count for player
        /// </summary>
        /// <param name="user"></param>
        public Task UpdateDeathCount(SocketGuildUser user, int death)
        {
            int lineToEdit = 0;
            switch (user.Username)
            {
                case ("alexmcintosh"): lineToEdit = AlexsPosition; break;
                case ("thatguytrevor"): lineToEdit = TrevorPosition; break;
                case ("reiswarman"): lineToEdit = ReisPosition; break;
                default: break;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Deathboard.txt");

            if (lineToEdit != 0)
            {
                //Overwrite existing file
                string[] lines = File.ReadAllLines(path);

                lines[lineToEdit - 1] = (Int32.Parse(lines[lineToEdit - 1]) + death).ToString();
                File.WriteAllLines("Deathboard.txt", lines, Encoding.UTF8);

            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Displays number of deaths for each person
        /// </summary>
        [Command("deathboard")]
        public async Task ShowDeathboardAsync()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Deathboard.txt");

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Deathboard")
                .AddInlineField("Alex", File.ReadLines(path).Skip(0).Take(1).First()) //Alex is 1st line
                .AddInlineField("Trevor", File.ReadLines(path).Skip(1).Take(1).First()) //Trevor is 2nd line
                .AddInlineField("Reis", File.ReadLines(path).Skip(2).Take(1).First())   //Reis is 3rd line
                .WithCurrentTimestamp();

            await ReplyAsync("", false, builder.Build());
        }
    }
}
