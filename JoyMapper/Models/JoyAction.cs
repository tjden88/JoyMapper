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

        public JoyAction(StateType stateType) => Type = stateType;

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
            Up,
            Down,
            Left,
            Right,
            UpLeft,
            DownLeft,
            DownRight,
            UpRight,
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

        public POWPoint POWPosition { get; set; }

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
                    var pow1State = state.PointOfViewControllers[0];
                    return GetPowPoint(pow1State) == POWPosition;
                case StateType.POW2:
                    var pow2State = state.PointOfViewControllers[1];
                    return GetPowPoint(pow2State) == POWPosition;

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
                StateType.POW1 or StateType.POW2 => "Переключатель вида " + POWPosition,
                _ => throw new ArgumentOutOfRangeException()
            };
            return txt;
        }

        private POWPoint GetPowPoint(int rawValue) =>
            rawValue switch
            {
                -1 => POWPoint.None,
                0 => POWPoint.Up,
                4500 => POWPoint.UpRight,
                9000 => POWPoint.Right,
                13500 => POWPoint.DownRight,
                18000 => POWPoint.Down,
                22500 => POWPoint.DownLeft,
                27000 => POWPoint.Left,
                31500 => POWPoint.UpLeft,
                _ => POWPoint.None
            };
    }
}
