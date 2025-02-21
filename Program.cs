using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualBasic;

namespace NET_DiscordBot {
  public class LoggingService {
    public LoggingService(DiscordSocketClient client, CommandService command) {
      client.Log += LogAsync;
      command.Log += LogAsync;
    }

    private Task LogAsync(LogMessage msg) {
      if (msg.Exception is CommandException cmdException) {
        Console.WriteLine($"[Command/{msg.Severity}] {cmdException.Command.Aliases.First()}" + $" failed to execute in {cmdException.Context.Channel}");
        Console.WriteLine(cmdException);
      }
      else {
        Console.WriteLine($"[General/{msg.Severity}] {msg}");
      }

      return Task.CompletedTask;
    }
  }

  public class Client {
    //// WebSocket connection to Discord bot API
    private static DiscordSocketClient _client;

    //// Logging method
    private static Task Log(LogMessage msg) {
      Console.WriteLine(msg.ToString());
      return Task.CompletedTask;
    }
        
    public static async Task Main() {
      // Create a new instance of the Discord client
      _client = new DiscordSocketClient();
      // Log handler event for the discord client instance
      _client.Log += Log;

      // Get the token from the env variable
      var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
      if (String.IsNullOrEmpty(token)) {
        Console.WriteLine("No token found. Exiting.");
        return;
      }

      await _client.LoginAsync(TokenType.Bot, token);
      await _client.StartAsync();

      Console.WriteLine("Bot is now running.");
      await Task.Delay(-1);
    }

    public static async Task MainAsync() {
      var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
      _client = new DiscordSocketClient(_config);

      await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
      await _client.StartAsync();

      _client.MessageUpdated += MessageUpdated;
      _client.Ready += () => {
        Console.WriteLine("Bot is connected");
        return Task.CompletedTask;
      };

      await Task.Delay(-1);
    }

    private static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel) {
      var message = await before.GetOrDownloadAsync();
      Console.WriteLine($"{message} -> {after}");
    }
  }
}