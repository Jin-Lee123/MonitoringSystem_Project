using Caliburn.Micro;
using MonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ViewModels
{
    public class LogViewModel : Conductor<object>
    {
        private ObservableCollection<TB_Log> types;

        public ObservableCollection<TB_Log> Types
        {
            get
            {
                return types;
            }
            set
            {
                types = value;
            }
        }

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

        private TB_Log selectedType;
        public TB_Log SelectedType
        {
            get
            {
                return selectedType;
            }
            set
            {
                selectedType = value;

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

        private string level;
        public string Level
        {
            get => level;
            set
            {
                level = value;
                NotifyOfPropertyChange(() => Level);
            }
        }

        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        private string additionalInfo;
        public string AddtionalInfo
        {
            get => additionalInfo;
            set
            {
                additionalInfo = value;
                NotifyOfPropertyChange(() => AddtionalInfo);
            }
        }

        private DateTime loggedonDate;
        public DateTime LoggedOnDate
        {
            get => loggedonDate;
            set
            {
                loggedonDate = value;
                NotifyOfPropertyChange(() => loggedonDate);
            }
        }

        public void GetLogs()
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                string Type;
                if (SelectedType.Type == "All")
                {
                    Type = "%";
                }
                else
                {
                    Type = "%" + SelectedType.Type + "%";
                }
                conn.Open();


                SqlCommand cmd = new SqlCommand(Models.TB_Log.SELECT_QUERY, conn);
                SqlParameter TypeP = new SqlParameter("@type", Type);
                cmd.Parameters.Add(TypeP);
                SqlDataReader reader = cmd.ExecuteReader();
                Log = new BindableCollection<TB_Log>();
                
                while (reader.Read())
                {

                    var empTmp = new TB_Log
                    {

                        ID = (int)reader["ID"],
                        Level = reader["Level"].ToString(),
                        Message = reader["Message"].ToString(),
                        AdditionalInfo = reader["AdditionalInfo"].ToString(),
                        LoggedOnDate = reader["LoggedOnDate"].ToString()

                    };
                    Log.Add(empTmp);
                }
            }
        }

        public LogViewModel()
        {
            // DB연결
            Types = new ObservableCollection<TB_Log>()
            {
                new TB_Log(){Type = "All"},
                new TB_Log(){Type = "Fatal"},
                new TB_Log(){Type = "Warn"},
                new TB_Log(){Type = "Info"}
            };

        }
    }
}
