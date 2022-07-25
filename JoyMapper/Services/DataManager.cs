using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JoyMapper.Models;

namespace JoyMapper.Services
{
    /// <summary>
    /// Загрузка и сохранение профилей
    /// </summary>
    internal class DataManager
    {

        private readonly DataSerializer _DataSerializer = new();

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

        /// <summary> Добавить профиль </summary>
        public void AddProfile(Profile profile)
        {
            var nextId = Profiles
                .Select(p => p.Id)
                .DefaultIfEmpty()
                .Max() + 1;

            profile.Id = nextId;
            ProfilesData.Profiles.Add(profile);
            SaveData();
        }



        /// <summary> Удалить профиль </summary>
        public void RemoveProfile(int profileId)
        {
            ProfilesData.Profiles.Remove(ProfilesData.Profiles.FirstOrDefault(p => p.Id == profileId));
            SaveData();
        }


        /// <summary> Добавить паттерн </summary>
        public void AddKeyPattern(KeyPattern keyPattern)
        {
            var nextId = KeyPatterns
                .Select(p => p.Id)
                .DefaultIfEmpty()
                .Max() + 1;

            keyPattern.Id = nextId;
            ProfilesData.KeyPatterns.Add(keyPattern);
            SaveData();
        }


        /// <summary> Обновить паттерн (заменить по Id) </summary>
        public void UpdateKeyPattern(KeyPattern pattern)
        {
            var editPattern = ProfilesData.KeyPatterns.FirstOrDefault(p => p.Id == pattern.Id);
            if (editPattern != null)
            {
                var index = ProfilesData.KeyPatterns.IndexOf(editPattern);
                ProfilesData.KeyPatterns.Remove(editPattern);
                ProfilesData.KeyPatterns.Insert(index, pattern);
                SaveData();
            }
        }


        /// <summary> Удалить паттерн </summary>
        public void RemoveKeyPattern(int patternId)
        {
            ProfilesData.KeyPatterns.Remove(ProfilesData.KeyPatterns.FirstOrDefault(p => p.Id == patternId));

            foreach (var profile in Profiles)
            { 
                profile.KeyPatternsIds.Remove(patternId);
            }

            SaveData();
        }

        #endregion

        private Data LoadData() => _DataSerializer.LoadFromFile<Data>(_SettingsFileName);

        private void SaveData() => _DataSerializer.SaveToFile(ProfilesData, _SettingsFileName);
    }
}
