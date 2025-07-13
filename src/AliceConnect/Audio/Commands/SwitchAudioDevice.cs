using AlienFruit.CommandLine.Abstractions;
using AlienFruit.CommandLine.Abstractions.Attributes;

namespace AliceConnect.Audio.Commands
{
    [Command("switch-audio", Help = "Swith connected audio device")]
    internal class SwitchAudioDevice : ICommand
    {
        [Option('n', "name", Help = "device name", Required = true)]
        public required string DeviceName { get; set; }
    }
}
