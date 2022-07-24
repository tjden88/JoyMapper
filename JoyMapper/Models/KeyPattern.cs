using System.Collections.Generic;

namespace JoyMapper.Models
{
    internal class KeyPattern
    {
        public string Name { get; set; }

        public int JoyId { get; set; }

        public int JoyKey { get; set; }

        public List<KeyBinding> PressKeyBindings { get; set; }
        public List<KeyBinding> ReleaseKeyBindings { get; set; }
    }
}
