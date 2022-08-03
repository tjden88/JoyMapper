using System.Collections.Generic;

namespace JoyMapper.Models
{
    internal class KeyPattern
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string JoyName { get; set; }

        public JoyActionOld JoyActionOld { get; set; }

        public List<KeyboardKeyBinding> PressKeyBindings { get; set; }
        public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }
    }
}
