using System.Collections.Generic;

namespace JoyMapper.Models
{
    internal class Profile
    {
        public string Name { get; set; }

        public List<KeyPattern> KeyPatterns { get; set; }
    }
}
