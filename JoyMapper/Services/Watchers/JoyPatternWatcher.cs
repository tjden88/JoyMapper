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
        private readonly IServiceProvider _ServiceProvider;

        private readonly int _PollingDelay; 

        public JoyPatternWatcher(IJoystickStateManager JoystickStateManager, DataManager DataManager, IServiceProvider ServiceProvider)
        {
            _JoystickStateManager = JoystickStateManager;
            _ServiceProvider = ServiceProvider;
            _PollingDelay = DataManager.AppSettings.JoystickPollingDelay;
        }


        private List<PatternsByJoyName> _PatternsByJoyName;


        public void StartWatching(ICollection<JoyPattern> Patterns)
        {
            if (_PatternsByJoyName?.Count > 0)
            {
                StopWatching();
            }

            foreach (var joyPattern in Patterns)
            {
                joyPattern.PatternAction.Initialize(_ServiceProvider);
                joyPattern.PatternAction.ReportMessage += ReportPatternAction;
            }


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
            _PatternsByJoyName?.Clear();
            //foreach (var d in ReportPatternAction?.GetInvocationList()) 
            //    ReportPatternAction -= (Action<string>) d;

            Debug.WriteLine("JoyPatternWatcher остановлен");
        }

        public Action<string> ReportPatternAction { get; set; }


        private bool _IsRunning;

        private async Task Polling()
        {
            while (_IsRunning)
            {
                try
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
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }
            }
        }


        private class PatternsByJoyName
        {
            public string JoyName { get; set; }

            public List<JoyPattern> JoyPatterns { get; set; }
        }
    }
}
