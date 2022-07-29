using System;
using System.Diagnostics.CodeAnalysis;
using JoyMapper.ViewModels;
using SharpDX.DirectInput;

namespace JoyMapper.Models
{
    /// <summary>
    /// Действие джойстикка (кнопка, указатель, ось)
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class JoyAction
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }


        private POWPoint GetPowPoint(int rawValue) =>
            rawValue switch
            {
                -1 => POWPoint.None,
                0 => POWPoint.Up,
                _ => POWPoint.None
            };
    }
}
