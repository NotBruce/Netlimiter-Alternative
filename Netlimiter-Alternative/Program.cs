using NetLimiter.Service;
using Netlimiter_Alternative.Limit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlimiter_Alternative
{
    internal class Program
    {
        public static ConfigurationServices config = ConfigurationServices.Load();
        public static NLClient client = new NLClient();
        public static List<VFilter> filters = new List<VFilter>();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += Shutdown;
            // Connecting to Netlimiter 4 Pro
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            Console.WriteLine("Adding Filters");
            foreach (FilterModel filt in config.filters)
            {
                filters.Add(new VFilter(client, ((filt.isOutbound == true) ? RuleDir.Out : RuleDir.In), filt.port, filt.bytes, filt));
                Console.WriteLine(filt.port + " Added");
            }

            Console.Clear();
            Console.WriteLine("Limiter Options: ");
            Console.WriteLine("Kill Connections - " + config.modifier.ToString() + " + 0");
            foreach (VFilter filt in filters)
            {
                if (filt.port == 0)
                {
                    Console.WriteLine("Destiny 2" + ((filt.ruleDir.ToString() == "Out") ? " UL" : " DL") + " [" + ((filt.rule.IsEnabled == true) ? "On]" : "Off]") + " - CTRL + " + filt.filterModel.getKeyFromString().ToString());

                }
                else
                {
                    Console.WriteLine(filt.port.ToString() + ((filt.ruleDir.ToString() == "Out") ? " UL" : " DL") + " [" + ((filt.rule.IsEnabled == true) ? "On]" : "Off]") + " - CTRL + " + filt.filterModel.getKeyFromString().ToString());
                }
            }

            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>((object sender, HotKeyEventArgs e) =>
            {
                Console.Clear();
                Console.WriteLine("Limiter Options: ");
                Console.WriteLine("Kill Connections - " + config.modifier.ToString() + " + 0");
                foreach (VFilter filt in filters)
                {
                    if (e.Key == System.Windows.Forms.Keys.D0 && e.Modifiers == config.modifier && filt.port == 30000)
                    {
                        filt.killConnections();
                    }

                    if (e.Key == filt.filterModel.getKeyFromString() && e.Modifiers == config.modifier)
                    {
                        filt.rule.IsEnabled = !filt.rule.IsEnabled;
                        client.UpdateRule(filt.rule);
                    }

                    if (filt.port == 0)
                    {
                        Console.WriteLine("Destiny 2" + ((filt.ruleDir.ToString() == "Out") ? " UL" : " DL") + " [" + ((filt.rule.IsEnabled == true) ? "On]" : "Off]") + " - CTRL + " + filt.filterModel.getKeyFromString().ToString());

                    }
                    else
                    {
                        Console.WriteLine(filt.port.ToString() + ((filt.ruleDir.ToString() == "Out") ? " UL" : " DL") + " [" + ((filt.rule.IsEnabled == true) ? "On]" : "Off]") + " - CTRL + " + filt.filterModel.getKeyFromString().ToString());
                    }
                }
            });

            Console.ReadLine();
        }

        static void Shutdown(object sender, ConsoleCancelEventArgs e)
        {
            foreach (VFilter filt in filters)
            {
                filt.rule.IsEnabled = false;
                client.UpdateRule(filt.rule);
            }
        }
    }
}
