using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using Microsoft.Extensions.Logging;

namespace AliceConnect.Audio.Services
{
    internal class AudioService(ILogger<AudioService> logger)
    {
        public void SwitchAudioDevice(string deviceName)
        {
            logger.LogInformation($"Searching for audio device: {deviceName}");

            using var controller = new CoreAudioController();

            var devices = controller.GetPlaybackDevices(DeviceState.Active).ToList();

            if (devices.Count == 0)
            {
                logger.LogWarning("No active audio devices found!");
                return;
            }

            logger.LogInformation("Available devices:");
            foreach (var device in devices)
            {
                logger.LogInformation($"- {device.FullName} ({device.State})");
            }

            var targetDevice = devices.FirstOrDefault(d => d.FullName.Contains(deviceName, StringComparison.OrdinalIgnoreCase));

            if (targetDevice == null)
            {
                logger.LogWarning($"Device '{deviceName}' not found");
                return;
            }

            if (targetDevice.IsDefaultDevice)
            {
                logger.LogInformation($"Device '{targetDevice.FullName}' is already set as default");
                return;
            }

            targetDevice.SetAsDefault();
            logger.LogInformation($"Success: {targetDevice.FullName} is now the default device");
        }
    }
}
