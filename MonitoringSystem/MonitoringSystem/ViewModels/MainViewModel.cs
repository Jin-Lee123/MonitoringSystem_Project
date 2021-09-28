using System.Dynamic;
using System.Windows;
using Caliburn.Micro;
using MonitoringSystem.Views;

namespace MonitoringSystem.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        #region ###변수 선언###

        

        #endregion

        #region ### 로그인 전 화면 구성

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

        public static void ShowVMDialog(PropertyChangedBase viewmodel)
        {
            WindowManager windowManager = new WindowManager();
            dynamic settings = new ExpandoObject();
            settings.WindowStyle = WindowStyle.None;
            settings.ShowInTaskbar = false;
            settings.WindowState = WindowState.Normal;
            settings.ResizeMode = ResizeMode.CanMinimize;
            settings.Height = 450;
            settings.Width = 450;
            settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            windowManager.ShowDialogAsync(viewmodel, null, settings);
        }
        #endregion
    }
}
