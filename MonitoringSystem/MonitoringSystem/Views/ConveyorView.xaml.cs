using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// ConveyorView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConveyorView : UserControl, INotifyPropertyChanged
    {
        //mqttㅇㅇ
        private string serverIpNum = "192.168.0.195";   //학원 "192.168.0.195"; //핸드폰 "192.168.21.186";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private string clientId = "SCADA_system";
        private string factoryId = "kasan01";
        private MqttClient client;
        
        //opencv
        private const string windowName = "src";

        //아이피(라즈베리아이피) 바꿔줘야댐 192.168.191.185   192.168.0.14
        private string RtspUrl = "rtsp://192.168.0.13:9000";   
        private DirectoryInfo libDirectory;
        Uri uri;

        public ConveyorView()
        {
            InitializeComponent();
            InitAllCustomComponnet();//mqtt
            //CCTV
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            image.SourceProvider.CreatePlayer(libDirectory);
            uri = new Uri(RtspUrl);
            image.SourceProvider.MediaPlayer.Play(uri);

            //화살표
            Value = 160;
            DataContext = this;
        }

        #region MQTT
        private void InitAllCustomComponnet()
        {
            client = new MqttClient(serverIpNum);
            client.MqttMsgPublished += Client_MqttMsgPublished;
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.Connect(clientId);
            client.Subscribe(new string[] { $"{factoryId}/4003/" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        private void Client_ConnectionClosed(object sender, EventArgs e)
        {
           
        }

        private void Client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Message); //e.Message(byte[]) ==> string 변환
                var currData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                if (currData["dev_addr"] == "4003" && currData["code"] == "Conveydist" && int.Parse(currData["sensor"]) <= 100) // RobotTemp 데이터 수신
                {
                    Thread.Sleep(1000);
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

                        client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                        
                        AutoPlay();
                        
                        Thread.Sleep(8000);
                       
                        pubData = "{ \n" +
                                         "   \"dev_addr\" : \"4002\", \n" +
                                         $"   \"currtime\" : \"{currtime}\" , \n" +
                                         "   \"code\" : \"Convey\", \n" +
                                         "   \"value\" : \"1\", \n" +
                                         "   \"sensor\" : \"0\" \n" +
                                         "}";

                        client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"접속 오류 { ex.Message}");
                    }
                    // 물체감지
                    App.Logger.Fatal(new Exception("컨베이어"), "물체감지 정지");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 애니메이션
        public void ConveyStartAnimation1()
        {
            // 애니메이션 모터회전(정회전)
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 360;
            da.Duration = TimeSpan.FromSeconds(2);
            da.RepeatBehavior = RepeatBehavior.Forever;

            RotateTransform rt = new RotateTransform();
            Motor1.RenderTransform = rt;
            Motor1.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            Motor2.RenderTransform = rt;
            Motor2.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            Motor3.RenderTransform = rt;
            Motor3.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            Motor4.RenderTransform = rt;
            Motor4.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
        public void ConveyStartAnimation2()
        {
            // 애니메이션 모터회전(역회전)
            DoubleAnimation da = new DoubleAnimation();
            da.From = 360;
            da.To = 0;
            da.Duration = TimeSpan.FromSeconds(2);
            da.RepeatBehavior = RepeatBehavior.Forever;

            RotateTransform rt = new RotateTransform();
            Motor1.RenderTransform = rt;
            Motor1.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            Motor2.RenderTransform = rt;
            Motor2.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            Motor3.RenderTransform = rt;
            Motor3.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            Motor4.RenderTransform = rt;
            Motor4.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
        private void StopAnimation()
        {
            // 로봇팔, 컨베이어 정지
            Motor1.RenderTransform = null;
            Motor2.RenderTransform = null;
            Motor3.RenderTransform = null;
            Motor4.RenderTransform = null;
            Arm.RenderTransform = null;
        }
        private void ArmStartAnimation()
        {
            // 애니메이션 로봇팔 움직임(양품)
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = -40;
            da.Duration = TimeSpan.FromSeconds(2);
            //da.RepeatBehavior = RepeatBehavior;
            da.AutoReverse = true;

            RotateTransform rt = new RotateTransform();
            Arm.RenderTransform = rt;
            Arm.RenderTransformOrigin = new System.Windows.Point(0, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
        private void ArmStartAnimation1()
        {
            // 애니메이션 로봇팔 움직임(불량)
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 40;
            da.Duration = TimeSpan.FromSeconds(2);
            //da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;

            RotateTransform rt = new RotateTransform();
            Arm.RenderTransform = rt;
            Arm.RenderTransformOrigin = new System.Windows.Point(0, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }

        private void ConveyRun_Click(object sender, RoutedEventArgs e)
        {
            ConveyStartAnimation1();
        }
        private void ConveyStop_Click(object sender, RoutedEventArgs e)
        {
            StopAnimation();
        }
        private void ConveyBack_Click(object sender, RoutedEventArgs e)
        {
            ConveyStartAnimation2();
        }
        private void ArmRun_Click(object sender, RoutedEventArgs e) //로봇팔 양품
        {
            ArmStartAnimation();
        }
        private void ArmRun2_Click(object sender, RoutedEventArgs e) //로봇팔 불량
        {
            ArmStartAnimation1();
        }
        private void ArmStop_Click(object sender, RoutedEventArgs e) // 로폿팔 정지
        {
            StopAnimation();
        }
        #endregion 

        #region CCTV 플레이 버튼
        //private void Play_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    image.SourceProvider.CreatePlayer(libDirectory);
        //    image.SourceProvider.MediaPlayer.Play(new Uri(RtspUrl));
        //}
        #endregion

        #region 온도값 화살표바인딩 생성자 + 함수
        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        private void ChangeValueOnClick(object sender, RoutedEventArgs e)
        {
            Value = new Random().Next(0, 100);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region OpenCV버튼 동영상 캡처 + 오픈 CV
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Mat src = Cv2.ImRead("../Image/test.jpg");
            Mat image = new Mat();
            Mat dst = src.Clone();

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            Cv2.CvtColor(src, image, ColorConversionCodes.BGR2GRAY);
            Cv2.Dilate(image, image, kernel, new OpenCvSharp.Point(-1, -1), 3);
            Cv2.GaussianBlur(image, image, new OpenCvSharp.Size(13, 13), 3, 3, BorderTypes.Reflect101);
            Cv2.Erode(image, image, kernel, new OpenCvSharp.Point(-1, -1), 3);

            // Cv2.HoughCircles(image, HoughModes.Gradient, 1, 30, 62(캐니엣지 변경 필요), 35, 0(최소반지름), 0(최대반지름));
            CircleSegment[] circles = Cv2.HoughCircles(image, HoughModes.Gradient, 1, 30, 61, 35, 35, 40);

            LblResult.Content = "구멍 갯수 :" + circles.Length;
            for (int i = 0; i < circles.Length; i++)
            {
                OpenCvSharp.Point center = new OpenCvSharp.Point(circles[i].Center.X, circles[i].Center.Y);

                Cv2.Circle(dst, center, (int)circles[i].Radius, Scalar.White, 3);
                Cv2.Circle(dst, center, 5, Scalar.AntiqueWhite, Cv2.FILLED);
            }

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();


        }

        private void AutoPlay()
        {
            Thread.Sleep(4000);
            FileInfo fi = new FileInfo("C:\\GitRepository\\MonitoringSystem_Project\\MonitoringSystem\\MonitoringSystem\\bin\\Image\\test.jpg");
            
            // 동영상 멈추면 다시 실행시키고 사진찍게함
            if (image.SourceProvider.MediaPlayer.IsPlaying())
            {
                image.SourceProvider.MediaPlayer.TakeSnapshot(fi);
            }
            else
            {
                //MessageBox.Show("Not play");
                image.SourceProvider.MediaPlayer.Play(uri);
                Thread.Sleep(2000);
                image.SourceProvider.MediaPlayer.TakeSnapshot(fi);
            }

            Mat src = Cv2.ImRead("../Image/test.jpg", ImreadModes.AnyColor);
            Mat src2 = Cv2.ImRead("../Image/test.jpg", ImreadModes.AnyColor);


            #region 구멍갯수
            Mat image2 = new Mat();
            Mat dst = src.Clone();

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            Cv2.CvtColor(src, image2, ColorConversionCodes.BGR2GRAY);
            Cv2.Dilate(image2, image2, kernel, new OpenCvSharp.Point(-1, -1), 3);
            Cv2.GaussianBlur(image2, image2, new OpenCvSharp.Size(13, 13), 3, 3, BorderTypes.Reflect101);
            Cv2.Erode(image2, image2, kernel, new OpenCvSharp.Point(-1, -1), 3);

            // Cv2.HoughCircles(image, HoughModes.Gradient, 1, 30, 62(캐니엣지 변경 필요), 35, 0(최소반지름), 0(최대반지름    ));
            CircleSegment[] circles = Cv2.HoughCircles(image2, HoughModes.Gradient, 1, 30, 59, 35, 35, 40);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                LblResult.Content = "구멍 갯수 :" + circles.Length;
            }));

            for (int i = 0; i < circles.Length; i++)
            {
                OpenCvSharp.Point center = new OpenCvSharp.Point(circles[i].Center.X, circles[i].Center.Y);

                Cv2.Circle(dst, center, (int)circles[i].Radius, Scalar.White, 3);
                Cv2.Circle(dst, center, 5, Scalar.AntiqueWhite, Cv2.FILLED);
            }
            Cv2.ImShow("dst", dst);
            #endregion


            Mat gray = new Mat();   // 흑백
            Mat gray2 = new Mat();   // 흑백

            Mat binary = new Mat(); //이진화
            Mat binary2 = new Mat(); //이진화

            Mat[] mv = new Mat[3];
            Mat mask = new Mat();  //빨강색
            Mat mask2 = new Mat();  //파랑색

            Cv2.CvtColor(src, src, ColorConversionCodes.BGR2HSV); //색공간을 그레이케일 영사을 변환하여 속도와 메로리 줄이기
            mv = Cv2.Split(src); //채널 분리(창하나더띠우기)
            Cv2.CvtColor(src, src, ColorConversionCodes.HSV2BGR);

            Cv2.InRange(mv[0], new Scalar(170), new Scalar(180), mask); //빨강색
            Cv2.InRange(mv[1], new Scalar(120), new Scalar(123), mask2); //파랑색
            Cv2.BitwiseAnd(src, mask.CvtColor(ColorConversionCodes.GRAY2BGR), src);
            Cv2.BitwiseAnd(src2, mask2.CvtColor(ColorConversionCodes.GRAY2BGR), src2);

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); // 흑백
            Cv2.CvtColor(src2, gray2, ColorConversionCodes.BGR2GRAY); // 흑백

            Cv2.Threshold(gray, binary, 127, 255, ThresholdTypes.Binary);  //이진화
            Cv2.Threshold(gray2, binary2, 127, 255, ThresholdTypes.Binary);  //이진화


            int pixels = Cv2.CountNonZero(binary);
            int pixels2 = Cv2.CountNonZero(binary2);

            #region 구멍갯수 판별
            if (circles.Length > 3)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    LblResult2.Content = "제품 판별 : 양품";
                }));

                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4001\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Arm\", \n" +
                                 "   \"value\" : \"1\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";
                client.Publish($"{factoryId}/4001/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            else
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => {
                    LblResult2.Content = "제품 판별 : 불량";
                }));
                var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pubData = "{ \n" +
                                 "   \"dev_addr\" : \"4001\", \n" +
                                 $"   \"currtime\" : \"{currtime}\" , \n" +
                                 "   \"code\" : \"Arm\", \n" +
                                 "   \"value\" : \"2\", \n" +
                                 "   \"sensor\" : \"0\" \n" +
                                 "}";
                client.Publish($"{factoryId}/4001/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            #endregion


            //#region 색상 판별
            //if (pixels > 50)   
            //{
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => {
            //        LblResult2.Content = "제품 판별 : 양품";
            //    }));

            //    Cv2.PutText(src, "red exist", new OpenCvSharp.Point(10, 400), HersheyFonts.HersheyComplex, 2, Scalar.White, 5, LineTypes.AntiAlias);
            //    var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    string pubData = "{ \n" +
            //                     "   \"dev_addr\" : \"4001\", \n" +
            //                     $"   \"currtime\" : \"{currtime}\" , \n" +
            //                     "   \"code\" : \"Arm\", \n" +
            //                     "   \"value\" : \"1\", \n" +
            //                     "   \"sensor\" : \"0\" \n" +
            //                     "}";


            //    client.Publish($"{factoryId}/4001/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

            //}
            //else if (pixels2 > 50)
            //{
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => {
            //        LblResult2.Content = "제품 판별 : 불량";
            //    }));
                
            //    Cv2.PutText(src2, "blue exist", new OpenCvSharp.Point(10, 400), HersheyFonts.HersheyComplex, 2, Scalar.White, 5, LineTypes.AntiAlias);
            //    var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    string pubData = "{ \n" +
            //                     "   \"dev_addr\" : \"4001\", \n" +
            //                     $"   \"currtime\" : \"{currtime}\" , \n" +
            //                     "   \"code\" : \"Arm\", \n" +
            //                     "   \"value\" : \"2\", \n" +
            //                     "   \"sensor\" : \"0\" \n" +
            //                     "}";


            //    client.Publish($"{factoryId}/4001/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

            //}
            //#endregion

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
            
        } //자동 opencv
        #endregion

        #region 공장 온도 차트그래프 버튼이벤트
        private void LoadLineTemp_Click(object sender, RoutedEventArgs e)
        {
            var win = new LineTempView();
            win.Topmost = true;
            win.ShowDialog();
        }
        #endregion
    }
}
