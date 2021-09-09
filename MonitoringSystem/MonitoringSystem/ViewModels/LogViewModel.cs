using Caliburn.Micro;
using MonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ViewModels
{
    public class LogViewModel : Conductor<object>
    {
        private BindableCollection<TB_Log> TB_log;

        public BindableCollection<TB_Log> TB_Log
        {
            get => TB_log;
            set
            {
                TB_log = value;
                NotifyOfPropertyChange(() => TB_Log);
            }
        }

        private int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        private string space;
        public string Space
        {
            get => space;
            set
            {
                space = value;
                NotifyOfPropertyChange(() => Space);
            }
        }

        private string error;
        public string Error
        {
            get => error;
            set
            {
                error = value;
                NotifyOfPropertyChange(() => Error);
            }
        }

        private string content;
        public string Content
        {
            get => content;
            set
            {
                content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }
    }
}
