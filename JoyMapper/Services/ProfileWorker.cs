using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using SharpDX.DirectInput;

namespace JoyMapper.Services
{
    /// <summary>
    /// Обработка нажатий кнопок контроллера и эмуляция клавиатурных команд
    /// </summary>
    internal class ProfileWorker
    {
        // задержка опроса джойстика
        private int _PollingDelay;


        // Используемые в профиле поллеры
        private List<JoystickPoller> _JoystickPollers;

        // Активен ли трекинг
        private bool IsActive { get; set; }


        /// <summary> Запустить отслеживание </summary>
        public void Start(Profile profile)
        {
            if (IsActive)
                Stop();

            _PollingDelay = App.DataManager.AppSettings.JoystickPollingDelay;

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

            var usedDevices = new DirectInput()
                .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                .Where(d => usedJoyNames.Contains(d.InstanceName))
                .ToArray();

            if (usedDevices.Length == 0)
            {
                AppLog.LogMessage("Джойстики, используемые в этом профиле, не найдены. Подключите их и перезапустите профиль"
                    , LogMessage.MessageType.Error);
                return;
            }

            _JoystickPollers = usedDevices
                .Select(joy => new JoystickPoller(joy.InstanceName,
                    keyPatterns
                        .Where(p => p.JoyName == joy.InstanceName)
                        .Select(p => p.JoyAction)
                ))
                .ToList();


            foreach (var poller in _JoystickPollers)
                Task.Run(poller.SyncActions);

            
            IsActive = true;
            Task.Run(Work);
            AppLog.LogMessage("Профиль запущен");

            if (usedDevices.Length < usedJoyNames.Length)
            {
                AppLog.LogMessage($"Найдено {usedDevices.Length} джойстиков из задействованных в профиле: {usedJoyNames.Length}.\n" +
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
                foreach (var poller in _JoystickPollers) poller.Poll();

                await Task.Delay(_PollingDelay);
            }
        }
    }
}
