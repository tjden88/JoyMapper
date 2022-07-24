using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using JoyMapper.Models;

namespace JoyMapper.Services
{
    /// <summary>
    /// Загрузка и сохранение профилей
    /// </summary>
    internal class DataManager
    {
        private class Data
        {
            public List<Profile> Profiles { get; set; } = new();

            public List<KeyPattern> KeyPatterns { get; set; } = new();
        }

        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Config.json");

        private Data _ProfilesData;
        private Data ProfilesData => _ProfilesData ??= LoadData();

        public List<Profile> Profiles => ProfilesData.Profiles;
        public List<KeyPattern> KeyPatterns => ProfilesData.KeyPatterns;


        private Data LoadData()
        {
            if (!File.Exists(_SettingsFileName))
                return new Data();
            try
            {
                return JsonSerializer.Deserialize<Data>(_SettingsFileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка загрузки данных");
                return new Data();
            }
        }

        public void SaveData()
        {
            try
            {
                var data = new Data
                {
                    KeyPatterns = KeyPatterns,
                    Profiles = Profiles
                };
                var serialized = JsonSerializer.Serialize(data);
                File.WriteAllText(_SettingsFileName, serialized);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка при сохраниении данных");
            }
        }

    }
}
