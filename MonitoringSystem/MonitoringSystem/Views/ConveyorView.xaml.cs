using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// ConveyorView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConveyorView : UserControl
    {
        private string serverIpNum = "192.168.21.186";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private string clientId = "SCADA_system";
        private string factoryId = "Kasan01";            //  Kasan01/4001/  kasan01/4002/ 
        private string ConveyorAddr = "4002";
        private string LobotAddr = "4001";

        private string connectionString = string.Empty;
        private MqttClient client;

        public ConveyorView()
        {
            InitializeComponent();
            InitAllCustomComponnet();
        }

        private void InitAllCustomComponnet()
        {
            
            // IPAddress serverAddress = IPAddress.Parse(serverIpNum);
            client = new MqttClient(serverIpNum);
            client.MqttMsgPublished += Client_MqttMsgPublished;
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.ConnectionClosed += Client_ConnectionClosed;

            connectionString = "Data Source=localhost;Initial Catalog=HMI_Data;Integrated Security=True"; //바꿔야댐
            client.Connect(clientId);
            client.Subscribe(new string[] { $"{factoryId}/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

            

        }

        private void Client_ConnectionClosed(object sender, EventArgs e)
        {
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //넘어어노느센서값 처리
        }

        private void Client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
        }



        #region 애니매이션
        private void RunStop_Click(object sender, RoutedEventArgs e)
        {
            if (IsRun == false)
            {
                StartAnimation();
                StartAnimation1();
                IsRun = true;

                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("DeviceId", clientId);
                pairs.Add("Topic", "Kasan01/4002");
                pairs.Add("CurrTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                pairs.Add("Values", "TempValue");

                var rawData = JsonConvert.SerializeObject(pairs, Formatting.Indented);

                // 메서드에 아래 
                client.Publish("Kasan01/4002", Encoding.UTF8.GetBytes(rawData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }
            else
            {
                StopAnimation();
                IsRun = false;
            }
        }
        private bool IsRun = false;
        private void StopAnimation()
        {
            Motor1.RenderTransform = null;
            Motor2.RenderTransform = null;
            Motor3.RenderTransform = null;
            Motor4.RenderTransform = null;
            Arm.RenderTransform = null;
        }
        public void StartAnimation()
        {
            // 애니메이션 gg
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 360;
            da.Duration = TimeSpan.FromSeconds(2);
            da.RepeatBehavior = RepeatBehavior.Forever;

            RotateTransform rt = new RotateTransform();
            Motor1.RenderTransform = rt;
            Motor1.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor2.RenderTransform = rt;
            Motor2.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor3.RenderTransform = rt;
            Motor3.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor4.RenderTransform = rt;
            Motor4.RenderTransformOrigin = new Point(0.5, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
        private void StartAnimation1()
        {
            // 애니메이션 gg
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = -40;
            da.Duration = TimeSpan.FromSeconds(2);
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;

            RotateTransform rt = new RotateTransform();
            Arm.RenderTransform = rt;
            Arm.RenderTransformOrigin = new Point(0, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
        #endregion
    }
}
