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

        #endregion

        #region ###Command 처리

        
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

        #endregion
    }
}
