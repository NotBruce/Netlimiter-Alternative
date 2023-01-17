using Microsoft.Win32;
using NetLimiter.Service;
using Netlimiter_Alternative.Limit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netlimiter_Alternative
{
    internal class Program
    {
        public static ConfigurationServices config = ConfigurationServices.Load();
        public static NLClient client = new NLClient();
        public static List<VFilter> filters = new List<VFilter>();

        static void Main(string[] args)
        {
            // Connecting to Netlimiter 4 Pro
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            // Feature doesn't work fulyl at the moment, but should be able to find app path.
            string[] appPath = checkInstalled("Destiny 2");
            if (appPath.Length != 0)
            {
                Console.WriteLine("Pick Which App Path [1/" + appPath.Length + "]:");
                for (int i = 0; i < appPath.Length; i++)
                {
                    Console.WriteLine(" » [" + i + "] " + appPath[i] + "\\destiny2.exe");
                }
                int value = Convert.ToInt32(Console.ReadLine());
                config.appPath = appPath[value] + "\\destiny2.exe";
                config.save();
                Console.WriteLine("App Path Found: " + appPath[value] + "\\destiny2.exe");
            }

            foreach (FilterModel filt in config.filters)
            {
                filters.Add(new VFilter(client, ((filt.isOutbound == true) ? RuleDir.Out : RuleDir.In), filt.port, filt.bytes, filt));
            }

            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.D0, config.modifier);

            Console.Clear();
            Console.WriteLine("Kill Connections - " + config.modifier.ToString() + " + 0");
            foreach (VFilter filt in filters)
            {
                Console.WriteLine(filt.filterName + " [" + ((filt.rule.IsEnabled == true) ? "On]" : "Off]") + " - CTRL + " + filt.filterModel.getKeyFromString().ToString());
            }

            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>((object sender, HotKeyEventArgs e) =>
            {
                Console.Clear();
                Console.WriteLine("Kill Connections - " + config.modifier.ToString() + " + 0");
                foreach (VFilter filt in filters)
                {
                    if (e.Key == filt.filterModel.getKeyFromString() && e.Modifiers == config.modifier)
                    {
                        filt.rule.IsEnabled = !filt.rule.IsEnabled;
                        client.UpdateRule(filt.rule);
                    }

                    if (e.Key == System.Windows.Forms.Keys.D0 && e.Modifiers == config.modifier && filt.port == 30000)
                    {
                        filt.killConnections();
                    }

                    Console.WriteLine(filt.filterName + " [" + ((filt.rule.IsEnabled == true) ? "On]" : "Off]") + " - CTRL + " + filt.filterModel.getKeyFromString().ToString());
                }
            });

            Console.ReadLine();
        }


        // Credits: https://stackoverflow.com/questions/909910/how-to-find-the-execution-path-of-a-installed-software
        public static string[] checkInstalled(string findByName)
        {
            string displayName;
            string InstallPath;
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            //64 bits computer
            RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey key = key64.OpenSubKey(registryKey);

            string[] locations = {};

            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(findByName))
                    {

                        InstallPath = subkey.GetValue("InstallLocation").ToString();

                        locations = locations.Append(InstallPath).ToArray();
                        //return InstallPath; //or displayName

                    }
                }
                key.Close();
            }

            return locations;
        }
    }
}
