using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Infrastructure
{
    /// <summary>
    /// Класс хранения настроек программы
    /// </summary>
    public class ConfigurationData
    {
        public static readonly ConfigurationData Data = new ConfigurationData();

        private ConfigurationData() { }

        Configuration config = ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

        public string GetData(string key, string defaultValue = "")
        {
            return config.AppSettings.Settings[key]?.Value ?? defaultValue;
        }

        public void SetData(string key, string value)
        {
            if(config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);

        }

        public bool SecondStart()
        {
            if(config.AppSettings.Settings[nameof(SecondStart)] == null)
            {
                config.AppSettings.Settings.Add(nameof(SecondStart),"True");
                return false;
            }
            return true;
        }

        public void SaveData() => config.Save();
    }
}
