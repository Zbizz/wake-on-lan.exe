using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WakeOnLan.exe
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!GetSettings())
                Environment.Exit(0);

            if (!ValidateArguments(args))
                Environment.Exit(0);

            string table = 
                (args.Length == 1) ? args[0] : ConfigurationManager.AppSettings["DatabaseTable"].ToString();            

            bool success;

            List<string> macs = Machines.GetMacs(table, out success);

            if (!success)
            {
                Log.Write("Failed to connect to database.", false);
                Environment.Exit(0);
            }

            foreach (string mac in macs)
            {
                ValidateMac(mac, out success);

                if (success)
                {
                    byte[] bytes = GetMacBytes(mac);

                    try
                    {
                        Power.WakeUp(bytes);

                        Log.Write("Wake up packets sent to " + mac, true);
                    }
                    catch
                    {
                        Log.Write("Failed to send wake up packets to " + mac, false);
                    }
                }
            }

            Environment.Exit(0);
        }

        protected static bool ValidateArguments(string[] args)
        {
            if (args.Length > 1)
            {
                Log.Write("Too many arguments.", false);
                return false;
            }

            return true;
        }

        protected static bool GetSettings()
        {
            string settings = "Settings.config";

            string file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + settings;

            if (!File.Exists(file))
            {
                Log.Write("Unable to load settings file.", false);

                return false;
            }
            else
            {               
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.File = settings;
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                return true;
            }
        }

        public static void ValidateMac(string mac, out bool success)
        {
            success = true;

            if (String.IsNullOrEmpty(mac))
            {
                Log.Write("MAC address is null.", false);
                success = false;
            }

            Regex macCheck = new Regex(@"([\dA-Fa-f]{2})(:[A-Fa-f\d]{2}){5}");

            if (!macCheck.IsMatch(mac))
            {
                Log.Write(mac + " is not a valid MAC address.", false);
                success = false;
            }
        }

        public static byte[] GetMacBytes(string mac)
        {
            string[] byteStrings = mac.Split(':');

            byte[] bytes = new byte[6];

            for (int i = 0; i < 6; i++)
                bytes[i] = (byte)Int32.Parse(byteStrings[i], System.Globalization.NumberStyles.AllowHexSpecifier);

            return bytes;
        }
    }
}
