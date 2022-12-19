using System.Collections.Generic;
using System.Diagnostics;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.Legacy.v1_3.Models;
using JoyMapper.Services.Data;
using Newtonsoft.Json;

namespace JoyMapper.Models.Legacy.v1_3;


internal class Data
{
    public AppSettings AppSettings { get; set; } = new();

    public List<Models.Profile> Profiles { get; set; } = new();

    public List<KeyPattern> KeyPatterns { get; set; } = new();

}


/// <summary> Перевод настроек версии 1.3 к 1.4 </summary>
public static class SettingsMapper
{
    public static DataManager.Data GetNewVersionData(string oldVersionJsonString)
    {
        // Исправление пространств имён
        oldVersionJsonString = oldVersionJsonString.Replace("JoyMapper.Models.JoyActions", "JoyMapper.Models.Legacy.v1_3.Models");

        var oldData = JsonConvert.DeserializeObject<Data>(oldVersionJsonString);
        if (oldData == null)
        {
            Debug.WriteLine("Не удалось конвертировать настройки");
            return null;
        }

        var newData = new DataManager.Data();

        foreach (var oldDataProfile in oldData.Profiles)
            newData.Profiles.Add(new Profile
            {
                Id = oldDataProfile.Id,
                Name = oldDataProfile.Name,
                PatternsIds = oldDataProfile.KeyPatternsIds,
            });

        foreach (var oldDataKeyPattern in oldData.KeyPatterns)
        {
            var oldJoyAction = oldDataKeyPattern.JoyAction;

            JoyBindingBase newBinding;

            if(oldJoyAction is )

            newData.JoyPatterns.Add(new JoyPattern()
            {
                Id = oldDataKeyPattern.Id,
                Name = oldDataKeyPattern.Name,
                Binding = oldDataKeyPattern.
            });}


    }
}