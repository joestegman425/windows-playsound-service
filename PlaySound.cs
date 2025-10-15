namespace Windows.PlaySound.Service;

using NAudio.CoreAudioApi;
using NAudio.Wave;

public static class Player
{
  public static void PlayWav(string audioPath, string? deviceFriendlyName = null)
  {
    const int HALF_SECOND = 500;

    // Get the player
    using IWavePlayer waveOutDevice = GetWavePlayer(deviceFriendlyName);

    // Get audio file
    using AudioFileReader audioFile = new(audioPath);

    // Play
    waveOutDevice.Init(audioFile);
    waveOutDevice.Play();
    while (waveOutDevice.PlaybackState == PlaybackState.Playing)
    {
      Thread.Sleep(HALF_SECOND);
    }
  }

  private static List<MMDevice> GetAudioDevices()
  {
    List<MMDevice> devices = [];
    MMDeviceEnumerator enumerator = new();

    // Enumerate output (render) devices
    var playbackDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

    foreach (MMDevice endpoint in playbackDevices)
    {
      devices.Add(endpoint);
    }

    return devices;
  }

  public static List<string> GetAudioDeviceNames()
  {
    List<string> devices = [];

    foreach (MMDevice device in GetAudioDevices())
    {
      devices.Add(device.FriendlyName);
    }

    return devices;
  }

  private static MMDevice? GetAudioDevice(string friendlyName)
  {
    MMDevice? outputDevice = null;

    // Enumerate output (render) devices
    foreach (MMDevice endpoint in GetAudioDevices())
    {
      if (endpoint.FriendlyName == friendlyName)
      {
        outputDevice = endpoint;
        break;
      }
    }

    return outputDevice;
  }

  private static IWavePlayer GetWavePlayer(string? friendlyName = null)
  {
    MMDevice? device = null;

    // If specifiec, find the audio output device (otherwise, use the default)
    if (null != friendlyName) device = GetAudioDevice(friendlyName);

    // Create and return the player
    return (device == null) ? new WaveOutEvent() : new WasapiOut(device, AudioClientShareMode.Shared, false, 0);
  }
}
