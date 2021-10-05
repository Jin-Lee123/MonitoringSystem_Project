using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.Models
{
    public class DataConnection
    {
        public static event PropertyChangedEventHandler PropertyChanged;

        private static void NotifyPropertyChange<T>(Expression<Func<T>> property)
        { 
            string propertyName = (((MemberExpression)property.Body).Member as PropertyInfo).Name;
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
        #region ### 변수 생성 ###

        private static string serverIpNum = "192.168.0.198";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private static string clientId = "Monitoring";
        private static string factoryId = "kasan01";            //  Kasan01/4001/  kasan01/4002/ 
        private static string connectionString = "Data Source=hangaramit.iptime.org;Initial Catalog=1조_database;Persist Security Info=True;User ID=team1;Password=team1_1234";
                 
        private static MqttClient client;            
        public static MqttClient Client
        {
            get => client;
            set
            {
                client = value;
                NotifyPropertyChange(() => Client);
            }
        }

        private static float plantT;

        public static float PlantT
        {
            get => plantT;
            set
            {
                plantT = value;
                NotifyPropertyChange(() => PlantT);
            }
        }
        #endregion
        #region DHT22 MQTT 통신

        public static void Client_Start()
        {
            Client = new MqttClient(serverIpNum);
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.Connect(clientId);
            Client.Subscribe(new string[] { $"{factoryId}/4006/" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        public static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Message); //e.Message(byte[]) ==> string 변환
                var currData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                if (currData["dev_addr"] == "4006" && currData["code"] == "PlantT") // DHT22에서 데이터 수신
                {
                    PlantT = float.Parse(currData["sensor"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //App.LOGGER.Info($"예외발생, Client_MqttMsgPublishReceived : [{ex.Message}]");
            }
        }
        public static void InsertData(Dictionary<string, string> currData)
        {
            using (var conn = new SqlConnection(connectionString))  // close 자동
            {
                string insertQuery = $@"INSERT INTO TB_Plant
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
    }
}
