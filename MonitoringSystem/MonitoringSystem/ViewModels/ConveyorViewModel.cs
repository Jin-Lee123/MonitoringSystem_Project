using Caliburn.Micro;
using MonitoringSystem.Models;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.ViewModels
{
    public class ConveyorViewModel : Conductor<object>
    {
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> YFormatter2 { get; set; }

        private string serverIpNum = "192.168.0.195";   //학원 "192.168.0.195"; //핸드폰 "192.168.21.186";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private string clientId = "학원";
        private string factoryId = "kasan01";            //  Kasan01/4001/  kasan01/4002/ 
        private string motorAddr = "4002";
        private string tankAddr = "4001";
        private string connectionString = "Data Source=hangaramit.iptime.org;Initial Catalog=1조_database;Persist Security Info=True;User ID=team1;Password=team1_1234";

        private VideoCapture _videoCapture = null; // 캠 캡쳐

        #region ### 생산량 불량률 생성자 생성 ### 삭제할수도
        private BindableCollection<TB_Line> TB_Line;

        public BindableCollection<TB_Line> Line
        {
            get => TB_Line;
            set
            {
                TB_Line = value;
                NotifyOfPropertyChange(() => Line);
            }
        }
        private int plantcode;
        public int Plantcode
        {
            get => plantcode;
            set
            {
                plantcode = value;
                NotifyOfPropertyChange(() => Plantcode);
            }
        }
        private int goalqty;
        public int GoalQty
        {
            get => goalqty;
            set
            {
                goalqty = value;
                NotifyOfPropertyChange(() => GoalQty);
            }
        }
        private int totalqty;
        public int TotalQty
        {
            get => totalqty;
            set
            {
                totalqty = value;
                NotifyOfPropertyChange(() => TotalQty);
            }
        }
        private int prodqty;
        public int ProdQty
        {
            get => prodqty;
            set
            {
                prodqty = value;
                NotifyOfPropertyChange(() => ProdQty);
            }
        }
        private int badqty;
        public int BadQty
        {
            get => badqty;
            set
            {
                badqty = value;
                NotifyOfPropertyChange(() => BadQty);
            }
        }
        private string woker;
        public string Woker
        {
            get => woker;
            set
            {
                woker = value;
                NotifyOfPropertyChange(() => Woker);
            }
        }
        #endregion

        #region MQTT 생성자 RobotArm, Conveyor
        private MqttClient client;
        public MqttClient Client
        {
            get => client;
            set
            {
                client = value;
                NotifyOfPropertyChange(() => Client);
            }
        }

        // 확인용 Label --삭제 예정
        private string lblStatus;
        public string LblStatus
        {
            get => lblStatus;
            set
            {
                lblStatus = value;
                NotifyOfPropertyChange(() => LblStatus);
            }
        }

        // 확인용 Label --삭제 예정
        private string lblStatus1;
        public string LblStatus1
        {
            get => lblStatus1;
            set
            {
                lblStatus1 = value;
                NotifyOfPropertyChange(() => LblStatus1);
            }
        }

        // RobotTemp
        private string robotTemp;
        public string RobotTemp
        {
            get => robotTemp;
            set
            {
                robotTemp = value;
                NotifyOfPropertyChange(() => RobotTemp);
            }
        }

        // ConveyTemp
        private string conveyTemp;
        public string ConveyTemp
        {
            get => conveyTemp;
            set
            {
                conveyTemp = value;
                NotifyOfPropertyChange(() => ConveyTemp);
            }
        }

        // Duty 미세먼지
        private string duty;
        public string Duty
        {
            get => duty;
            set
            {
                duty = value;
                NotifyOfPropertyChange(() => Duty);
            }
        }
        
        // MQ5 LPG부탄프로탄가스센서
        private string mq5;
        public string MQ5
        {
            get => mq5;
            set
            {
                mq5 = value;
                NotifyOfPropertyChange(() => MQ5);
            }
        }
        #endregion

        //MqttClient Clients;
        #region ### 화면 로딩 + 이벤트 ###
        public ConveyorViewModel()
        {
            //Clients = _Client;
            // 화면 로드 되자마자 MQTT에 접속, 이벤트 처리
            Client = new MqttClient(serverIpNum);
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.ConnectionClosed += Client_ConnectionClosed;

            Client.Connect(clientId);
            Client.Subscribe(new string[] { $"{factoryId}/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            GetEmployees();
        }
        #endregion

       
        private void GetEmployees() // 1. SELECT 문
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                conn.Open();
                string selquery = Models.TB_Line.SELECT_QUERY;
                SqlCommand cmd = new SqlCommand(selquery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                Line = new BindableCollection<TB_Line>();

                while (reader.Read())
                {
                    var empTmp = new TB_Line
                    {
                        Plantcode = (int)reader["Plantcode"],
                        GoalQty = (int)reader["GoalQty"],
                        TotalQty = (int)reader["TotalQty"],
                        ProdQty = (int)reader["ProdQty"],
                        BadQty = (int)reader["BadQty"],
                        Woker = reader["Woker"].ToString(),
                        
                    };
                    Line.Add(empTmp);
                    
                }
                conn.Close();
            }

            TotalQty = Line[0].TotalQty;
            ProdQty = Line[0].ProdQty;
            BadQty = Line[0].BadQty;
            GoalQty = Line[0].GoalQty;

            YFormatter = (val) => $"{(val / GoalQty) * 100}%";
            YFormatter2 = (val) => $"{(val / TotalQty) * 100}%";
          
        }

        private void Client_ConnectionClosed(object sender, EventArgs e)   // 모니터링 종료
        {
            LblStatus = "모니터링 종료!";
        }

        public void ConveyRun() 
        {
            //컨베이어 작동(시계방향)
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4002\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Convey\", \n" +
                                 "   \"value\" : \"1\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }

            // DB입력
            App.Logger.Fatal(new Exception("컨베이어"), "정뱡향 작동");
            //

        } //컨베이어 작동(시계방향)

        public void ConveyBack()
        {
            // Publish 컨베이어 작동(반시계방향)
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4002\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Convey\", \n" +
                                 "   \"value\" : \"2\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                
                Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }

            // DB입력
            App.Logger.Fatal(new Exception("컨베이어"), "역뱡향 작동");
            //
        } //컨베이어 작동(반시계방향)

        public void ConveyStop()
        {
            // Publish 컨베이어 Stop
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4002\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Convey\", \n" +
                                 "   \"value\" : \"3\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }
            // DB입력
            App.Logger.Fatal(new Exception("컨베이어"), "정지");
            //
        } //컨베이어 비작동

        public void ArmRun()
        {
            // Publish 로봇팔 Run
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4001\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Arm\", \n" +
                                 "   \"value\" : \"1\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                Client.Publish($"{factoryId}/4001/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }
            // DB입력
            App.Logger.Fatal(new Exception("로봇팔"), "양품");
            //
        }  // 로봇팔 좌회전

        public void ArmRun2()
        {
            // Publish 로봇팔 Run
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4001\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Arm\", \n" +
                                 "   \"value\" : \"2\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                Client.Publish($"{factoryId}/4001/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }
            // DB입력
            App.Logger.Fatal(new Exception("로봇팔"), "불량");
            //
        } // 로봇팔 우회전

        public void AutoLabel()
        {

        }

        #region ### MQTT Subscribe ###   // Subscribe 한 값을 바인딩 해주는 곳
        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Message); //e.Message(byte[]) ==> string 변환
                // JSON 넘어온 데이터를 확인 후 내부 SCADA 작업
                //"dev_addr" : "4001",
                //"currtime" : "2021-08-26 11:05:30 ",
                //"code" : "red",
                //"value" : "1"
                var currData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                if (currData["dev_addr"] == "4003" && currData["code"] == "RobotTemp") // RobotTemp 데이터 수신
                {
                    RobotTemp = currData["sensor"];
                    LblStatus = message;

                }
                else if (currData["dev_addr"] == "4003" && currData["code"] == "ConveyTemp") // ConveyTemp 데이터 수신
                {
                    ConveyTemp = currData["sensor"];
                    LblStatus1 = message;
                }
                //InsertData(currData);
                else if (currData["dev_addr"] == "4004" && currData["code"] == "Duty") // ConveyTemp 데이터 수신
                {
                    Duty = currData["sensor"];
                    LblStatus1 = message;
                }
                else if (currData["dev_addr"] == "4005" && currData["code"] == "MQ5") // ConveyTemp 데이터 수신
                {
                    MQ5 = currData["sensor"];
                    LblStatus1 = message;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //App.LOGGER.Info($"예외발생, Client_MqttMsgPublishReceived : [{ex.Message}]");
            }
        }
        #endregion


        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)  // 창을 종료할 때 Mqtt Client 종료
        {
            if (Client.IsConnected) Client.Disconnect();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
