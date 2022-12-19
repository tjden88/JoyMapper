using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.Listeners
{
    /// <summary>
    /// Модель привязки с модификатором - для прослушивателя привязок
    /// </summary>
    public class ModificatedJoyBinding
    {
        public JoyBindingBase BindingBase { get; set; }

        public int? ModificatorId { get; set; }
    }
}
