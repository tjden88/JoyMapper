using JoyMapper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Services.Data;

namespace JoyMapper.Services.Watchers
{
    public class JoyPatternWatcher : IJoyPatternWatcher
    {
        private readonly IJoystickStateManager _JoystickStateManager;
        private readonly DataManager _DataManager;

        private readonly int _PollingDelay; 

        public JoyPatternWatcher(IJoystickStateManager JoystickStateManager, DataManager DataManager)
        {
            _JoystickStateManager = JoystickStateManager;
            _DataManager = DataManager;
            _PollingDelay = DataManager.AppSettings.JoystickPollingDelay;
        }


        private List<PatternsByJoyName> _PatternsByJoyName;


        public void StartWatching(ICollection<JoyPattern> Patterns)
        {
            foreach (var joyPattern in Patterns)
                joyPattern.PatternAction.ReportMessage += OnActionReportMessage;


            _PatternsByJoyName = Patterns.GroupBy(p => p.Binding.JoyName)
                .Where(g => g.Key != null)
                .Select(gr => new PatternsByJoyName
                {
                    JoyName = gr.Key,
                    JoyPatterns = gr.ToList()
                })
                .ToList();
            _IsRunning = true;
            Task.Run(Polling);
        }

        private void OnActionReportMessage(string message)
        {
            Debug.WriteLine(message);
        }

        public void StopWatching()
        {
            _IsRunning = false;
        }


        private bool _IsRunning;

        private async Task Polling()
        {
            while (_IsRunning)
            {
                foreach (var patternsByJoyName in _PatternsByJoyName)
                {
                    var state = _JoystickStateManager.GetJoyState(patternsByJoyName.JoyName);
                    if (state != null)
                    {
                        patternsByJoyName.JoyPatterns.ForEach(pattern =>
                        {
                            var lastState = pattern.Binding.IsActive;
                            var newState = pattern.Binding.UpdateIsActive(state);
                            if (lastState != newState)
                               pattern.PatternAction.BindingStateChanged(newState);
                        });
                    }
                }

                await Task.Delay(_PollingDelay);
            }
        }


        private class PatternsByJoyName
        {
            public string JoyName { get; set; }

            public List<JoyPattern> JoyPatterns { get; set; }
        }
    }
}
