using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace AliceConnect.Bluetooth.Services
{
    internal class BluetoothService(ILogger<BluetoothService> logger)
    {
        const int BLUETOOTH_SERVICE_DISABLE = 0;
        const int BLUETOOTH_SERVICE_ENABLE = 0x00000001;

        [DllImport("bluetoothapis.dll", SetLastError = true)]
        private static extern int BluetoothSetServiceState(
            nint hRadio,
            ref BLUETOOTH_DEVICE_INFO pbtdi,
            ref Guid serviceGuid,
            int serviceState);

        [DllImport("bluetoothapis.dll", SetLastError = true)]
        private static extern nint BluetoothFindFirstRadio(
            ref BLUETOOTH_FIND_RADIO_PARAMS pbtfrp,
            out nint phRadio);

        [DllImport("bthprops.cpl", SetLastError = true)]
        private static extern nint BluetoothFindFirstDevice(ref BLUETOOTH_DEVICE_SEARCH_PARAMS pbtsp, ref BLUETOOTH_DEVICE_INFO pbtdi);

        [DllImport("bthprops.cpl", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BluetoothFindNextDevice(nint hFind, ref BLUETOOTH_DEVICE_INFO pbtdi);

        [DllImport("bthprops.cpl", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BluetoothFindDeviceClose(nint hFind);

        [DllImport("bluetoothapis.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BluetoothFindRadioClose(nint hFind);


        [DllImport("bluetoothapis.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BluetoothFindNextRadio(
            nint hFind,
            out nint phRadio);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(nint hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct BLUETOOTH_FIND_RADIO_PARAMS
        {
            public int dwSize;
        }

        [StructLayout(LayoutKind.Sequential, Size = 40)]
        private struct BLUETOOTH_DEVICE_SEARCH_PARAMS
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fReturnAuthenticated;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fReturnRemembered;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fReturnUnknown;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fReturnConnected;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fIssueInquiry;
            public int cTimeoutMultiplier;
            public nint hRadio;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct BLUETOOTH_DEVICE_INFO
        {
            public int dwSize;
            public ulong Address;
            public uint ulClassofDevice;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fConnected;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fRemembered;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAuthenticated;
            public SYSTEMTIME stLastSeen;
            public SYSTEMTIME stLastUsed;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 248)]
            public string szName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        // AUDIO SINK (A2DP)
        private static Guid AudioSinkServiceClassId = new("0000110e-0000-1000-8000-00805f9b34fb");

        public void ConnectAudioDeviceByName(string deviceName)
        {
            logger.LogInformation($"Searching for {deviceName} in paired devices...");

            var devices = GetBluetoothDevices();
            if (devices.Count == 0)
            {
                logger.LogWarning("No paired Bluetooth devices found");
                return;
            }

            logger.LogInformation($"Found {devices.Count} paired devices");
            foreach (var device in devices)
            {
                logger.LogInformation($"Device: {device.szName}, Connected: {device.fConnected}");
            }

            var speaker = devices.FirstOrDefault(d => 
                !string.IsNullOrEmpty(d.szName) 
                && d.szName.Contains(deviceName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(speaker.szName))
            {
                logger.LogInformation("Available devices:");
                foreach (var device in devices)
                {
                    logger.LogInformation($"- {device.szName} (Connected: {device.fConnected})");
                }
                throw new Exception($"Device '{deviceName}' not found in paired devices list");
            }

            logger.LogInformation($"Found speaker: {speaker.szName} (Connected: {speaker.fConnected})");

            if (speaker.fConnected)
            {
                logger.LogInformation($"Device '{speaker.szName}' is already connected.");
                return;
            }

            ConnectViaWindowsAPI(speaker.Address);
        }

        private static List<BLUETOOTH_DEVICE_INFO> GetBluetoothDevices()
        {
            var devices = new List<BLUETOOTH_DEVICE_INFO>();

            var searchParams = new BLUETOOTH_DEVICE_SEARCH_PARAMS
            {
                dwSize = Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
                fReturnAuthenticated = true,
                fReturnRemembered = true,
                fReturnUnknown = false,
                fReturnConnected = true,
                fIssueInquiry = false,
                cTimeoutMultiplier = 0,
                hRadio = nint.Zero
            };

            var deviceInfo = new BLUETOOTH_DEVICE_INFO
            {
                dwSize = Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>()
            };

            nint hFind = BluetoothFindFirstDevice(ref searchParams, ref deviceInfo);
            if (hFind == nint.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new BluetoothException(error, "BluetoothFindFirstDevice error");
                }
                return devices;
            }

            try
            {
                devices.Add(deviceInfo);

                while (true)
                {
                    deviceInfo = new BLUETOOTH_DEVICE_INFO
                    {
                        dwSize = Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>()
                    };

                    if (!BluetoothFindNextDevice(hFind, ref deviceInfo))
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error == 259)
                            break;

                        throw new BluetoothException(error, "BluetoothFindNextDevice error");
                    }

                    devices.Add(deviceInfo);
                }
            }
            finally
            {
                BluetoothFindDeviceClose(hFind);
            }

            return devices;
        }

        private void ConnectViaWindowsAPI(ulong address)
        {
            BLUETOOTH_FIND_RADIO_PARAMS findParams = new()
            {
                dwSize = Marshal.SizeOf<BLUETOOTH_FIND_RADIO_PARAMS>()
            };

            var findHandle = nint.Zero;
            var radioHandle = nint.Zero;
            var radios = new List<nint>();

            try
            {
                findHandle = BluetoothFindFirstRadio(ref findParams, out radioHandle);
                if (findHandle == nint.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new BluetoothException(error, "Error searching for radio adapters");
                }

                radios.Add(radioHandle);

                logger.LogInformation("Found first Bluetooth adapter");

                // Search for additional adapters
                while (BluetoothFindNextRadio(findHandle, out radioHandle))
                {
                    radios.Add(radioHandle);
                    logger.LogInformation($"Found additional Bluetooth adapter: 0x{radioHandle:X}");
                }

                if (radios.Count == 0)
                {
                    logger.LogWarning("No Bluetooth adapters found!");
                    return;
                }

                BLUETOOTH_DEVICE_INFO deviceInfo = new()
                {
                    dwSize = Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>(),
                    Address = address,
                    fRemembered = true,
                    fAuthenticated = true,
                    szName = new string('\0', 248) // Важно: заполняем пустой строкой
                };

                foreach (var radio in radios)
                {
                    logger.LogInformation($"Checking adapter: 0x{radio:X}");

                    int disableResult = BluetoothSetServiceState(
                        radio,
                        ref deviceInfo,
                        ref AudioSinkServiceClassId,
                        BLUETOOTH_SERVICE_DISABLE
                    );

                    if (disableResult == 0)
                    {
                        logger.LogInformation("Audio profile successfully disabled");
                    }
                    else
                    {
                        int error = Marshal.GetLastWin32Error();
                        throw new BluetoothException(error, "Error disabling audio profile");
                    }

                    Thread.Sleep(1000);

                    int enableResult = BluetoothSetServiceState(
                        radio,
                        ref deviceInfo,
                        ref AudioSinkServiceClassId,
                        BLUETOOTH_SERVICE_ENABLE
                    );

                    if (enableResult == 0)
                    {
                        logger.LogInformation("Audio profile successfully enabled");
                        break;
                    }
                    else
                    {
                        int error = Marshal.GetLastWin32Error();
                        throw new BluetoothException(error, $"Error enabling profile");
                    }
                }
            }
            finally
            {
                if (findHandle != nint.Zero)
                {
                    if (!BluetoothFindRadioClose(findHandle))
                    {
                        int error = Marshal.GetLastWin32Error();
                        throw new BluetoothException(error, "Error closing radio search handle");
                    }
                }

                foreach (var radio in radios)
                {
                    if (radio != nint.Zero)
                    {
                        if (!CloseHandle(radio))
                        {
                            int error = Marshal.GetLastWin32Error();
                            throw new BluetoothException(error, "Error closing radio adapter");   
                        }
                    }
                }
            }
        }
    }
}
