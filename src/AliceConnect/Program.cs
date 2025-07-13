using AliceConnect.Audio.Commands;
using AliceConnect.Audio.Handlers;
using AliceConnect.Audio.Services;
using AliceConnect.Bluetooth.Commands;
using AliceConnect.Bluetooth.Handlers;
using AliceConnect.Bluetooth.Services;
using AlienFruit.CommandLine;
using AlienFruit.Core.Abstractions;
using AlienFruit.Otml.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

const string SPEAKER_NAME = "Станция Мини 3";
const string TARGET_AUDIO_DEVICE = "Headphones";

Console.OutputEncoding = Encoding.UTF8;



using IHost host = Host.CreateDefaultBuilder(args)
    .AddOtmlConfigurationFile("settings.otml")
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Warning);
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddSingleton<BluetoothService>()
            .AddSingleton<AudioService>()
            .AddScoped<IHandler<ConnectBluetoothAudio>, ConnectBluetoothAudioHandler>()
            .AddScoped<IHandler<SwitchAudioDevice>, SwitchAudioDeviceHandler>();
    })
.Build();
await host.StartAsync();

await CommandLineProcessor.GetBuilder()
    .WithCommandLineSettings(x => x.ThrowExcceptions = true)
    .WithHandlersFactory(new AliceConnect.HandlersFactory(host.Services))
    .Build()
    .RegisterCommand<ConnectBluetoothAudio>()
    .RegisterCommand<SwitchAudioDevice>()
    .ParseAsync(args);