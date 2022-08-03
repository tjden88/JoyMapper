using System.Text.Json.Serialization;

namespace JoyMapper.Models.JoyActions
{
    /// <summary>
    /// Дейстаие джойстика (базовый класс)
    /// </summary>
    internal abstract class JoyActionBase
    {
        public string JoyName { get; set; }

        /// <summary> Описание дествия </summary>
        [JsonIgnore]
        public abstract string Description { get; }
    }
}
