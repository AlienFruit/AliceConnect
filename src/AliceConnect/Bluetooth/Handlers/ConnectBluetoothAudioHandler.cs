using AliceConnect.Bluetooth.Commands;
using AliceConnect.Bluetooth.Services;
using AlienFruit.Core.Abstractions;

namespace AliceConnect.Bluetooth.Handlers
{
    internal class ConnectBluetoothAudioHandler(BluetoothService bluetoothService) : IHandler<ConnectBluetoothAudio>
    {
        public Task HandleAsync(ConnectBluetoothAudio arg)
        {
            bluetoothService.ConnectAudioDeviceByName(arg.DeviceName);
            return Task.CompletedTask;
        }

        public Task HandleAsync(object arg) => HandleAsync((ConnectBluetoothAudio) arg);
    }
}
