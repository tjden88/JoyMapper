using System.Linq;
using System.Windows;
using JoyMapper.Services;

namespace JoyMapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary> Менеджер данных профилей текущей сессии </summary>
        internal static DataManager DataManager { get; } = new();

        /// <summary> Активное окно </summary>
        internal static Window ActiveWindow => Current.Windows.Cast<Window>().First(w => w.IsActive);


    }

}
