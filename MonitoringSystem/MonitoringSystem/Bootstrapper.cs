using Caliburn.Micro;
using System.Windows;
using MonitoringSystem.ViewModels;

namespace MonitoringSystem
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
            {
                Initialize(); // Caliburn.Micro 초기화(필수!!)
            }

        protected override void OnStartup(object sender, StartupEventArgs e)
            {
                //base.OnStartup(sender, e);
                DisplayRootViewFor<MainViewModel>();

            // 실행시 로그인창 modal로 실행
            //MainViewModel.ShowVMDialog(new LoginViewModel());
            }
        }
    }
