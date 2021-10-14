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
            settings.Height = 450;
            settings.Width = 450;
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

        #region Error 처리x

        #endregion
        
        public MainViewModel()
        {
            //DataConnection.Client_Start();
            GetSettings();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            DisplayDateTextBlock = DateTime.Now.ToString(@"HH:mm:ss");
            PlantT = DataConnection.PlantT;

        }
    }
}

