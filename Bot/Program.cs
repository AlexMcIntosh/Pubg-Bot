namespace Bot
{
    using System;
    using System.Threading.Tasks;
    using System.IO;
    using Discord;
    using Discord.WebSocket;
    using Discord.Commands;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _service;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _service = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
             
            string tokenPath = Path.Combine(Directory.GetCurrentDirectory(), "Key.txt");
            string botToken = File.ReadAllText(tokenPath);

            //Event subscription
            _client.Log += Log;

            await RegisterCommandAsync();
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Reads every message in chat and responds to messages with '!' prefix
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task HandleCommandAsync(SocketMessage args)
        {
            var message = args as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix("-", ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _service);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
