using Netlimiter_Alternative.Limit;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Netlimiter_Alternative
{
    internal class ConfigurationServices
    {
        public KeyModifiers modifier = KeyModifiers.Control;
        public FilterModel[] filters = {new FilterModel(3074, 1, "4", false), new FilterModel(30000, 1, "5", false), new FilterModel(0, 800, "6", false)};
        public string appPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Destiny 2\\destiny2.exe";


        public static ConfigurationServices Load()
        {
            try
            {
                return JsonConvert.DeserializeObject<ConfigurationServices>(File.ReadAllText("filters.json"));
            }
            catch (Exception)
            {
                ConfigurationServices config = new ConfigurationServices();
                File.WriteAllText("filters.json", JsonConvert.SerializeObject(config));
                return config;
            }
        }

        public void save()
        {
            File.WriteAllText("filters.json", JsonConvert.SerializeObject(this));
        }
    }
}
