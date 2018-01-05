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
        readonly int AlexsPosition = 1;
        readonly int TrevorPosition = 2;
        readonly int ReisPosition = 3;

        readonly string SquadDeathboardFileName = "SquadDeathboard.txt";
        readonly string DuoDeathboardFileName = "DuosDeathboard.txt";

        /// <summary>
        /// Calls UpdateDeathCount(user, 1, lobby) to increase death count by 1 in lobby then displays deathboard
        /// Called by !kill @alexmcintosh [lobby] or !kill @alexmcintosh
        /// </summary>
        /// <param name="user"></param>
        [Command("kill")]
        public async Task KillAsync(SocketGuildUser user, string lobby = "squad")
        {

            if (lobby == "squad" || lobby == "duos") {
                
                await UpdateDeathCount(user, 1, lobby);

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle("No Chicken For You")
                       .WithDescription($"**{user.Username.ToUpper()}** died first in " + lobby.ToUpper() + " this round.")
                    .WithColor(Color.Red);
                
                await ReplyAsync("", false, builder.Build());
                await ShowDeathboardAsync(lobby);

            }
            else {
                await ReplyAsync("```Uknown lobby. Use: 'duos', 'squad', or leave blank```");
            }
        }

        /// <summary>
        /// Calls UpdateDeathCount(user, -1, lobby) to decrease death count by 1 in lobby then displays deathboard
        /// Called by !revive @alexmcintosh [lobby]
        /// Requies owner
        /// </summary>
        /// <param name="user"></param>
        [Command("revive"), RequireOwner]
        public async Task ReviveAsync(SocketGuildUser user, string lobby = "squad")
        {

            if (lobby == "squad" || lobby == "duos")
            {

                await UpdateDeathCount(user, -1, lobby);

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle("Saved!")
                       .WithDescription($"**{user.Username.ToUpper()}** had a death removed from " + lobby.ToUpper())
                       .WithColor(Color.Red);

                await ReplyAsync("", false, builder.Build());
                await ShowDeathboardAsync(lobby);

            }
            else
            {
                await ReplyAsync("```Uknown lobby. Use: 'duos', 'squad', or leave blank```");
            }
        }

        /// <summary>
        /// Updates the death count for player
        /// </summary>
        /// <param name="user"></param>
        public Task UpdateDeathCount(SocketGuildUser user, int death, string lobby)
        {
            int lineToEdit = 0;
            switch (user.Username)
            {
                case ("alexmcintosh"): lineToEdit = AlexsPosition; break;
                case ("thatguytrevor"): lineToEdit = TrevorPosition; break;
                case ("reiswarman"): lineToEdit = ReisPosition; break;
            }

            var path = string.Empty;

            if (lobby == "duos")
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), DuoDeathboardFileName);
            }
            else
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), SquadDeathboardFileName);
            }

            if (lineToEdit != 0)
            {
                //Overwrite existing file
                string[] lines = File.ReadAllLines(path);

                lines[lineToEdit - 1] = (Int32.Parse(lines[lineToEdit - 1]) + death).ToString();
                File.WriteAllLines(path, lines, Encoding.UTF8);

            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Displays number of deaths for each person.
        /// Defaults to squad if nothing entered. Called with !deathboard duos
        /// </summary>
        [Command("deathboard")]
        public async Task ShowDeathboardAsync(string lobby = "squad")
        {
            var path = string.Empty;

            switch(lobby) {
                case "duos": 
                    path = Path.Combine(Directory.GetCurrentDirectory(), DuoDeathboardFileName);
                    lobby = "Duos"; //For the title in the embed message
                    break;
                case "squad":
                    path = Path.Combine(Directory.GetCurrentDirectory(), SquadDeathboardFileName);
                    lobby = "Squad"; //For the title in the embed message
                    break;
                default: 
                    await ReplyAsync("```Uknown lobby. Use: 'duos', 'squad', or leave blank```");
                    return;
            }

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle(lobby + " Deathboard")
                .AddInlineField("Alex", File.ReadLines(path).Skip(0).Take(1).First()) //Alex is 1st line
                .AddInlineField("Trevor", File.ReadLines(path).Skip(1).Take(1).First()) //Trevor is 2nd line
                .AddInlineField("Reis", File.ReadLines(path).Skip(2).Take(1).First())   //Reis is 3rd line
                .WithCurrentTimestamp();

            await ReplyAsync("", false, builder.Build());
        }
    }
}
