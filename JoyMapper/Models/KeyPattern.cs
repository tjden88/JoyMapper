using System.Collections.Generic;

namespace JoyMapper.Models
{
    internal class KeyPattern
    {
        public string Name { get; set; }

        public int JoyId { get; set; }

        public int JoyKey { get; set; }

        public bool IsPressed { get; set; }

        public bool IsRelease => !IsPressed;

        public List<KeyBinding> KeyBindings { get; set; }
    }
}
