using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using JoyMapper.Models;

namespace JoyMapper.Services.Data;

/// <summary>
/// Загрузка и сохранение профилей
/// </summary>
public class DataManager
{
    private readonly string _SettingsFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");

    private readonly JoyPatternManager _JoyPatternManager;
    private readonly ProfilesManager _ProfilesManager;
    private readonly ModificatorManager _ModificatorManager;
    private readonly DataSerializer _DataSerializer;



    private Data _ProfilesData;
    private Data ProfilesData => _ProfilesData ??= LoadData();

    public IEnumerable<Profile> Profiles => ProfilesData.Profiles.OrderBy(p => p.Id);
    public IEnumerable<JoyPattern> JoyPatterns => ProfilesData.JoyPatterns.OrderBy(p => p.Id);
    public IEnumerable<Modificator> Modificators => ProfilesData.Modificators.OrderBy(m => m.Id);

    public AppSettings AppSettings => ProfilesData.AppSettings;

    public DataManager(JoyPatternManager JoyPatternManager, ProfilesManager ProfilesManager, ModificatorManager ModificatorManager, DataSerializer DataSerializer)
    {
        _JoyPatternManager = JoyPatternManager;
        _ProfilesManager = ProfilesManager;
        _ModificatorManager = ModificatorManager;
        _DataSerializer = DataSerializer;
    }

    #region Public Methods


    /// <summary> Добавить профиль </summary>
    public Profile AddProfile()
    {
        var profile = _ProfilesManager.AddProfile();

        if (profile == null)
            return null;

        SaveProfile(profile);
        return profile;
    }


    /// <summary>
    /// Копировать профиль
    /// </summary>
    /// <param name="id">ID копируемого профиля</param>
    /// <returns>Скопированный профиль</returns>
    public Profile CopyProfile(int id)
    {
        var oldProf = Profiles.First(p => p.Id == id);
        var newProfile = _ProfilesManager.CopyProfile(oldProf);
        SaveProfile(newProfile);

        return newProfile;
    }



    /// <summary> Обновить паттерн (заменить по Id) </summary>
    public Profile UpdateProfile(int Id)
    {
        var editProfile = ProfilesData.Profiles.First(p => p.Id == Id);

        var updated = _ProfilesManager.UpdateProfile(editProfile);
        if (updated == null) return null;


        var index = ProfilesData.Profiles.IndexOf(editProfile);
        ProfilesData.Profiles.Remove(editProfile);
        ProfilesData.Profiles.Insert(index, updated);
        SaveData();

        return updated;
    }


    /// <summary> Удалить профиль </summary>
    public void RemoveProfile(int profileId)
    {
        ProfilesData.Profiles.Remove(ProfilesData.Profiles.FirstOrDefault(p => p.Id == profileId));
        SaveData();
    }


    /// <summary> Добавить паттерн </summary>
    public JoyPattern AddJoyPattern()
    {
        var pattern = _JoyPatternManager.AddPattern();
        if (pattern is null)
            return null;

        SavePattern(pattern);
        return pattern;
    }


    /// <summary>
    /// Копировать паттерн
    /// </summary>
    /// <param name="id">ID копируемого паттерна</param>
    /// <returns>Скопированный паттерн</returns>
    public JoyPattern CopyJoyPattern(int id)
    {
        var oldPattern = JoyPatterns.First(p => p.Id == id);

        var newPattern = _JoyPatternManager.CopyPattern(oldPattern);
        if (newPattern is null)
            return null;

        SavePattern(newPattern);
        return newPattern;
    }


    /// <summary> Обновить паттерн (заменить по Id) </summary>
    public JoyPattern UpdateJoyPattern(int Id)
    {
        var editPattern = ProfilesData.JoyPatterns.First(p => p.Id == Id);

        var updated = _JoyPatternManager.EditPattern(editPattern);

        if (updated is null)
            return null;

        var index = ProfilesData.JoyPatterns.IndexOf(editPattern);
        ProfilesData.JoyPatterns.Remove(editPattern);
        ProfilesData.JoyPatterns.Insert(index, updated);
        SaveData();
        return updated;
    }


