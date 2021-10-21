using Caliburn.Micro;
using MonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ViewModels
{
    public class ErrorViewModel : Conductor<object>
    {
        public void OK()
        {
            TryCloseAsync();
        }

        private string eMessage;
        public string EMessage
        {
            get => eMessage;
            set
            {
                eMessage = value;
                NotifyOfPropertyChange(() => EMessage);
            }
        }

        private string solution;
        public string Solution
        {
            get => solution;
            set
            {
                solution = value;
                NotifyOfPropertyChange(() => Solution);
            }
        }

        public ErrorViewModel()
        {
            EMessage = DataConnection.EMessage;
            Solution = DataConnection.Solution;
        }
    }
}
