using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.ViewModels;
using SharpDX.DirectInput;

namespace JoyMapper.Services
{
    /// <summary>
    /// Обработка нажатий кнопок контроллера и эмуляция клавиатурных команд
    /// </summary>
    internal class ProfileWorker
    {
        // задержка опроса джойстика
        private const int PollingDelay = 50;

        // задержка между командами ввода клавиатуры
        private const int InputDelay = 5;


        private readonly KeyboardSender _Sender = new();

        // Используемые в профиле джойстики
        private List<JoyState> _UsedInProfileJoystickStates;

        // Активен ли трекинг
        private bool IsActive { get; set; }


        /// <summary> Запустить отслеживание </summary>
        public void Start(Profile profile)
        {
            if (IsActive)
                Stop();


            var keyPatterns = App.DataManager.KeyPatterns
                .Where(p => profile.KeyPatternsIds.Contains(p.Id))
                .ToArray();

            var usedJoyNames = keyPatterns
                .Select(p => p.JoyName)
                .Distinct();

            var usedJoysticks = new DirectInput()
                .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                .Where(d => usedJoyNames.Contains(d.InstanceName))
                .Select(d => new Joystick(new DirectInput(), d.InstanceGuid))
                .ToList();



            _UsedInProfileJoystickStates = usedJoysticks
                .Select(joy => new JoyState
                {
                    Joystick = joy,
                    BtnStates = keyPatterns
                        .Where(p=>p.JoyName == joy.Information.InstanceName)
                        .Select(p=> new JoyState.BtnState(p.JoyKey))
                        .ToList()
                })
                .ToList();


            foreach (var joyState in _UsedInProfileJoystickStates)
            {
                foreach (var btnState in joyState.BtnStates)
                {
                    var pattern = keyPatterns.First(p =>
                        p.JoyName == joyState.Joystick.Information.InstanceName && p.JoyKey == btnState.BtnNumber);
                    btnState.PressKeyBindings = pattern.PressKeyBindings;
                    btnState.ReleaseKeyBindings = pattern.ReleaseKeyBindings;
                }

                joyState.UpdateBtnStatus();
            }

            IsActive = true;
            Task.Run(Work);
        }

        /// <summary> Остановить отслеживание </summary>
        public void Stop()
        {
            IsActive = false;
            Thread.Sleep(PollingDelay + 10);
        }

        private async Task Work()
        {
            while (IsActive)
            {
                foreach (var joyState in _UsedInProfileJoystickStates)
                {
                    var diff = joyState.GetDifferents();
                    foreach (var diffState in diff)
                        await SendCommands(diffState.IsPressed
                            ? diffState.PressKeyBindings
                            : diffState.ReleaseKeyBindings);
                }
                await Task.Delay(PollingDelay);
            }
        }


        private async Task SendCommands(List<KeyboardKeyBinding> keyBindings)
        {
            foreach (var binding in keyBindings)
            {
                if(binding.IsPress)
                    _Sender.PressKey(binding.KeyCode);
                else
                    _Sender.ReleaseKey(binding.KeyCode);

                await Task.Delay(InputDelay);
            }

        }
    }
}
