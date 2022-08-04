using System;
using System.Diagnostics;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.Services.ActionWatchers
{
    internal class SimpleButtonActionWatcher : ActionWatcherBase
    {
        private readonly SimpleButtonJoyAction _ButtonJoyAction;

        public SimpleButtonActionWatcher(SimpleButtonJoyAction buttonJoyAction)
        {
            _ButtonJoyAction = buttonJoyAction;
        }

        public override void Poll(JoyState joyState)
        {
            var btnState = _ButtonJoyAction.Button.Type switch
            {
                ButtonType.Button => joyState.Buttons[_ButtonJoyAction.Button.Value],
                ButtonType.Pow1 => joyState.Pow1Value == _ButtonJoyAction.Button.Value,
                ButtonType.Pow2 => joyState.Pow2Value == _ButtonJoyAction.Button.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

            if(IsActive == btnState) return;

            IsActive = btnState;
            Debug.WriteLine(btnState ? "SendingPressKbCommands" : "SendingReleaseKbCommands");
            SendKeyboardCommands(btnState 
                ? _ButtonJoyAction.PressKeyBindings 
                : _ButtonJoyAction.ReleaseKeyBindings);
        }
    }
}
