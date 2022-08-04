using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;
using JoyMapper.Services.ActionWatchers;
using JoyMapper.ViewModels;
using JoyState = JoyMapper.Models.JoyState;

namespace JoyMapper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        internal MainWindowViewModel ViewModel { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
          // TestSimple();
          //TestAxis();
          TestExtended();
        }

        private async void TestSimple()
        {
            Debug.WriteLine("Тест запущен");
            var service = new SimpleButtonActionWatcher(new SimpleButtonJoyAction() { JoyName = "TestSimple", Button = new JoyButton() { Type = ButtonType.Button, Value = 1 } });

            while (true)
            {
                service.Poll(new JoyState()
                {
                    Buttons = new[] { false, Keyboard.IsKeyDown(Key.Space) }
                });
                await Task.Delay(50);
            }
        }

        private async void TestAxis()
        {
            Debug.WriteLine("Тест запущен");
            var service = new AxisActionWatcher(new AxisJoyAction() { JoyName = "TestAxis",Axis = AxisJoyAction.Axises.X, StartValue = 20, EndValue = 40});

            while (true)
            {
                service.Poll(new JoyState()
                {
                   AxisValues = new JoyState.AxisState()
                   {
                       X = Keyboard.IsKeyDown(Key.Space) ? 30 : 50            
                   }
                });
                await Task.Delay(50);
            }
        }

        private async void TestExtended()
        {
            Debug.WriteLine("Тест запущен");
            var service = new ExtendedButtonActionWatcher(new ExtendedButtonJoyAction() { JoyName = "TestExtended", Button = new JoyButton() { Type = ButtonType.Button, Value = 1 } });

            while (true)
            {
                service.Poll(new JoyState()
                {
                    Buttons = new[] { false, Keyboard.IsKeyDown(Key.Space) }
                });
                await Task.Delay(50);
            }
        }
    }
}