    /// <summary> Удалить паттерн </summary>
    public void RemoveJoyPattern(int patternId)
    {
        ProfilesData.JoyPatterns.Remove(ProfilesData.JoyPatterns.First(p => p.Id == patternId));

        foreach (var profile in Profiles)
            profile.PatternsIds.Remove(patternId);

        SaveData();
    }


    /// <summary> Добавить модификатор </summary>
    public Modificator AddModificator()
    {
        var modificator = _ModificatorManager.AddModificator();
        if (modificator is null)
            return null;

        var nextId = Modificators
            .Select(p => p.Id)
            .DefaultIfEmpty()
            .Max() + 1;

        modificator.Id = nextId;
        ProfilesData.Modificators.Add(modificator);
        SaveData();
        return modificator;
    }

    /// <summary> Удалить модификатор </summary>
    public void RemoveModificator(int modId)
    {
        ProfilesData.Modificators.Remove(ProfilesData.Modificators.FirstOrDefault(p => p.Id == modId));

        foreach (var pattern in JoyPatterns)
            if (pattern.ModificatorId == modId)
                pattern.ModificatorId = null;

        SaveData();
    }

    /// <summary> Обновить модификатор </summary>
    public Modificator UpdateModificator(int Id)
    {
        var editModificator = ProfilesData.Modificators.First(m => m.Id == Id);
        var modified = _ModificatorManager.UpdateModificator(editModificator);
        if (modified is null) 
            return null;
        var index = ProfilesData.Modificators.IndexOf(editModificator);
        ProfilesData.Modificators.Remove(editModificator);
        ProfilesData.Modificators.Insert(index, modified);
        SaveData();
        return modified;
    }


    /// <summary> Сохранить профили и настройки </summary>
    public void SaveData() => _DataSerializer.SaveToFile(ProfilesData, _SettingsFileName);

    #endregion

    private Data LoadData()
    {
        if(!File.Exists(_SettingsFileName))
            return new Data();

        var appSettVersion = _DataSerializer.LoadFromFile<AppSettings>(_SettingsFileName)?.AppVersion;
        if (!Equals(App.AppVersion, appSettVersion)) // Бекап настроек
            File.Copy(_SettingsFileName, Path.Combine(Environment.CurrentDirectory, $"Config-{appSettVersion}-backup.json"), true);

        var loadFromFile = _DataSerializer.LoadFromFile<Data>(_SettingsFileName);

        if (loadFromFile is not null)
            return loadFromFile;

        var failFileName = $"ConfigLoadFail-{DateTime.Now:dd-MM-yyyy:hh-mm-ss}.json";
        File.Copy(_SettingsFileName, Path.Combine(Environment.CurrentDirectory, failFileName), true);
        MessageBox.Show($"Бекап сохранён в файл {failFileName}", "Ошибка загрузки настроек");

        return new Data();
    }

    private void SaveProfile(Profile profile)
    {
        var nextId = Profiles
            .Select(p => p.Id)
            .DefaultIfEmpty()
            .Max() + 1;

        profile.Id = nextId;
        ProfilesData.Profiles.Add(profile);
        SaveData();
    }

    private void SavePattern(JoyPattern pattern)
    {
        var nextId = JoyPatterns
            .Select(p => p.Id)
            .DefaultIfEmpty()
            .Max() + 1;

        pattern.Id = nextId;
        ProfilesData.JoyPatterns.Add(pattern);
        SaveData();
    }

    /// <summary>
    /// Данные для сериализации
    /// </summary>
    public class Data
    {
        public AppSettings AppSettings { get; set; } = new();

        public List<Profile> Profiles { get; set; } = new();

        public List<JoyPattern> JoyPatterns { get; set; } = new();

        public List<Modificator> Modificators { get; set; } = new();
    }
}