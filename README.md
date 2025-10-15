# windows-playsound-service

This is a .NET Rest API for playing sounds on a Windows device. This is implemented using .NET Core v8. The REST API is used to play a specific sound. At the REST level, sounds are logical (named). Sounds are mapped to specific wav files using appsettings.json (details below).

The use case for this is narrow. In my case, I have a non-default audio output on a Windows server connected to a home audio system. When a sound is played on that audio output device, it is heard in parts of the home. This service is connected to an Apple home system via a Homebridge plugin (which calls this via REST APIs). The homebridge plug-in exposes the sounds (REST API) as switches (when the switch is triggered, the sound plays). 

On the Apple Home side, there are automations that trigger under certain conditions (e.g. a specific person arrives home, a "person" was detected on a camera or a door/window/gate was opened). When those automations are triggered, a sound is played in the house.

# Usage
Once setup (details below), a wav file can be played by making a GET call to this service in the form of:

* http://localhost:[port]/playsound/{sound}

Where "sound" is the name of the sound to play. "sound" is mapped to a specific wav file using appsettings.json. See below for how to map a "sound" string to a specific wav file.

Debug and configiration data will be returned when a sound is ommitted:

* http://localhost:[port]/playsound

# Setup and Build

Details on IIS and ASP.NET Core setup below. This information comes from [this link](https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-iis?view=aspnetcore-9.0&tabs=visual-studio).

## Install IIS (if not already installed)

Search (Windows Key) for and run "Turn Windows features on or off". If not installed, select and install "Internet Information Services".

## Install the .NET Windows Hosting Bundle

The .NET 8.0 version can be found [here](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.21-windows-hosting-bundle-installer).

## Config

### appsettings.json
The appsetting.json file contains config data needed by the service. All service config is contained within the PlaySound section.

* DeviceFriendlyName: Set this to play sound on the non-default device. http://localhost:[port]/playsound will return a device list.
* WavFolder: The folder that contains the wav files.
* Sounds:{name}: Maps a sound name from the GET request to a specific wav file.

### appsettings.json Examples

```json
  {
    "PlaySound": {
      "DeviceFriendlyName": "Speakers (TTGK Audio)",
      "WavFolder": "C:/Sounds/",
      "Sounds": {
        "arrive": "arrive.wav",
        "leave": "depart.wav"
      }
    }
  }
```

In response to a GET http://localhost:[port]/playsound/arrive, the service will play "C:/Sounds/arrive.wav" on the output device with the friendly name "Speakers (TTGK Audio)".

# Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

# License

[MIT](https://choosealicense.com/licenses/mit/)