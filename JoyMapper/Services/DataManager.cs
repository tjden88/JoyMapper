using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public IEnumerable<Profile> Profiles => ProfilesData.Profiles;
        public IEnumerable<KeyPattern> KeyPatterns => ProfilesData.KeyPatterns;



        #region Public Methods

        public void AddProfile(Profile profile)
        {
            var nextId = Profiles.Max(p => p.Id) + 1;
            profile.Id = nextId;
            ProfilesData.Profiles.Add(profile);
            SaveData();
        }

        public void AddKeyPattern(KeyPattern keyPattern)
        {
            var nextId = KeyPatterns.Max(p => p.Id) + 1;
            keyPattern.Id = nextId;
            ProfilesData.KeyPatterns.Add(keyPattern);
            SaveData();
        }


        public void RemoveProfile(int profileId)
        {
            ProfilesData.Profiles.Remove(ProfilesData.Profiles.FirstOrDefault(p=> p.Id == profileId));
            SaveData();
        }

        public void RemovePattern(int patternId)
        {
            ProfilesData.KeyPatterns.Remove(ProfilesData.KeyPatterns.FirstOrDefault(p => p.Id == patternId));

            foreach (var profile in Profiles)
            {
                var pattern = profile.KeyPatterns.Find(p => p.Id == patternId);
                if (pattern != null) profile.KeyPatterns.Remove(pattern);
            }

            SaveData();
        }

        #endregion

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

        private void SaveData()
        {
            try
            {
                var data = new Data
                {
                    KeyPatterns = KeyPatterns.ToList(),
                    Profiles = Profiles.ToList()
                };
                var serialized = JsonSerializer.Serialize(data);
                File.WriteAllText(_SettingsFileName, serialized, Encoding.UTF8);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка при сохраниении данных");
            }
        }

    }
}
