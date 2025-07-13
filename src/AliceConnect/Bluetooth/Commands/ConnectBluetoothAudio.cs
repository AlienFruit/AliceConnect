using AlienFruit.CommandLine.Abstractions;
using AlienFruit.CommandLine.Abstractions.Attributes;

namespace AliceConnect.Bluetooth.Commands
{
    [Command("connect-audio", Help = "Connect bluetooth audio device")]
    internal class ConnectBluetoothAudio : ICommand
    {
        [Option('n', "name", Help = "device name", Required = true)]
        public required string DeviceName { get; set; }
    }
}
