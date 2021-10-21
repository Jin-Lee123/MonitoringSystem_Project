using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Text;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using MonitoringSystem.Models;
using MonitoringSystem.Views;
using Newtonsoft.Json;
using NLog;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Threading;

namespace MonitoringSystem.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        #region ###변수 선언###
        #region TB_SET 설정



        private BindableCollection<TB_SET> TB_set;

        public BindableCollection<TB_SET> TB_SET
        {
            get => TB_set;
            set
            {
                TB_set = value;
                NotifyOfPropertyChange(() => TB_SET);
            }
        }

        private float plantT;

        public float PlantT
        {
            get => plantT;
            set
            {
                plantT = value;
                NotifyOfPropertyChange(() => PlantT);
            }
        }
        

        private string _DisplayDateTextBlock;

        public string DisplayDateTextBlock
        {
            get => _DisplayDateTextBlock;
            set
            {
                _DisplayDateTextBlock = value;
                NotifyOfPropertyChange(() => DisplayDateTextBlock);
            }
        }
        private float SplantT;

        public float SPlantT
        {
            get => SplantT;
            set
            {
                SplantT = value;
                NotifyOfPropertyChange(() => SPlantT);
            }
        }


        private float SplantH;

        public float SPlantH
        {
            get => SplantH;
            set
            {
                SplantH = value;
                NotifyOfPropertyChange(() => SPlantH);
            }
        }

        private float SrobotArm;

        public float SRobotArm
        {
            get => SrobotArm;
            set
            {
                SrobotArm = value;
                NotifyOfPropertyChange(() => SRobotArm);
            }
        }

        private float Sconveyor;

        public float SConveyor
        {
            get => Sconveyor;
            set
            {
                Sconveyor = value;
                NotifyOfPropertyChange(() => SConveyor);
            }
        }

        private float SpumpT;

        public float SPumpT
        {
            get => SpumpT;
            set
            {
                SpumpT = value;
                NotifyOfPropertyChange(() => SPumpT);
            }
        }

        private float SflowRate;

        public float SFlowRate
        {
            get => SflowRate;
            set
            {
                SflowRate = value;
                NotifyOfPropertyChange(() => SFlowRate);
            }
        }

        private float Sdensity;

        public float SDensity
        {
            get => Sdensity;
            set
            {
                Sdensity = value;
                NotifyOfPropertyChange(() => SDensity);
            }
        }
       

        #region Gas 변수
        private float gas1;
        private float gas2;
        private float gas3;
        private float gas4;
        private float gas5;
        private float gas6;

        public float Gas1
        {
            get => gas1;
            set
            {
                gas1 = value;
                NotifyOfPropertyChange(() => Gas1);
            }
        }
        public float Gas2
        {
            get => gas2;
            set
            {
                gas2 = value;
                NotifyOfPropertyChange(() => Gas2);
            }
        }
        public float Gas3
        {
            get => gas3;
            set
            {
                gas3 = value;
                NotifyOfPropertyChange(() => Gas3);
            }
        }
        public float Gas4
        {
            get => gas4;
            set
            {
                gas4 = value;
                NotifyOfPropertyChange(() => Gas4);
            }
        }
        public float Gas5
        {
            get => gas5;
            set
            {
                gas5 = value;
                NotifyOfPropertyChange(() => Gas5);
            }
        }
        public float Gas6
        {
            get => gas6;
            set
            {
                gas6 = value;
                NotifyOfPropertyChange(() => Gas6);
            }
        }
        #endregion

        #endregion



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
            settings.Height = 700;
            settings.Width = 500;
            settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            windowManager.ShowDialogAsync(viewmodel, null, settings);
        }
        #endregion

        #region Setting 값 받아오기
        public void GetSettings()
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                SqlCommand cmd = new SqlCommand(Models.TB_SET.SELECT_QUERY, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    SPlantT = float.Parse(reader["PlantT"].ToString());
                    SPlantH = float.Parse(reader["PlantH"].ToString());
                    SRobotArm = float.Parse(reader["RobotArm"].ToString());
                    SConveyor = float.Parse(reader["Conveyor"].ToString());
                    SPumpT = float.Parse(reader["PumpT"].ToString());
                    SFlowRate = float.Parse(reader["FlowRate"].ToString());
                    SDensity = float.Parse(reader["Density"].ToString());
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        #endregion

        public MainViewModel()
        {
            DataConnection.Client_Start();
            GetSettings();
            

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();


        }
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            DisplayDateTextBlock = DateTime.Now.ToString(@"HH:mm:ss");

            // 값 재정의
            PlantT = DataConnection.PlantT; 
            Gas1 = DataConnection.Gas1;
            Gas2 = DataConnection.Gas2;
            Gas3 = DataConnection.Gas3;
            Gas4 = DataConnection.Gas4;
            Gas5 = DataConnection.Gas5;
            Gas6 = DataConnection.Gas6;
            
            // Error 표시
            if (Gas1 < 2.5 || Gas2 < 3.0 || PlantT < 25)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
               // ShowVMDialog(new ErrorViewModel());
                
            }
            else
            {

            }


        }
    }
}



