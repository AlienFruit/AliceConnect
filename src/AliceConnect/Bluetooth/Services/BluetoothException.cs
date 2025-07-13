using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliceConnect.Bluetooth.Services
{
    internal class BluetoothException(int code, string message) : Exception($"Win32Error: {code}. Messge: {message}")
    {
    }
}
