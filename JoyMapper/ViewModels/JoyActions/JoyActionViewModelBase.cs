using System.Windows.Input;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.JoyActions
{
    internal abstract class JoyActionViewModelBase : ViewModel
    {

        /// <summary>Описание действия</summary>
        public abstract string Description { get; }

        /// <summary> Есть ли назначенные кнопки </summary>
        public abstract bool HasKeyBindings { get; }

        /// <summary> Действие ожидает записи команд </summary>
        public abstract bool IsRecording { get; }

        /// <summary> Добавить команду в ожидающие списки </summary>
        public abstract void AddKeyBinding(Key key, bool isPress);
    }
}
