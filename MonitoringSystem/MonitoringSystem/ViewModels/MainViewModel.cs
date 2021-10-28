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

        private double plantT;

        public double PlantT
        {
            get => plantT;
            set
            {
                plantT = value;
                NotifyOfPropertyChange(() => PlantT);
            }
        }

        private double plantH;

        public double PlantH
        {
            get => plantH;
            set
            {
                plantH = value;
                NotifyOfPropertyChange(() => PlantH);
            }
        }
        // RobotTemp
        private double robotTemp;
        public double RobotTemp
        {
            get => robotTemp;
            set
            {
                robotTemp = value;
                NotifyOfPropertyChange(() => RobotTemp);
            }
        }

        // ConveyTemp
        private double conveyTemp;
        public double ConveyTemp
        {
            get => conveyTemp;
            set
            {
                conveyTemp = value;
                NotifyOfPropertyChange(() => ConveyTemp);
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

        private string _DisplayDate;

        public string DisplayDate
        {
            get => _DisplayDate;
            set
            {
                _DisplayDate = value;
                NotifyOfPropertyChange(() => DisplayDate);
                NotifyOfPropertyChange(() => DisplayDate);
            }
        }
        private string _DisplayDate1;
        public string DisplayDate1
        {
            get => _DisplayDate1;
            set
            {
                _DisplayDate1 = value;
                NotifyOfPropertyChange(() => DisplayDate1);
                NotifyOfPropertyChange(() => DisplayDate1);
            }
        }

        private double SplantT;

        public double SPlantT
        {
            get => SplantT;
            set
            {
                SplantT = value;
                NotifyOfPropertyChange(() => SPlantT);
            }
        }


        private double SplantH;

        public double SPlantH
        {
            get => SplantH;
            set
            {
                SplantH = value;
                NotifyOfPropertyChange(() => SPlantH);
            }
        }

        private double SrobotArm;

        public double SRobotArm
        {
            get => SrobotArm;
            set
            {
                SrobotArm = value;
                NotifyOfPropertyChange(() => SRobotArm);
            }
        }

        private double Sconveyor;

        public double SConveyor
        {
            get => Sconveyor;
            set
            {
                Sconveyor = value;
                NotifyOfPropertyChange(() => SConveyor);
            }
        }

        private double SpumpT;

        public double SPumpT
        {
            get => SpumpT;
            set
            {
                SpumpT = value;
                NotifyOfPropertyChange(() => SPumpT);
            }
        }

        private double SflowRate;

        public double SFlowRate
        {
            get => SflowRate;
            set
            {
                SflowRate = value;
                NotifyOfPropertyChange(() => SFlowRate);
            }
        }

        private double Sdensity;

        public double SDensity
        {
            get => Sdensity;
            set
            {
                Sdensity = value;
                NotifyOfPropertyChange(() => SDensity);
            }
        }
       

        #region Gas 변수
        private double gas1;
        private double gas2;
        private double gas3;
        private double gas4;
        private double gas5;
        private double gas6;

        public double Gas1
        {
            get => gas1;
            set
            {
                gas1 = value;
                NotifyOfPropertyChange(() => Gas1);
            }
        }
        public double Gas2
        {
            get => gas2;
            set
            {
                gas2 = value;
                NotifyOfPropertyChange(() => Gas2);
            }
        }
        public double Gas3
        {
            get => gas3;
            set
            {
                gas3 = value;
                NotifyOfPropertyChange(() => Gas3);
            }
        }
        public double Gas4
        {
            get => gas4;
            set
            {
                gas4 = value;
                NotifyOfPropertyChange(() => Gas4);
            }
        }
        public double Gas5
        {
            get => gas5;
            set
            {
                gas5 = value;
                NotifyOfPropertyChange(() => Gas5);
            }
        }
        public double Gas6
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

        public static void ShowWindow(PropertyChangedBase viewmodel)
        {
            WindowManager windowManager = new WindowManager();
            dynamic settings = new ExpandoObject();
            settings.Height = 450;
            settings.Width = 700;
            settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            windowManager.ShowWindowAsync(viewmodel, null, settings);
        }

        public static void ShowVMDialog(PropertyChangedBase viewmodel)
        {
            WindowManager windowManager = new WindowManager();
            dynamic settings = new ExpandoObject();
            settings.WindowStyle = WindowStyle.None;
            settings.ShowInTaskbar = false;
            settings.WindowState = WindowState.Maximized;
         //   settings.ResizeMode = ResizeMode.CanMinimize;

            settings.Height = 1000;
            settings.Width = 1000;
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
                    SPlantT = double.Parse(reader["PlantT"].ToString());
                    SPlantH = double.Parse(reader["PlantH"].ToString());
                    SRobotArm = double.Parse(reader["RobotArm"].ToString());
                    SConveyor = double.Parse(reader["Conveyor"].ToString());
                    SPumpT = double.Parse(reader["PumpT"].ToString());
                    SFlowRate = double.Parse(reader["FlowRate"].ToString());
                    SDensity = double.Parse(reader["Density"].ToString());
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
            DisplayDate = DateTime.Now.ToString(@"yyyy-MM-dd");
            DisplayDate1 = DateTime.Now.ToString(@"ddd"+"요일");
            // 값 재정의
            GetSettings();
            PlantT = DataConnection.PlantT;
            PlantH = DataConnection.PlantH;
            ConveyTemp = DataConnection.ConveyTemp;
            RobotTemp = DataConnection.RobotTemp;
            Gas1 = DataConnection.Gas1;
            Gas2 = DataConnection.Gas2;
            Gas3 = DataConnection.Gas3;
            Gas4 = DataConnection.Gas4;
            Gas5 = DataConnection.Gas5;
            Gas6 = DataConnection.Gas6;
            
            // Error 표시
            if (Gas1 > 2.5)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());
                
            }
            if (Gas2 > 5.0)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());

            }
            if (Gas3 > 5.0)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());

            }
            if (Gas4 > 5.0)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());

            }
            if (Gas5 > 5.0)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());

            }
            if (Gas6 > 5.0)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());

            }
            if (PlantH > SPlantH)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());

            }
            if (PlantT > SPlantT)
            {
                DataConnection.EMessage = "위험 경보 : CO가 일정수치 이상입니다.";
                DataConnection.Solution = "팬을 돌려서 Gas를 밖으로 내보냅니다.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());
            }

            if (RobotTemp > SRobotArm)
            {
                DataConnection.EMessage = "위험 경보 : 로봇팔의 온도가 30 보다 높습니다.";
                DataConnection.Solution = "작동을 멈추고 30 보다 내려갔을 때 다시 작동해주세요.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());
            }
            if (ConveyTemp > SConveyor)
            {
                DataConnection.EMessage = "위험 경보 : 컨베이어의 온도가 30 보다 높습니다.";
                DataConnection.Solution = "작동을 멈추고 30 보다 내려갔을 때 다시 작동해주세요.";
                App.Logger.Warn("CO 가스 수치 이상");
                ShowVMDialog(new ErrorViewModel());
            }


        }
    }
}



