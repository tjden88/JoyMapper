using System.Collections.Generic;

namespace JoyMapper.Models
{
    internal class KeyPattern
    {
        public string Name { get; set; }

        public string JoyName { get; set; }

        public int JoyKey { get; set; }

        public List<KeyboardKeyBinding> PressKeyBindings { get; set; }
        public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }
    }
}
