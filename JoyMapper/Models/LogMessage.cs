using System;
using System.Windows.Media;

namespace JoyMapper.Models
{
    /// <summary>
    /// Сообщение состояния программы
    /// </summary>
    internal class LogMessage
    {
        public enum MessageType
        {
            Info,
            Warning,
            Error,
        }

        public LogMessage(MessageType Type, string Message)
        {
            this.Type = Type;
            this.Message = Message;
            Time = DateTime.Now;
        }


        public MessageType Type { get; }
        public string Message { get; }
        public DateTime Time { get; init; }

        public Color Color => Type switch
        {
            MessageType.Error => Colors.Red,
            MessageType.Warning => Colors.Yellow,
            MessageType.Info => Colors.Green,
            _ => throw new ArgumentOutOfRangeException()
        };

        public override string ToString() => Time.ToLongTimeString() + " - " + Message;
    }
}
