﻿using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.ViewModels
{
    public class TankViewModel : Conductor<object>
    {
        #region ### 변수 생성 ###

        private string serverIpNum = "192.168.56.1";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private string clientId = "학원";
        private string factoryId = "Kasan01";            //  Kasan01/4001/  kasan01/4002/ 
        private string motorAddr = "4002";
        private string tankAddr = "4001";
        private string connectionString = "Data Source=hangaramit.iptime.org;Initial Catalog=1조_database;Persist Security Info=True;User ID=team1;Password=team1_1234";

        #endregion

        #region ### 생성자 생성 ###

        // MQTT Client 생성
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

        // MainTankValue 계산
        private double mainTankValue;
        public double MainTankValue
        {
            get => mainTankValue;
            set
            {
                if (value >= 700)
                {
                    mainTankValue = 700 / 700 * 100;
                }
                else
                {
                    mainTankValue = Math.Round(value / 700 * 100);
                }
                NotifyOfPropertyChange(() => MainTankValue);
            }
        }
        // MainTankTon 계산
        private double mainTankTon;
        public double MainTankTon
        {
            get => mainTankTon;
            set
            {
                if (value >= 700)
                {
                    mainTankTon = 700 * 400;
                }
                else
                { 
                    mainTankTon = Math.Round(value / 700 * 400);
                }
                NotifyOfPropertyChange(() => MainTankTon);
            }
        }
        // SubTankValue 계산
        private double subTankValue;
        public double SubTankValue
        {
            get => subTankValue;
            set
            {
                if(value>=630)
                {
                    subTankValue = 630 / 630 * 100;
                }
                else
                {
                    subTankValue = Math.Round(value / 630 * 100);
                }
                NotifyOfPropertyChange(() => SubTankValue);
            }
        }
        // SubTankTon 계산
        private double subTankTon;
        public double SubTankTon
        {
            get => subTankTon;
            set
            {
                if (value >= 630)
                {
                    subTankTon = 630 / 630 * 100;
                }
                else
                {
                    subTankTon = Math.Round(value / 630 * 100);
                }
            NotifyOfPropertyChange(() => SubTankTon);
            }
        }
        #endregion

        #region ### 화면 로딩 + 이벤트 ###
        // 화면 로드 되자마자 MQTT에 접속, 이벤트 처리
        public TankViewModel()
        {
            Client = new MqttClient(serverIpNum);
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.ConnectionClosed += Client_ConnectionClosed;

            Client.Connect(clientId);
            Client.Subscribe(new string[] { $"{factoryId}/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        } 
        #endregion

        #region ### 펌프제어 ### -- 이후 버튼 Publish에 사용할 예정
        public void BtnClickOn()
        {
            // Publish 펌프 제어 ON
            try
            {
                 var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                 string pubData = "{ \n" +
                                  "   \"dev_addr\" : \"4002\", \n" +
                                  $"   \"currtime\" : \"{currtime}\" , \n" +
                                  "   \"code\" : \"pump\", \n" +
                                  "   \"value\" : \"1\", \n" +
                                  "   \"sensor\" : \"0\" \n" +
                                  "}";
            
                 Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }
        }
        public void BtnClickOff()
        {
            // Publish 펌프 제어 OFF
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                  "   \"dev_addr\" : \"4002\", \n" +
                                  $"   \"currtime\" : \"{currtime}\" , \n" +
                                  "   \"code\" : \"pump\", \n" +
                                  "   \"value\" : \"0\", \n" +
                                  "   \"sensor\" : \"0\" \n" +
                                  "}";

                Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"접속 오류 { ex.Message}");
            }
        }
        #endregion

        #region ### MQTT Subscribe ###
        // Subscribe 한 값을 바인딩 해주는 곳
        public void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
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

                if (currData["dev_addr"] == "4001" && currData["code"] == "MainTank") // MainTank에서 데이터 수신
                {
                    MainTankValue = int.Parse(currData["sensor"]);
                    MainTankTon = int.Parse(currData["sensor"]);
                    LblStatus = message;
                }
                else if (currData["dev_addr"] == "4001" && currData["code"] == "SubTank")
                {
                    SubTankValue = int.Parse(currData["sensor"]);
                    SubTankTon = int.Parse(currData["sensor"]);
                    LblStatus1 = message;
                }
                InsertData(currData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //App.LOGGER.Info($"예외발생, Client_MqttMsgPublishReceived : [{ex.Message}]");
            }
        }
        #endregion

        #region ### SQL Save ###
        // SQL SERVER 저장
        public void InsertData(Dictionary<string, string> currData)
        {
            using (var conn = new SqlConnection(connectionString))  // close 자동
            {
                string insertQuery = $@"INSERT INTO TB_MainTank
                                         VALUES
                                               ('{currData["dev_addr"]}'
                                               ,'{currData["currtime"]}'
                                               ,'{currData["code"]}'
                                               ,'{currData["value"]}'
                                               ,'{currData["sensor"]}')";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(insertQuery, conn);

                    if (cmd.ExecuteNonQuery() == 1) // 전송 성공
                    {
                        //App.LOGGER.Info("IoT 데이터 입력 성공!");
                    }
                    else
                    {
                        //App.LOGGER.Info($"오류 발생, InsertData 데이터 입력 실패 : [{insertQuery}]");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    // App.LOGGER.Info($"예외 발생, InsertData : [{ex.Message}]");
                }
                conn.Close();
            }
        }
        #endregion

        #region ### DisConnect ###
        // MQTT 서버와 접속이 끊어졌을때 이벤트 처리
        private void Client_ConnectionClosed(object sender, EventArgs e)
        {
            LblStatus = "모니터링 종료!";
        }


        // 창을 종료할 때 Mqtt Client 종료
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (Client.IsConnected) Client.Disconnect();
            return base.OnDeactivateAsync(close, cancellationToken);
        } 
        #endregion

    }
}
