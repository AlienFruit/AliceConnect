import sys
import asyncio
import random
from bleak import BleakClient

address = "FF:FF:11:38:5D:0E"
# handle = 0x000D
# value = 'bc01010155'

commands = [
    ("POWER_OFF", 0x000D, 'bc01010055'),
    ("POWER_ON", 0x000D, 'bc01010155')
]

def get_command_parameters_iterative(command_name):
    """
    Returns command code and data; raises ValueError if command not found.
    """
    for name, handle, value in commands:
        if name == command_name:
            return handle, value
    raise ValueError(f"Command '{command_name}' not found.")
   

async def write_command(client, command):
    handle, value = get_command_parameters_iterative(command)
    value_bytes = bytes.fromhex(value)
    await client.write_gatt_char(handle, value_bytes)
    print(f"Wrote {value_bytes.hex()} to handle 0x{handle:04X}")

async def main(command, max_retries=10, initial_delay=1):
    delay = initial_delay
    for attempt in range(1, max_retries + 1):
        client = BleakClient(address)
        try:
            print(f"Attempt {attempt}/{max_retries} connecting to {address}...")
            await client.connect()
            print(f"Connected to {address}")
            await write_command(client, command)
            await client.disconnect()
            return  # Exit successfully

        except Exception as e:
            print(f"Attempt {attempt} failed: {e}")
            await client.disconnect()
            if attempt < max_retries:
                print(f"Retrying in {delay} seconds...")
                await asyncio.sleep(delay + random.uniform(0, delay)) # Add some jitter
                #delay *= 2  # Exponential backoff
    print(f"Failed to connect after {max_retries} attempts.")


asyncio.run(main(sys.argv[1]))