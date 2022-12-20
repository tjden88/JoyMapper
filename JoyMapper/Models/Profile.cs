using System.Collections.Generic;

namespace JoyMapper.Models;

public class Profile
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<int> PatternsIds { get; set; } = new();
}