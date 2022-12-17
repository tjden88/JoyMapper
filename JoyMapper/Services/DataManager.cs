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
            public AppSettings AppSettings { get; set; } = new();

            public List<Profile> Profiles { get; set; } = new();

            public List<JoyPattern> JoyPatterns { get; set; } = new();
            public List<Modificator> Modificators { get; set; } = new();

        }

        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Config.json");

        private Data _ProfilesData;
        private Data ProfilesData => _ProfilesData ??= LoadData();

        public IEnumerable<Profile> Profiles => ProfilesData.Profiles.OrderBy(p => p.Id);
        public IEnumerable<JoyPattern> JoyPatterns => ProfilesData.JoyPatterns.OrderBy(p => p.Id);
        public IEnumerable<Modificator> Modificators => ProfilesData.Modificators.OrderBy(m => m.Id);

        public AppSettings AppSettings => ProfilesData.AppSettings;


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


        /// <summary>
        /// Копировать профиль
        /// </summary>
        /// <param name="id">ID копируемого профиля</param>
        /// <returns>Скопированный профиль</returns>
        public Profile CopyProfile(int id)
        {
            var oldProf = Profiles.First(p => p.Id == id);

            var newProfile = _DataSerializer.CopyObject(oldProf);
            AddProfile(newProfile);
            return newProfile;
        }



        /// <summary> Обновить паттерн (заменить по Id) </summary>
        public void UpdateProfile(Profile profile)
        {
            var editProfile = ProfilesData.Profiles.FirstOrDefault(p => p.Id == profile.Id);
            if (editProfile != null)
            {
                var index = ProfilesData.Profiles.IndexOf(editProfile);
                ProfilesData.Profiles.Remove(editProfile);
                ProfilesData.Profiles.Insert(index, profile);
                SaveData();
            }
        }


        /// <summary> Удалить профиль </summary>
        public void RemoveProfile(int profileId)
        {
            ProfilesData.Profiles.Remove(ProfilesData.Profiles.FirstOrDefault(p => p.Id == profileId));
            SaveData();
        }


        /// <summary> Добавить паттерн </summary>
        public void AddKeyPattern(JoyPattern JoyPattern)
        {
            var nextId = JoyPatterns
                .Select(p => p.Id)
                .DefaultIfEmpty()
                .Max() + 1;

            JoyPattern.Id = nextId;
            ProfilesData.JoyPatterns.Add(JoyPattern);
            SaveData();
        }


        /// <summary>
        /// Копировать паттерн
        /// </summary>
        /// <param name="id">ID копируемого паттерна</param>
        /// <returns>Скопированный паттерн</returns>
        public JoyPattern CopyKeyPattern(int id)
        {
            var oldPattern = JoyPatterns.First(p => p.Id == id);

            var newPattern = _DataSerializer.CopyObject(oldPattern);
            AddKeyPattern(newPattern);
            return newPattern;
        }


        /// <summary> Обновить паттерн (заменить по Id) </summary>
        public void UpdateKeyPattern(JoyPattern pattern)
        {
            var editPattern = ProfilesData.JoyPatterns.FirstOrDefault(p => p.Id == pattern.Id);
            if (editPattern == null) return;
            var index = ProfilesData.JoyPatterns.IndexOf(editPattern);
            ProfilesData.JoyPatterns.Remove(editPattern);
            ProfilesData.JoyPatterns.Insert(index, pattern);
            SaveData();
        }


        /// <summary> Удалить паттерн </summary>
        public void RemoveKeyPattern(int patternId)
        {
            ProfilesData.JoyPatterns.Remove(ProfilesData.JoyPatterns.FirstOrDefault(p => p.Id == patternId));

            foreach (var profile in Profiles)
            {
                profile.PatternsIds.Remove(patternId);
            }

            SaveData();
        }


        public void AddModificator(Modificator modificator)
        {
            var nextId = Modificators
                .Select(p => p.Id)
                .DefaultIfEmpty()
                .Max() + 1;

            modificator.Id = nextId;
            ProfilesData.Modificators.Add(modificator);
            SaveData();

        }

        //public void RemoveModificator(int modId)
        //{
        //    ProfilesData.Modificators.Remove(ProfilesData.Modificators.FirstOrDefault(p => p.Id == modId));

        //    foreach (var pattern in JoyPatterns)
        //    {
        //        if (pattern.Modificator?.Id == modId)
        //        {
        //            pattern.Modificator = null;
        //        }
        //    }

        //    SaveData();
        //}

        public void UpdateModificator(Modificator modificator)
        {
            var editModificator = ProfilesData.Modificators.FirstOrDefault(m => m.Id == modificator.Id);
            if (editModificator == null) return;
            var index = ProfilesData.Modificators.IndexOf(editModificator);
            ProfilesData.Modificators.Remove(editModificator);
            ProfilesData.Modificators.Insert(index, modificator);
            SaveData();

        }


        /// <summary> Сохранить профили и настройки </summary>
        public void SaveData() => _DataSerializer.SaveToFile(ProfilesData, _SettingsFileName);

        #endregion

        private Data LoadData()
        {
            var appSettVersion = _DataSerializer.LoadFromFile<AppSettings>(_SettingsFileName)?.AppVersion;
            if (!Equals(App.AppVersion, appSettVersion) && File.Exists(_SettingsFileName)) // Бекап настроек
                File.Copy(_SettingsFileName, Path.Combine(Environment.CurrentDirectory, $"Config-{appSettVersion}-backup.json"), true);

            return _DataSerializer.LoadFromFile<Data>(_SettingsFileName) ?? new Data();
        }
    }
}
