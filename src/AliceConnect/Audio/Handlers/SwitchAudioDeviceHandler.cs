using AliceConnect.Audio.Commands;
using AliceConnect.Audio.Services;
using AlienFruit.Core.Abstractions;

namespace AliceConnect.Audio.Handlers
{
    internal class SwitchAudioDeviceHandler(AudioService audioService) : IHandler<SwitchAudioDevice>
    {
        public Task HandleAsync(SwitchAudioDevice arg)
        {
            audioService.SwitchAudioDevice(arg.DeviceName);
            return Task.CompletedTask;
        }

        public Task HandleAsync(object arg) => HandleAsync((SwitchAudioDevice) arg);
    }
}
