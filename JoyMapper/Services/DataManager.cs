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
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Config.json");

        private List<Profile> _Profiles ;

        public List<Profile> Profiles => _Profiles ??= LoadProfiles();


        public List<Profile> LoadProfiles()
        {
            if (!File.Exists(_SettingsFileName))
                return new List<Profile>();
            try
            {
                return JsonSerializer.Deserialize<List<Profile>>(_SettingsFileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка загрузки профилей");
                return new List<Profile>();
            }
        }

        public void SaveProfiles()
        {
            try
            {
                var serialized = JsonSerializer.Serialize(Profiles);
                File.WriteAllText(_SettingsFileName, serialized);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка при сохраниении профилей");
            }
        }

    }
}
