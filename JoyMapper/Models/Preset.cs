using System.Collections.Generic;

namespace JoyMapper.Models
{
    internal class Preset
    {
        public string Name { get; set; }

        public List<Profile> Profiles { get; set; }
    }
}
