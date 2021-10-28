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

        private static string serverIpNum = "192.168.0.195";  // 윈도우(MQTT Broker, SQLServer) 아이피
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

        #region Conveyor 변수
        private static double robotTemp;
        public static double RobotTemp
        {
            get => robotTemp;
            set
            {
                robotTemp = value;
                NotifyPropertyChange(() => RobotTemp);
            }
        }

        // ConveyTemp
        private static double conveyTemp;
        public static double ConveyTemp
        {
            get => conveyTemp;
            set
            {
                conveyTemp = value;
                NotifyPropertyChange(() => ConveyTemp);
            }
        }

        #endregion

        private static double plantT;

        public static double PlantT
        {
            get => plantT;
            set
            {
                plantT = value;
                NotifyPropertyChange(() => PlantT);
            }
        }

        private static double plantH;

        public static double PlantH
        {
            get => plantH;
            set
            {
                plantH = value;
                NotifyPropertyChange(() => PlantH);
            }
        }

        private static double duty;

        #region Gas 변수
        private static double gas1;
        private static double gas2;
        private static double gas3;
        private static double gas4;
        private static double gas5;
        private static double gas6;

        public static double Gas1
        {
            get => gas1;
            set
            {
                gas1 = value;
                NotifyPropertyChange(() => Gas1);
            }
        }
        public static double Gas2
        {
            get => gas2;
            set
            {
                gas2 = value;
                NotifyPropertyChange(() => Gas2);
            }
        }
        public static double Gas3
        {
            get => gas3;
            set
            {
                gas3 = value;
                NotifyPropertyChange(() => Gas3);
            }
        }
        public static double Gas4
        {
            get => gas4;
            set
            {
                gas4 = value;
                NotifyPropertyChange(() => Gas4);
            }
        }
        public static double Gas5
        {
            get => gas5;
            set
            {
                gas5 = value;
                NotifyPropertyChange(() => Gas5);
            }
        }
        public static double Gas6
        {
            get => gas6;
            set
            {
                gas6 = value;
                NotifyPropertyChange(() => Gas6);
            }
        }
        #endregion
        public static double Duty
        {
            get => duty;
            set
            {
                duty = value;
                NotifyPropertyChange(() => Duty);
            }
        }

        private static string eMessage;
        public static string EMessage
        {
            get => eMessage;
            set
            {
                eMessage = value;
            }
        }

        private static string solution;
        
        public static string Solution
        {
            get => solution;
            set
            {
                solution = value;
            }
        }


        #endregion
        #region DHT22 MQTT 통신

        public static void Client_Start()
        {
            Client = new MqttClient(serverIpNum);
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.Connect(clientId);
            Client.Subscribe(new string[] { $"{factoryId}/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        public static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Message); //e.Message(byte[]) ==> string 변환
                var currData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                if (currData["dev_addr"] == "4006" && currData["code"] == "PlantT") // DHT22에서 데이터 수신
                {
                    PlantT = double.Parse(currData["sensor"]);
                }
                else if (currData["dev_addr"] == "4011" && currData["code"] == "Duty") // ConveyTemp 데이터 수신
                {
                    Duty = double.Parse(currData["sensor"]);
                    
                }

                #region Conveyor
                // Conveyor Temp 값 받아오기

                else if (currData["dev_addr"] == "4004" && currData["code"] == "RobotTemp") // RobotTemp 데이터 수신
                {
                    RobotTemp = double.Parse(currData["sensor"]);
                    InsertData(currData);

                }
                else if (currData["dev_addr"] == "4004" && currData["code"] == "ConveyTemp") // ConveyTemp 데이터 수신
                {
                    ConveyTemp = double.Parse(currData["sensor"]);
                    InsertData(currData);
                }
                #endregion

                #region 가스값 센서
                else if (currData["dev_addr"] == "4010")
                {
                    InsertData(currData);
                    switch (currData["value"])
                    {
                        case "1":
                            Gas1 = double.Parse(currData["sensor"]);
                            break;
                        case "2":
                            Gas2 = double.Parse(currData["sensor"]);
                            break;
                        case "3":
                            Gas3 = double.Parse(currData["sensor"]);
                            break;
                        case "4":
                            Gas4 = double.Parse(currData["sensor"]);
                            break;

                        case "5":
                            Gas5 = double.Parse(currData["sensor"]);
                            break;

                        case "6":
                            Gas6 = double.Parse(currData["sensor"]);
                            break;
                        case "7":
                            PlantH = double.Parse(currData["sensor"]);
                            break;
                        case "8":
                            PlantT = double.Parse(currData["sensor"]);
                            break;

                    }
                }

                #endregion
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
                    App.Logger.Info($"예외 발생, InsertData : [{ex.Message}]");
                }
                conn.Close();
            }
        }


        #endregion
    }
}
