using Newtonsoft.Json;

namespace JoyMapper.Models.JoyActions
{
    /// <summary>
    /// Дейстаие джойстика (базовый класс)
    /// </summary>
    internal abstract class JoyActionBase
    {
        /// <summary> Описание дествия </summary>
        [JsonIgnore]
        public abstract string Description { get; }
    }
}