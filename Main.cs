namespace Windows.PlaySound.Service;

using System.IO;
using System.Text;

public class Program
{
  public static void Main(string[] args)
  {
    // appsettings.json sections for sound
    const string SOUND_CONFIG = "PlaySound:";
    const string SOUNDS_CONFIG = SOUND_CONFIG + "Sounds:";
    const string FOLDER_CONFIG = SOUND_CONFIG + "WavFolder";
    const string DEVICE_CONFIG = SOUND_CONFIG + "DeviceFriendlyName";

    // Error wav
    const string ERROR_WAV = "./error.wav";

    // Sound route
    const string SOUND_ROUTE = "sound";

    // Full end-point /playsound/{sound:alpha}
    const string PLAYSOUND_ROUTE = "/playsound";
    const string FULL_SOUND_ROUTE = PLAYSOUND_ROUTE + "/{" + SOUND_ROUTE + ":alpha}";

    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();
    var config = builder.Configuration;

    // Get /playsound with no other route. For this, return debug info
    app.MapGet(PLAYSOUND_ROUTE, async context =>
    {
      // If only /playsound endpoint, then return debug info
      StringBuilder sb = new();

      sb.Append($"Wav Folder: {config[FOLDER_CONFIG]}\n");
      sb.Append($"Error Sound: {ERROR_WAV}\n");
      sb.Append("Audio Devices:\n");
      foreach (string ad in Windows.PlaySound.Service.GetAudioDeviceNames())
      {
        sb.Append($"  '{ad}'\n");
      }
      await context.Response.WriteAsync(sb.ToString());
    });

    app.MapGet(FULL_SOUND_ROUTE, async context =>
    {
      // Get the sound name (e.g., "arrival" -> /playsound/arrival) and convert to wav file name (via appsettings.json)
      string? wavFile = config[SOUNDS_CONFIG + context.GetRouteValue(SOUND_ROUTE) as string];

      // Get the sound name (e.g., arrival) from the query string: http://localhost/playsound?sound=arrival
      if (!string.IsNullOrEmpty(wavFile))
      {
        string? wavFolder;

        // If no default folder, use current directory
        if (string.IsNullOrEmpty(wavFolder = config[FOLDER_CONFIG])) wavFolder = @"./";

        // Set full path
        wavFile = Path.Combine(wavFolder, wavFile);
      }

      // If invalid, play the ERROR wav file
      if (!File.Exists(wavFile)) wavFile = ERROR_WAV;

      // Play sound
      WindowsSound.PlaySound.PlayWav(wavFile, config[DEVICE_CONFIG]);

      // Debug info
      await context.Response.WriteAsync($"playing: {wavFile}");
    });

    // Reminder to use the /playsound endpoint
    app.MapGet("/", () => "Use " + FULL_SOUND_ROUTE + " endpoint.");

    app.Run();
  }
}
