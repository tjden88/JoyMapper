using System;
using System.Collections.Generic;
using System.Diagnostics;
using JoyMapper.Models.JoyBindings;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.Legacy.v1_3.Models;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
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
            
            JoyBindingBase newBinding = new ButtonJoyBinding {JoyName = "Не определено", ButtonNumber = 1};
            PatternActionBase newAction = new SimpleKeySenderPatternAction();

            var oldJoyAction = oldDataKeyPattern.JoyAction;

            if (oldJoyAction is SimpleButtonJoyAction sb) // простая кнопка
            {
                newAction = new SimpleKeySenderPatternAction
                {
                    PressKeyBindings = sb.PressKeyBindings,
                    ReleaseKeyBindings = sb.ReleaseKeyBindings,
                };
                newBinding = GetFromButton(sb.Button, oldDataKeyPattern.JoyName);
            }
            if (oldJoyAction is ExtendedButtonJoyAction eb) // простая кнопка
            {
                newAction = new ExtendedKeySenderPatternAction
                {
                    SinglePressKeyBindings = eb.SinglePressKeyBindings,
                    DoublePressKeyBindings = eb.DoublePressKeyBindings,
                    LongPressKeyBindings = eb.LongPressKeyBindings,
                };
                newBinding = GetFromButton(eb.Button, oldDataKeyPattern.JoyName);
            }

            if (oldJoyAction is AxisJoyAction aa) // Действие оси
            {
                newAction = new SimpleKeySenderPatternAction
                {
                    PressKeyBindings = aa.OnRangeKeyBindings,
                    ReleaseKeyBindings = aa.OutOfRangeKeyBindings
                };
                newBinding = new AxisJoyBinding
                {
                    JoyName = oldDataKeyPattern.JoyName,
                    Axis = (AxisJoyBinding.Axises) aa.Axis,
                    StartValue = aa.StartValue,
                    EndValue = aa.EndValue,
                };
            }


            newData.JoyPatterns.Add(new JoyPattern()
            {
                Id = oldDataKeyPattern.Id,
                Name = oldDataKeyPattern.Name,
                Binding = newBinding,
                PatternAction = newAction
            });}

        newData.AppSettings= oldData.AppSettings;

        return newData;
    }

    private static JoyBindingBase GetFromButton(JoyButton button, string name)
    {
        return button.Type switch
        {
            ButtonType.Button => new ButtonJoyBinding
            {
                JoyName = name,
                ButtonNumber = button.Value
            },
            ButtonType.Pow1 => new PowJoyBinding
            {
                JoyName = name,
                PowNumber = PowJoyBinding.PowNumbers.Pow1,
                PowValue = button.Value
            },
            ButtonType.Pow2 => new PowJoyBinding
            {
                JoyName = name,
                PowNumber = PowJoyBinding.PowNumbers.Pow2,
                PowValue = button.Value
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }


}