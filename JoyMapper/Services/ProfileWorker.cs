using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using SharpDX.DirectInput;
using JoyState = JoyMapper.ViewModels.JoyState;

namespace JoyMapper.Services
{
    /// <summary>
    /// Обработка нажатий кнопок контроллера и эмуляция клавиатурных команд
    /// </summary>
    internal class ProfileWorker
    {
        // задержка опроса джойстика
        private int _PollingDelay ;

        // задержка между командами ввода клавиатуры
        private int _InputDelay ;


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

            _PollingDelay = App.DataManager.AppSettings.JoystickPollingDelay;
            _InputDelay = App.DataManager.AppSettings.KeyboardInputDelay;

            var keyPatterns = App.DataManager.KeyPatterns
                .Where(p => profile.KeyPatternsIds.Contains(p.Id))
                .ToArray();

            if (keyPatterns.Length == 0)
            {
                AppLog.LogMessage("В выбранном профиле не назначено ни одного паттерна! Необходимо отредактировать профиль или запустить другой"
                    , LogMessage.MessageType.Error);
                return;
            }


            var usedJoyNames = keyPatterns
                .Select(p => p.JoyName)
                .Distinct()
                .ToArray();

            var usedJoysticks = new DirectInput()
                .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                .Where(d => usedJoyNames.Contains(d.InstanceName))
                .Select(d => new Joystick(new DirectInput(), d.InstanceGuid))
                .ToArray();

            if (usedJoysticks.Length == 0)
            {
                AppLog.LogMessage("Джойстики, используемые в этом профиле, не найдены. Подключите их и перезапустите профиль"
                    , LogMessage.MessageType.Error);
                return;
            }

            _UsedInProfileJoystickStates = usedJoysticks
                .Select(joy => new JoyState
                {
                    Joystick = joy,
                    Actions = keyPatterns
                        .Where(p=>p.JoyName == joy.Information.InstanceName)
                        .Select(p=> new JoyState.ActionState(p.JoyActionOld))
                        .ToList()
                })
                .ToList();


            foreach (var joyState in _UsedInProfileJoystickStates)
            {
                foreach (var actionState in joyState.Actions)
                {
                    var pattern = keyPatterns.First(p =>
                        p.JoyName == joyState.Joystick.Information.InstanceName && p.JoyActionOld == actionState.ActionOld);
                    actionState.PressKeyBindings = pattern.PressKeyBindings;
                    actionState.ReleaseKeyBindings = pattern.ReleaseKeyBindings;
                }

                joyState.SyncStatus();
            }

            IsActive = true;
            Task.Run(Work);
            AppLog.LogMessage("Профиль запущен");

            if (usedJoysticks.Length < usedJoyNames.Length)
            {
                AppLog.LogMessage($"Найдено {usedJoysticks.Length} джойстиков из задействованных в профиле: {usedJoyNames.Length}.\n" +
                                  $"Подключите нужные устройства и перезапустите профиль", LogMessage.MessageType.Warning);
            }

        }

        /// <summary> Остановить отслеживание </summary>
        public void Stop()
        {
            IsActive = false;
            Thread.Sleep(_PollingDelay + 10);
        }

        private async Task Work()
        {
            while (IsActive)
            {
                foreach (var joyState in _UsedInProfileJoystickStates)
                {
                    var diff = joyState.GetDifferents();
                    foreach (var diffState in diff)
                        await SendCommands(diffState.IsActive
                            ? diffState.PressKeyBindings
                            : diffState.ReleaseKeyBindings);
                }
                await Task.Delay(_PollingDelay);
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

                await Task.Delay(_InputDelay);
            }

        }
    }
}
