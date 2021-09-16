using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;

namespace MonitoringSystem.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        #region ###변수 선언###
        #endregion
        #region ###Property 선언###

        private string displayDateTextBlock;
        public string DisplayDateTextBlock
        {
            get => displayDateTextBlock;
            set
            {
                displayDateTextBlock = value;
                NotifyOfPropertyChange(() => DisplayDateTextBlock);
            }
        }
        #endregion

        #region ###Command 처리

        public void Timer_Tick(object sender, EventArgs e)
        {
            DisplayDateTextBlock = DateTime.Now.ToString(@"HH:mm:ss");
        }
        public void LoadSettings()
        {
            ActivateItemAsync(new SettingsViewModel());
        }

        public void LoadLogs()
        {
            ActivateItemAsync(new LogViewModel());
        }

        public void LoadMonitoring()
        {
            ActivateItemAsync(new MonitoringViewModel());
            

        }

        public void LoadPumps()
        {
            ActivateItemAsync(new TankViewModel());
        }

        public void LoadConveyor()
        {
            ActivateItemAsync(new ConveyorViewModel());
        }
        public MainViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

        }
        #endregion
    }
}
