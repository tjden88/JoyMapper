using JoyMapper.Models.JoyActions;

namespace JoyMapper.Models
{
    public class KeyPattern
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string JoyName { get; set; }

        public JoyActionBase JoyAction { get; set; }

        public Modificator Modificator { get; set; }

    }
}
