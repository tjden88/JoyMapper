﻿using System.Collections.Generic;

namespace JoyMapper.Models
{
    public class Profile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<int> KeyPatternsIds { get; set; } = new();
    }
}
