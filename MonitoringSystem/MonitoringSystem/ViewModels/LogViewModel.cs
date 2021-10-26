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


        private bool ischecked1 = false;
        public bool IsChecked1
        {
            get => ischecked1;
            set
            {
                ischecked1 = value;
                NotifyOfPropertyChange(() => IsChecked1);
                if (IsChecked1 == true)
                {
                    IsChecked2 = false;
                    IsChecked3 = false;
                    IsChecked4 = false;
                }
            }
        }
        private bool ischecked2 = false;
        public bool IsChecked2
        {
            get => ischecked2;
            set
            {
                ischecked2 = value;
                NotifyOfPropertyChange(() => IsChecked2);
                if (IsChecked2 == true)
                {
                    IsChecked1 = false;
                    IsChecked3 = false;
                    IsChecked4 = false;
                }

            }
        }
        private bool ischecked3 = false;
        public bool IsChecked3
        {
            get => ischecked3;
            set
            {
                ischecked3 = value;
                NotifyOfPropertyChange(() => IsChecked3);
                if (IsChecked3 == true)
                {
                    IsChecked1 = false;
                    IsChecked2 = false;
                    IsChecked4 = false;
                }

            }
        }
        private bool ischecked4 = false;
        public bool IsChecked4
        {
            get => ischecked4;
            set
            {
                ischecked4 = value;
                NotifyOfPropertyChange(() => IsChecked4);
                if (IsChecked4 == true)
                {
                    IsChecked1 = false;
                    IsChecked2 = false;
                    IsChecked3 = false;
                }

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

        private DateTime startDate = DateTime.Now;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        private DateTime endDate = DateTime.Now;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        
        public void GetLogs()
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                DateTime SDate = StartDate;
                DateTime EDate = EndDate;
                string Type;
                string Message1 = "%";
                try
                {
                    if (IsChecked1 == true)
                    {
                        Message1 = "공장";

                    }
                    else if (IsChecked2 == true)
                    {
                        Message1 = "컨베이어";
                    }
                    else if (IsChecked3 == true)
                    {
                        Message1 = "로봇팔";

                    }
                    else if (IsChecked4 == true)
                    {
                        Message1 = "펌프";
                    }

                    if (SelectedType.Type == "All")
                    {
                        Type = "%";
                    }
                    else
                    {
                        Type = "%" + SelectedType.Type + "%";
                    }

                }
                catch (Exception)
                {
                    Type = "%";
                }
                conn.Open();


                SqlCommand cmd = new SqlCommand(Models.TB_Log.SELECT_QUERY, conn);
                SqlParameter TypeP = new SqlParameter("@type", Type);
                SqlParameter SDATEP = new SqlParameter("@SDATE", SDate);
                SqlParameter EDATEP = new SqlParameter("@EDATE", EDate);
                SqlParameter MESSAGE1P = new SqlParameter("@MESSAGE1", Message1);
                cmd.Parameters.Add(TypeP);
                cmd.Parameters.Add(SDATEP);
                cmd.Parameters.Add(EDATEP);
                cmd.Parameters.Add(MESSAGE1P);
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
