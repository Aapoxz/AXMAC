using System;
using System.Linq;
using System.Management;

namespace axMac
{
    // Coded for learning purposes
    // AUTHOR: Aapoxz
    // DO NOT STEAL!
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "AXMAC - Made by Aapoxz";
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(" ▄▄▄      ▒██   ██▒ ███▄ ▄███▓ ▄▄▄       ▄████▄  \r\n▒████▄    ▒▒ █ █ ▒░▓██▒▀█▀ ██▒▒████▄    ▒██▀ ▀█  \r\n▒██  ▀█▄  ░░  █   ░▓██    ▓██░▒██  ▀█▄  ▒▓█    ▄ \r\n░██▄▄▄▄██  ░ █ █ ▒ ▒██    ▒██ ░██▄▄▄▄██ ▒▓▓▄ ▄██▒\r\n ▓█   ▓██▒▒██▒ ▒██▒▒██▒   ░██▒ ▓█   ▓██▒▒ ▓███▀ ░\r\n ▒▒   ▓▒█░▒▒ ░ ░▓ ░░ ▒░   ░  ░ ▒▒   ▓▒█░░ ░▒ ▒  ░\r\n  ▒   ▒▒ ░░░   ░▒ ░░  ░      ░  ▒   ▒▒ ░  ░  ▒   \r\n  ░   ▒    ░    ░  ░      ░     ░   ▒   ░        \r\n      ░  ░ ░    ░         ░         ░  ░░ ░      \r\n                                        ░        ");
            Console.ForegroundColor = ConsoleColor.White;
       
            Console.WriteLine("1. Change MAC address (ADVANCED) ");
            Console.WriteLine("2. Change MAC address (FAST)");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option:");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                string newMacAddress = GenerateRandomMacAddress();
                ChangeMacAddress(newMacAddress);
            }
            if (choice == "2")
            {
                string newMacAddress = GenerateRandomMacAddress();
            
                FastChangeMacAddress(newMacAddress);
            }
            if (choice == "3")
            {
                Environment.Exit(0);    
            }
        }
        static string GenerateRandomMacAddress()
        {
            Random rand = new Random();
            byte[] macAddr = new byte[6];
            rand.NextBytes(macAddr);
            // Set the local admin bit. The second least significant bit of the first byte
            macAddr[0] = (byte)(macAddr[0] & (byte)254);
            // Ensure it is not multicast
            macAddr[0] = (byte)(macAddr[0] | (byte)2);

            return string.Join(":", macAddr.Select(b => b.ToString("X2")));
        }

        static void ChangeMacAddress(string newMacAddress)
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID != NULL");
                var adapters = searcher.Get().Cast<ManagementObject>();

                foreach (var adapter in adapters)
                {
                    Console.WriteLine($"Adapter: {adapter["NetConnectionID"]}");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter the name of the adapter to change its MAC address: ");
                string adapterName = Console.ReadLine();

                var selectedAdapter = adapters.FirstOrDefault(a => a["NetConnectionID"].ToString() == adapterName);

                if (selectedAdapter != null)
                {
                    selectedAdapter.InvokeMethod("Disable", null);
                    selectedAdapter["MACAddress"] = newMacAddress.Replace(":", "");
                    selectedAdapter.Put();
                    selectedAdapter.InvokeMethod("Enable", null);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"MAC address changed.\n");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("! WARNING !");
                    Console.WriteLine("You may need to restart your system for changes to take effect.");
                    Console.ReadLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Adapter not found.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        static void FastChangeMacAddress(string newMacAddress)
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionID != NULL");
                var adapters = searcher.Get().Cast<ManagementObject>();

                var firstAdapter = adapters.FirstOrDefault();

                if (firstAdapter != null)
                {
                    Console.WriteLine($"");
                    firstAdapter.InvokeMethod("Disable", null);
                    firstAdapter["MACAddress"] = newMacAddress.Replace(":", "");
                    firstAdapter.Put();
                    firstAdapter.InvokeMethod("Enable", null);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"MAC address changed.");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("! WARNING !");
                    Console.WriteLine("You may need to restart your system for changes to take effect.");
                    Console.ReadLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("No adapters found.");
                    Console.ReadLine();    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
