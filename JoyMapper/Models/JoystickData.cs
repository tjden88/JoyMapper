namespace JoyMapper.Models
{
    /// <summary>
    /// Данные устройства для определения привязок
    /// </summary>
    public record JoystickData
    {
        /// <summary>Идентификатор устройства</summary>
        public string DeviceId { get; set; }


        /// <summary> Отображаемое имя </summary>
        public string DeviceName { get; set; }
    }
}
