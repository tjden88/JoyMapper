using System;
using System.Diagnostics.CodeAnalysis;
using SharpDX.DirectInput;

namespace JoyMapper.Models
{
    /// <summary>
    /// Действие джойстикка (кнопка, указатель, ось)
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class JoyAction
    {

        public enum StateType
        {
            Button,
            POW1,
            POW2,
            Axis
        }

        public enum POWPoint
        {
            None,
            Вверх,
            Вниз,
            Влево,
            Вправо,
            ВверхВлево,
            ВнизВлево,
            ВнизВправо,
            ВверхВправо,
        }

        public enum Axises
        {
            X,
            Y,
            Z,
            RX,
            RY,
            RZ,
            Slider1,
            Slider2,
        }

        public StateType Type { get; init; }

        public int ButtonNumber { get; set; }

        public int POWPosition { get; set; }

        public Axises Axis { get; set; }

        public int StartAxisValue { get; set; }

        public int EndAxisValue { get; set; }


        /// <summary> Проверка, активно ли в данный момент действие джойстика </summary>
        public bool IsActionActive(JoystickState state)
        {
            switch (Type)
            {
                case StateType.Button:
                    var btnValue = state.Buttons[ButtonNumber - 1];
                    return btnValue;

                case StateType.POW1:
                    return POWPosition == state.PointOfViewControllers[0];
                case StateType.POW2:
                    return POWPosition == state.PointOfViewControllers[1];

                case StateType.Axis:
                    var axisValue = Axis switch
                    {
                        Axises.X => state.X,
                        Axises.Y => state.Y,
                        Axises.Z => state.Z,
                        Axises.RX => state.RotationX,
                        Axises.RY => state.RotationY,
                        Axises.RZ => state.RotationZ,
                        Axises.Slider1 => state.Sliders[0],
                        Axises.Slider2 => state.Sliders[1],
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    return axisValue >= StartAxisValue && axisValue <= EndAxisValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Описание действия джойстика </summary>
        public string ActionText()
        {
            var txt = Type switch
            {
                StateType.Axis => "Ось " + Axis,
                StateType.Button => "Кнопка " + ButtonNumber,
                StateType.POW1 => "Переключатель вида №1 " + GetPowPoint(POWPosition),
                StateType.POW2 => "Переключатель вида №2 " + GetPowPoint(POWPosition),
                _ => throw new ArgumentOutOfRangeException()
            };
            return txt;
        }

        public static POWPoint GetPowPoint(int rawValue) =>
            rawValue switch
            {
                -1 => POWPoint.None,
                0 => POWPoint.Вверх,
                4500 => POWPoint.ВверхВправо,
                9000 => POWPoint.Вправо,
                13500 => POWPoint.ВнизВправо,
                18000 => POWPoint.Вниз,
                22500 => POWPoint.ВнизВлево,
                27000 => POWPoint.Влево,
                31500 => POWPoint.ВверхВлево,
                _ => POWPoint.None
            };
    }
}
