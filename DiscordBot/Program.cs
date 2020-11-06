using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;
using System.Configuration;

namespace DiscordBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private readonly string _botToken = ConfigurationManager.AppSettings["BotToken"];

        public static void Main(string[] args)
        {
            new Program().RunBotAsync().GetAwaiter().GetResult();
        }

        private async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<AudioService>()
                .BuildServiceProvider();

            _client.Log += Log;

            await RegisterCommandAsync();

            await _client.LoginAsync(TokenType.Bot, _botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        private async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            try
            {
                if (!(arg is SocketUserMessage message) || message.Author.IsBot) return;

                int argPos = 0;

                if (message.HasStringPrefix("sik!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var context = new SocketCommandContext(_client, message);

                    var result = await _commands.ExecuteAsync(context, argPos, _services);

                    if (!result.IsSuccess)
                        Console.WriteLine(result.ErrorReason);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}