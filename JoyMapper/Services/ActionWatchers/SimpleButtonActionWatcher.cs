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

        public override JoyActionBase JoyAction => _ButtonJoyAction;

        public override void Poll(JoyState joyState, bool SendCommands)
        {
            var btnState = _ButtonJoyAction.Button.Type switch
            {
                ButtonType.Button => joyState.Buttons[_ButtonJoyAction.Button.Value - 1],
                ButtonType.Pow1 => joyState.Pow1Value == _ButtonJoyAction.Button.Value,
                ButtonType.Pow2 => joyState.Pow2Value == _ButtonJoyAction.Button.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

            var isActive = IsActive;
            IsActive = btnState;
           if(isActive == btnState) return;

            if(!SendCommands) return;

            Debug.WriteLine(btnState ? "SendingPressKbCommands" : "SendingReleaseKbCommands");
            SendKeyboardCommands(btnState 
                ? _ButtonJoyAction.PressKeyBindings 
                : _ButtonJoyAction.ReleaseKeyBindings);
        }
    }
}
