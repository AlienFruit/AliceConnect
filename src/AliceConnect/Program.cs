using AliceConnect.Audio.Commands;
using AliceConnect.Audio.Handlers;
using AliceConnect.Audio.Services;
using AliceConnect.Bluetooth.Commands;
using AliceConnect.Bluetooth.Handlers;
using AliceConnect.Bluetooth.Services;
using AlienFruit.CommandLine;
using AlienFruit.Core.Abstractions;
using AlienFruit.FluentConsole;
using AlienFruit.FluentConsole.AsciiArt;
using AlienFruit.Otml.Configuration;
using Figgle;
using Figgle.Fonts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;


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


var assembly = Assembly.GetExecutingAssembly();
var version = assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion.Split('+', StringSplitOptions.RemoveEmptyEntries)
.First();

FConsole.GetInstance().DrawDemo(DemoPicture.AlienfruitLogo);
FConsole
    .Color(ConsoleColor.Blue)
    .WriteLine(FiggleFonts.Standard.Render("AliceConnect"))
    .ResetColors()
    .Color(ConsoleColor.Green)
    .WriteLine($"Version: {version}")
    .NextLine()
    .ResetColors();

await CommandLineProcessor.GetBuilder()
    .WithCommandLineSettings(x => x.ThrowExcceptions = true)
    .WithHandlersFactory(new AliceConnect.HandlersFactory(host.Services))
    .Build()
    .RegisterCommand<ConnectBluetoothAudio>()
    .RegisterCommand<SwitchAudioDevice>()
    .ParseAsync(args);