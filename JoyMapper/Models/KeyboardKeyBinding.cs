using System.Windows.Input;

namespace JoyMapper.Models
{
    public class KeyboardKeyBinding
    {
        public Key KeyCode { get; set; }

        public bool IsPress { get; set; }

        public override string ToString()
        {
            var prefix = IsPress
                ? "Нажать - "
                : "Отпустить - ";
            return prefix + KeyCode;
        }
    }
}
