using JoyMapper.Models.Base;

namespace JoyMapper.Models;

public class Modificator
{
    public int Id { get; set; }

    public string Name { get; set; }

    public JoyBindingBase Binding { get; set; }
}