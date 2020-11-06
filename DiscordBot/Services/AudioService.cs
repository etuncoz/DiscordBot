using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        try
        {
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                return;
            }

            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JoinAudio error: {ex}");
        }
    }

    public async Task LeaveAudio(IGuild guild)
    {
        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
        }
    }

    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string songName)
    {
        try
        {
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

            var resourcePath = @$"{AppDomain.CurrentDomain.BaseDirectory}Resources\Sounds\";

            Console.WriteLine($"Sound resource path:{resourcePath}");

            //var resourcePath = @"C:\Projects\Personal\Discord\DiscordBot\DiscordBot\Resources\Sounds\";

            var path = resourcePath + songName;

            var di = new DirectoryInfo(resourcePath);

            FileInfo[] files = di.GetFiles("*");

            var songFile = files.FirstOrDefault(f => f.Name.Contains(songName));

            if (songFile == null)
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }

            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                using var ffmpeg = CreateProcess(resourcePath + songFile.Name);
                using var stream = client.CreatePCMStream(AudioApplication.Music);
                try
                {
                    //await channel.SendMessageAsync($"Playing {songFile.Name}");
                    await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
                }
                finally
                {
                    await stream.FlushAsync();
                }
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SendAudio error: {ex}");
        }
    }

    private Process CreateProcess(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg.exe",
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });
    }
}