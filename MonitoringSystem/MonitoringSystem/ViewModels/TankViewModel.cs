using System;
using System.Windows;
using System.Windows.Input;

namespace MonitoringSystem.ViewModels
{
    public class TankViewModel
    {
        #region ###변수 선언###

        #endregion

        #region ###Property 선언###

        #endregion

        #region ###Command 처리

        ICommand clickCommand;
        public ICommand ClickCommand => clickCommand ?? (clickCommand = new RelayCommand<object>(o => Click(), o => IsClick()));


        private bool IsClick()
        {
            return true; // Validation Check를 쉽고 간단하게 
        }
        private void Click()
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외발생 : {ex.Message}");
            }
        }
        #endregion
    }
}
