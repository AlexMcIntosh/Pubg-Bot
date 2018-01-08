namespace Bot.Modules
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public class Update : ModuleBase<SocketCommandContext>
    {
        readonly int AlexsPosition = 1;
        readonly int TrevorPosition = 2;
        readonly int ReisPosition = 3;

        readonly string SquadsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SquadsDeathboard.txt");
        readonly string DuosFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DuosDeathboard.txt");


        /// <summary>
        /// Increases player death count by 1
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="lobby">Lobby. Must be duos or squads</param>
        [Command("kill")]
        public async Task AddDeathAsync(SocketGuildUser user, string lobby = "squads") {
            var isValidLobby = await ValidateLobbyType(lobby);

            if (!isValidLobby) { return; }

            var builder = new EmbedBuilder();
            builder.WithTitle("No Chicken For You")
                   .WithDescription($"**{user.Username.ToUpper()}** died first in **" + lobby.ToUpper() + "** this round.")
                   .WithColor(Color.Red);
            
            await ReplyAsync("", false, builder.Build());
            await UpdateDeathCount(user, 1, lobby);
        }

        /// <summary>
        /// Removes deaths from player
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="deathsToRemove">Deaths to remove.</param>
        /// <param name="lobby">Lobby. Must be duos or squads</param>
        [Command("remove-death"), RequireOwner]
        public async Task RemoveDeathAsync(SocketGuildUser user, int deathsToRemove = 1, string lobby = "squads") {
            var isValidLobby = await ValidateLobbyType(lobby);

            if (!isValidLobby) { return; }

            var builder = new EmbedBuilder();
            builder.WithDescription($"**{user.Username.ToUpper()}** had a death removed from **" + lobby.ToUpper() + "**")
                   .WithColor(Color.Blue);
            
            await ReplyAsync("", false, builder.Build());
            await UpdateDeathCount(user, deathsToRemove * -1, lobby);
        }

        /// <summary>
        /// Displays current death count for players in lobby
        /// </summary>
        /// <param name="lobby">Lobby type. Must be duos or squads</param>
        [Command("deathboard")]
        public async Task ShowDeathboardAsync(string lobby = "squads") {
            var path = string.Empty;

            switch(lobby) {
                case "duos":
                    path = DuosFilePath;
                    break;
                case "squads":
                    path = SquadsFilePath;
                    break;
                default: 
                    await ReplyAsync("```Uknown lobby. Use: 'duos', 'squads', or leave blank```");
                    return;
            }

            var builder = new EmbedBuilder();
            builder.WithTitle(lobby.ToUpper() + " Deathboard")
                .AddInlineField("Alex", File.ReadLines(path).Skip(0).Take(1).First()) //Alex is 1st line
                .AddInlineField("Trevor", File.ReadLines(path).Skip(1).Take(1).First()) //Trevor is 2nd line
                .AddInlineField("Reis", File.ReadLines(path).Skip(2).Take(1).First())   //Reis is 3rd line
                .WithCurrentTimestamp();

            await ReplyAsync("", false, builder.Build());
        }

        /// <summary>
        /// Updates the death count.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="death">Death.</param>
        /// <param name="lobby">Lobby.</param>
        public async Task UpdateDeathCount(SocketGuildUser user, int death, string lobby) {
            var lineToEdit = 0;

            switch (user.Username) {
                case ("alexmcintosh"): lineToEdit = AlexsPosition; break;
                case ("thatguytrevor"): lineToEdit = TrevorPosition; break;
                case ("reiswarman"): lineToEdit = ReisPosition; break;
            }

            var path = string.Empty;

            if (lobby == "duos") {
                path = DuosFilePath;
            }
            else {
                path = SquadsFilePath;
            }

            if (lineToEdit != 0) {
                //Overwrite existing file
                var lines = File.ReadAllLines(path);
                var currentDeathCount = Int32.Parse(lines[lineToEdit - 1]) + death;
                lines[lineToEdit - 1] = currentDeathCount.ToString();

                File.WriteAllLines(path, lines, Encoding.UTF8);

                await ShowDeathboardAsync(lobby);
            }

        }

        /// <summary>
        /// Resets the boards
        /// </summary>
        /// <param name="lobby">Lobby.</param>
        [Command("reset-board"), RequireOwner]
        public async Task ResetAsync(string lobby = "") {
            switch(lobby) {
                case "all":
                    await ResetFileAsync(DuosFilePath);
                    await ShowDeathboardAsync("duos");
                    await ResetFileAsync(SquadsFilePath);
                    await ShowDeathboardAsync("squads");
                    break;
                case "squads":
                    await ResetFileAsync(SquadsFilePath);
                    await ShowDeathboardAsync("squads");
                    break;
                case "duos":
                    await ResetFileAsync(DuosFilePath);
                    await ShowDeathboardAsync("duos");
                    break;
                default:
                    await ReplyAsync("```Please specify which boards you would like to reset: duos, squads, all```");
                    break;
            }
        }

        /// <summary>
        /// Validates the type of the lobby.
        /// </summary>
        /// <returns>Lobby validation</returns>
        /// <param name="lobby">Lobby.</param>
        public async Task<bool> ValidateLobbyType(string lobby) {
            if (lobby == "squads" || lobby == "duos") {
                return true;
            }

            await ReplyAsync("```Uknown lobby. Use: 'duos', 'squads', or leave blank```");

            return false;
        }

        /// <summary>
        /// Resets the file for deathboards.
        /// </summary>
        /// <returns>Task completion</returns>
        /// <param name="path">Path.</param>
        public Task ResetFileAsync(string path) {
            var lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Count(); i++) {
                lines[i] = "0";
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);

            return Task.CompletedTask;
        }

        /// <summary>
        /// To be used when someone abuses the bot
        /// </summary>
        [Command("fuck-up"), RequireOwner]
        public async Task DestroyTrevorAsync(SocketUser user, int loop) {
            
            while(loop > 0) {
                await user.SendMessageAsync("get fucked");
                loop--;
            }
        }
    }
}
