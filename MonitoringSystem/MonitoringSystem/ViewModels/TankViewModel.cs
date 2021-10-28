using Caliburn.Micro;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.ViewModels
{
    public class TankViewModel : Conductor<object>
    {
        #region ### 변수 생성 ###

        private string serverIpNum = "192.168.0.201";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private string clientId = "학원";
        private string factoryId = "Kasan01";            //  Kasan01/4001/  kasan01/4002/ 
        private string factoryId2 = "Kasan02";            //  Kasan01/4001/  kasan01/4002/ 
        private string connectionString = "Data Source=hangaramit.iptime.org;Initial Catalog=1조_database;Persist Security Info=True;User ID=team1;Password=team1_1234";        

        private Thread task;
        private Thread taskstop;
        private readonly object _lock = new object();

        #endregion

        #region ### 생성자 생성 ###

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

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
        // 펌프가 멈췄는지 확인
        private bool isStop =true;
        public bool IsStop
        {
            get => isStop;
            set
            {
                isStop = value;
                NotifyOfPropertyChange(() => IsStop);
            }
        }

        // 펌프가 작동중인지 확인
        private bool work = true;
        public bool Work
        {
            get => work;
            set
            {
                work = value;
                NotifyOfPropertyChange(() => Work);
            }
        }
        private bool _isEnable = true;
        public bool isEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                NotifyOfPropertyChange(() => isEnable);
            }
        }
        // Pump 작동 색 지정
        private string btnColor;
        public string BtnColor
        {
            get => btnColor;
            set
            {
                btnColor = value;
                NotifyOfPropertyChange(() =>BtnColor);
            }
        }
        // MainTankValue 계산
        private double mainTankValue;
        public double MainTankValue
        {
            get => mainTankValue;
            set
            {
                if(value>=1000)
                {
                    mainTankValue = 100;
                }
                else if (1000 > value && value >= 600)
                {
                    mainTankValue = Math.Round(value / 1000 * 100, 2);
                }
                else
                {
                    mainTankValue = Math.Round(value / 1200 * 100, 2);
                }
                NotifyOfPropertyChange(() => MainTankValue);
            }
        }

        // SubTankValue 계산
        private double subTankValue;
        public double SubTankValue
        {
            get => subTankValue;
            set
            {
                if (value >= 1000)
                {
                    subTankValue = 100;
                }
                else if (1000 > value && value >= 600)
                {
                    subTankValue = Math.Round(value / 1000 * 100, 2);
                }
                else if ( value < 300)
                {
                    subTankValue = 50;
                }
                
                NotifyOfPropertyChange(() => SubTankValue);
            }
        }

        // MainTankTon 계산
        private double mainTankTon;
        public double MainTankTon
        {
            get => mainTankTon;
            set
            {
                mainTankTon = value;
                NotifyOfPropertyChange(() => MainTankTon);
            }
        }
        // SubTankTon 계산
        private double subTankTon;
        public double SubTankTon
        {
            get => subTankTon;
            set
            {
                if (subTankTon <100)
                {
                    subTankTon = 50;
                }
                subTankTon = value;
                NotifyOfPropertyChange(() => SubTankTon);
            }
        }

        private PlotModel plotViewModel;
        public PlotModel PlotViewModel
        {
            get => plotViewModel;
            set
            {
                plotViewModel = value;
                OnPropertyChanged("PlotViewModel");
            }
        }
        Task T;
        private Visibility _Video = Visibility.Visible;
        public Visibility Video
        {
            get => _Video;
            set
            {
                _Video = value;
                NotifyOfPropertyChange(() => Video);
            }
        }
        #endregion

        #region ### LOADING + EVENT + OXY PLOT SETTINGS ###
        // 화면 로드 되자마자 MQTT에 접속, 이벤트 처리

        private LinearAxis linearAxis1;
        private LinearAxis linearAxis2;

        public TankViewModel()
        {
            Client = new MqttClient(serverIpNum);
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.ConnectionClosed += Client_ConnectionClosed;

            Client.Connect(clientId);
            Client.Subscribe(new string[] { $"{factoryId}/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            var plotModel1 = new PlotModel();
            var plotModel2 = new PlotModel();

            var dateTimeAxis1 = new DateTimeAxis();
            var dateTimeAxis2 = new DateTimeAxis();

            plotModel1.Axes.Add(dateTimeAxis1);
            plotModel2.Axes.Add(dateTimeAxis2);

            linearAxis1 = new LinearAxis()
            {
                AbsoluteMaximum = 100,
            };
            linearAxis2 = new LinearAxis()
            {
                AbsoluteMaximum = 100,
            };

            linearAxis1.Minimum = 0;
            linearAxis2.Minimum = 0;
            linearAxis1.Maximum = 100;
            linearAxis2.Maximum = 100;
            linearAxis1.IsZoomEnabled = false;
            linearAxis2.IsZoomEnabled = false;

            plotModel1.Axes.Add(linearAxis1);
            plotModel2.Axes.Add(linearAxis2);
            var lineSeries1 = new LineSeries();
            var lineSeries2 = new LineSeries();
            lineSeries1.MarkerType = MarkerType.Circle;
            lineSeries2.MarkerType = MarkerType.Circle;

            lineSeries1.Points.Add(new DataPoint());
            lineSeries2.Points.Add(new DataPoint());

            plotModel1.Series.Add(lineSeries1);
            plotModel2.Series.Add(lineSeries2);

            plotViewModel = plotModel1;
            plotViewModel = plotModel2;

            plotViewModel.InvalidatePlot(true); // 실시간 업데이트 

            T = new Task(FunctionA);
            T.Start();
        } 
        ~TankViewModel()
        {

        }
        #endregion

        #region ### Tank 수위 실시간 감지 ###
        public void Feedback()
        {
            try
            {
                while (IsStop)
                {
                    if (work)
                    {
                        // sub tank 수위가 높을경우 stop
                        if (subTankTon < 300)
                        {
                            BtnClickOn();
                            // subTank 수위가 낮아 mainpump 가동 시작
                            App.Logger.Info("subTank 수위가 낮아 mainpump 가동 시작");

                        }
                        else if (subTankTon > 630)
                        {
                            BtnClickOff();
                            // subTank 수위가 높아 mainpump 가동 시작
                            App.Logger.Info("subTank 수위가 높아 mainpump 가동 시작");

                            BtnClick2On();
                            // subTank 수위가 높아 subpump 가동 시작
                            App.Logger.Info("subTank 수위가 높아 subpump 가동 시작");
                            Thread.Sleep(50000);
                            BtnClick2Off();
                        }
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Thread.Yield();
                        App.Logger.Info("자동모드 종료");
                        BtnClickOff();
                        BtnClick2Off();
                        Thread.Sleep(60000);
                    }
                }

            }
            catch (Exception ex)
            {
                BtnClickOff();
                Debug.WriteLine($"Feedback() Excepteion  BtnClickOff() ");
                BtnClick2Off();
                Debug.WriteLine($"Feedback() Excepteion  BtnClick2Off() ");
                MessageBox.Show("Feedback() Task 오류발생", ex.ToString());
            }
            finally
            {
                BtnClickOff();
                BtnClick2Off();
            }

            Debug.WriteLine("Out of While");
        }
        #endregion

        #region ### AUTO ###
        public void AutoRun()
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

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                    BtnColor = "Red";
                }, DispatcherPriority.Normal);

                IsStop = true;
                work = true;
                task = new Thread(Feedback);

                task.Start();
                // 자동모드 시작
                App.Logger.Info("자동모드 시작");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"main pump on 접속 오류 { ex.Message}");
            }
        }
        #endregion

        #region 정지 로직 구현 어찌할 지 고민
        public void AutoStop()
        {
            // Thread 정지 이벤트 발생
            //var t = Task.Run(() => { Feedback(); });
            //t.Wait(60000);
            taskstop = new Thread(ThreadStop);
            taskstop.Start();
        }

        private void ThreadStop()
        {
            work = false;
        }

        #endregion

        #region ### 펌프제어 Publish ### 
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

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                    BtnColor = "Red";
                }, DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"main pump on 접속 오류 { ex.Message}");
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
                
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                    BtnColor = "Gray";
                }, DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"main pump off 접속 오류 { ex.Message}");
            }
        }

        public void BtnClick2On()
        {
            // Publish 펌프 제어 ON
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4005\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"pump2\", \n" +
                                 "   \"value\" : \"1\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Client.Publish($"{factoryId2}/4005/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                }, DispatcherPriority.Normal);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"sub pump on 접속 오류 { ex.Message}");
            }
        }
        public void BtnClick2Off()
        {
            // Publish 펌프 제어 OFF
            try
            {
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                  "   \"dev_addr\" : \"4005\", \n" +
                                  $"   \"currtime\" : \"{currtime}\" , \n" +
                                  "   \"code\" : \"pump2\", \n" +
                                  "   \"value\" : \"0\", \n" +
                                  "   \"sensor\" : \"0\" \n" +
                                  "}";

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Client.Publish($"{factoryId2}/4005/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                }, DispatcherPriority.Normal);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"sub pump off 접속 오류 { ex.Message}");
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
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        MainTankValue = int.Parse(currData["sensor"]);
                        MainTankTon = int.Parse(currData["sensor"]);
                        LblStatus = message;
                    }, DispatcherPriority.Normal);
                    
                }
                else if (currData["dev_addr"] == "4001" && currData["code"] == "SubTank")
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        SubTankValue = int.Parse(currData["sensor"]);
                        SubTankTon = int.Parse(currData["sensor"]);
                        LblStatus1 = message;
                    }, DispatcherPriority.Normal);
                    
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

        #region ### OxyPlot ###

        public void FunctionA()
        {
            DateTime curtime;
            var lineSeries1 = new LineSeries();
            var lineSeries2 = new LineSeries();

            lineSeries1.MarkerType = MarkerType.Circle;
            lineSeries2.MarkerType = MarkerType.Circle;

            // List < DataPoint > 는 기본적으로 Plot의 X,Y 값을 갖는다.
            List<DataPoint> lData = new List<DataPoint>();
            List<DataPoint> lData2 = new List<DataPoint>();

            DateTime StartDate;
            DateTime EndDate;

            while (true)
            {
                Thread.Sleep(100);
                plotViewModel.Series.Clear();

                lineSeries1.Points.Clear();
                lineSeries2.Points.Clear();

                curtime = DateTime.Now;

                // 현재 시간 Datetime -> double 변환
                double dCurtime = curtime.ToOADate();
                double dValue = subTankValue;
                double dValue2 = mainTankValue;

                if (lData.Count ==0 & lData2.Count == 0)
                {
                    lData.Add(new DataPoint(dCurtime, dValue));
                    lData2.Add(new DataPoint(dCurtime, dValue2));
                }
                else
                {
                    // X 좌표 double -> Datetime 으로 바꿔서 20초 차이를 계산하는 로직 
                    StartDate = DateTime.FromOADate(lData[0].X);
                    EndDate = DateTime.FromOADate(lData[lData.Count - 1].X);
                    double Diff = Math.Abs((StartDate - EndDate).Seconds);

                    if (Diff <= 10)
                    {
                        lData.Add(new DataPoint(dCurtime, dValue));
                        lData2.Add(new DataPoint(dCurtime, dValue2));

                    }
                    else
                    {
                        lData.RemoveAt(0);
                        lData2.RemoveAt(0);

                        lData.Add(new DataPoint(dCurtime, dValue));
                        lData2.Add(new DataPoint(dCurtime, dValue2));

                    }
                }

                foreach (var data in lData)
                {
                    lineSeries1.Points.Add(data);
                }
                foreach (var data in lData2)
                {
                    lineSeries2.Points.Add(data);
                }

                plotViewModel.Series.Add(lineSeries1);
                plotViewModel.Series.Add(lineSeries2);

                PlotViewModel.InvalidatePlot(true);

            }
        }
        #endregion

        #region ### CCTV PLAY BUTTON HIDDEN ###
        public void Play_Button()
        {
            Video = Visibility.Hidden;
        }

        #endregion

    }
}
