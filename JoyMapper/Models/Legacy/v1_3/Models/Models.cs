using System;
using System.Collections.Generic;

namespace JoyMapper.Models.Legacy.v1_3.Models;

internal class Profile
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<int> KeyPatternsIds { get; set; } = new();
}

internal class KeyPattern
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string JoyName { get; set; }

    public JoyActionBase JoyAction { get; set; }

}

public enum ButtonType
{
    Button,
    Pow1,
    Pow2,
}


internal class JoyButton
{
    public ButtonType Type { get; set; }

    public int Value { get; set; }


    public override string ToString()
    {
        var txt = Type switch
        {
            ButtonType.Button => "Кнопка " + Value,
            ButtonType.Pow1 => "Переключатель вида №1 " + GetPowPoint(Value),
            ButtonType.Pow2 => "Переключатель вида №2 " + GetPowPoint(Value),
            _ => throw new ArgumentOutOfRangeException()
        };
        return txt;
    }

    private static string GetPowPoint(int rawValue) =>
        rawValue switch
        {
            0 => "Вверх",
            4500 => "Вверх-Вправо",
            9000 => "Вправо",
            13500 => "Вниз-Вправо",
            18000 => "Вниз",
            22500 => "Вниз-Влево",
            27000 => "Влево",
            31500 => "Вверх-Влево",
            _ => "None"
        };
}