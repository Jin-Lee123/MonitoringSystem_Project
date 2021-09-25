using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private string serverIpNum = "192.168.0.195";   //학원 "192.168.0.195"; //핸드폰 "192.168.21.186";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private string clientId = "학원";
        private string factoryId = "kasan01";            //  Kasan01/4001/  kasan01/4002/ 
        private string motorAddr = "4002";
        private string tankAddr = "4001";
        private string connectionString = "Data Source=hangaramit.iptime.org;Initial Catalog=1조_database;Persist Security Info=True;User ID=team1;Password=team1_1234";

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


        #region ### 화면 로딩 + 이벤트 ###
        public ConveyorViewModel()
        {
            // 화면 로드 되자마자 MQTT에 접속, 이벤트 처리
            Client = new MqttClient(serverIpNum);
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.ConnectionClosed += Client_ConnectionClosed;

            Client.Connect(clientId);
            Client.Subscribe(new string[] { $"{factoryId}/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }
        #endregion

        private void Client_ConnectionClosed(object sender, EventArgs e)
        {
            LblStatus = "모니터링 종료!";
        }

        public void ConveyRun()
        {
            // Publish 컨베이어 작동(시계방향)
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
        } //컨베이어 비작동

        public void ArmRun()
        {
            // Publish 로봇팔 Run
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4002\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Arm\", \n" +
                                 "   \"value\" : \"1\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }
        }  // 로봇팔 좌회전

        public void ArmStop()
        {
            // Publish 로봇팔 Run
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4002\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Arm\", \n" +
                                 "   \"value\" : \"2\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }
        } // 로봇팔 우회전


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

                if (currData["dev_addr"] == "4001" && currData["code"] == "RobotTemp") // RobotTemp 데이터 수신
                {
                    RobotTemp = currData["sensor"];
                    LblStatus = message;

                }
                else if (currData["dev_addr"] == "4001" && currData["code"] == "ConveyTemp") // ConveyTemp 데이터 수신
                {
                    ConveyTemp = currData["sensor"];
                    LblStatus1 = message;
                }
                //InsertData(currData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //App.LOGGER.Info($"예외발생, Client_MqttMsgPublishReceived : [{ex.Message}]");
            }
        }
        #endregion


        // 창을 종료할 때 Mqtt Client 종료
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (Client.IsConnected) Client.Disconnect();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}
