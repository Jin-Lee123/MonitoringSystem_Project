using Caliburn.Micro;
using MonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ViewModels
{
    public class LogViewModel : Conductor<object>
    {
        private BindableCollection<TB_Log> TB_log;

        public BindableCollection<TB_Log> Log
        {
            get => TB_log;
            set
            {
                TB_log = value;
                NotifyOfPropertyChange(() => Log);
            }
        }

        private int seq;
        public int Seq
        {
            get => seq;
            set
            {
                seq = value;
                NotifyOfPropertyChange(() => Seq);
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

        public void GetLogs()
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.TB_Log.SELECT_QUERY, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                Log = new BindableCollection<TB_Log>();

                while (reader.Read())
                {
                    var empTmp = new TB_Log
                    {
                        Seq = (int)reader["Seq"],
                        Space = reader["Space"].ToString(),
                        Error = reader["Error"].ToString(),
                        Content = reader["Content"].ToString(),

                    };
                    Log.Add(empTmp);
                }
            }
        }

        public LogViewModel()
        {
            // DB연결
            GetLogs();
        }
    }
}
