using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
