using System.Windows;
using JoyMapper.Services;

namespace JoyMapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        internal static DataManager DataManager { get; } = new();
 
    }

}
