using JoyMapper.Models.JoyActions;

namespace JoyMapper.Models
{
    internal class Modificator
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string JoyName { get; set; }

        public JoyButton Button { get; set; }
    }
}
