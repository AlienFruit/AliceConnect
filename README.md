# AliceConnect

A Windows automation service that exploits a bug in Yandex Station Mini 3 speakers to enable remote media control while audio is played through different output devices.

## Overview

AliceConnect is a .NET service designed to automate Windows operations by leveraging a specific behavior in Yandex Station Mini 3 Bluetooth speakers. When connected to the speaker, selecting it as the current audio device, and then switching to primary speakers, the Yandex Station continues to control the computer's media player despite audio being played through the primary speakers.

This project serves as a foundation for Windows automation scenarios where remote control capabilities are needed.

## Features

- **Audio Device Management**: Automatically switch between audio output devices
- **Bluetooth Device Control**: Connect and manage Bluetooth audio devices
- **Media Player Control**: Maintain remote control capabilities through Yandex Station
- **Windows Automation**: Foundation for automated Windows operations

## Prerequisites

- Windows 10/11
- .NET 8.0 or later

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/AliceConnect.git
cd AliceConnect
```

2. Build the project:
```bash
dotnet build
```

3. Run the application:
```bash
dotnet run
```

## Usage

### Basic Audio Device Switching

```bash
# Switch to a specific audio device
AliceConnect switch-audio -n "Yandex Station Mini 3"
```

### Bluetooth Device Connection

```bash
# Connect to a Bluetooth audio device
AliceConnect connect-audio -n "Yandex Station Mini 3"
```

### Complete Workflow Example

```bash
# 1. Connect to Yandex Station Mini 3
AliceConnect connect-audio -n "Yandex Station Mini 3"

# 2. Switch audio output to the station
AliceConnect switch-audio -n "Yandex Station Mini 3"

# 3. Switch back to primary speakers (station maintains control)
AliceConnect switch-audio -n "Speakers"
```


## How It Works

1. **Device Discovery**: The service scans for available audio devices and Bluetooth connections
2. **Connection Management**: Establishes connections with Bluetooth devices using Windows API
3. **Audio Routing**: Manages audio output device switching while maintaining control connections
4. **Remote Control**: Leverages the Yandex Station bug to maintain media control capabilities

## Technical Details

- Uses `AudioSwitcher.AudioApi` for audio device management
- Implements Windows Bluetooth API for device connectivity
- Leverages CoreAudio API for audio device control
- Built with dependency injection for modularity

## Future Plans

- Integration with Windows Task Scheduler
- Support for additional automation scenarios
- Web API for remote control capabilities

## Contributing

1. Create a feature branch
2. Make your changes
3. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Disclaimer

This project exploits a specific behavior in Yandex Station Mini 3 speakers. Use responsibly and in accordance with applicable laws and regulations. The authors are not responsible for any misuse of this software.