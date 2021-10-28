using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.Views
{
    public partial class ConveyorView : UserControl, INotifyPropertyChanged
    {
        //mqtt
        private string serverIpNum = "192.168.0.195";   //학원 "192.168.0.195"; //핸드폰 "192.168.21.186";  // 윈도우(MQTT Broker, SQLServer) 아이피
        private string clientId = "SCADA_system";
        private string factoryId = "kasan01";
        private MqttClient client;
        

        //opencv
        private const string windowName = "src";

        //아이피(라즈베리아이피) 바꿔줘야댐 192.168.191.185   192.168.0.14
        private string RtspUrl = "rtsp://192.168.0.5:9000";
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
            //client.Subscribe(new string[] { $"{factoryId}/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

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

                // 거리가 100보다 작으면 컨베이어 밸트 멈추고 영상분석 후 8초후 다시 작동로직
                if (currData["dev_addr"] == "4003" && currData["code"] == "Conveydist" && int.Parse(currData["sensor"]) <= 100)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        // Publish 컨베이어 Stop
                        var currtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string pubData = "{ \n" +
                                         "   \"dev_addr\" : \"4002\", \n" +
                                         $"   \"currtime\" : \"{currtime}\" , \n" +
                                         "   \"code\" : \"Convey\", \n" +
                                         "   \"value\" : \"3\", \n" +
                                         "   \"sensor\" : \"0\" \n" +
                                         "}";
                        client.Publish($"{factoryId}/4002/", Encoding.UTF8.GetBytes(pubData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

                        // 오픈 CV 영상 분석후 로봇팔 움직이는 로직
                        AutoPlay();

                        // 8초동안 컨베이어 멈춤!
                        Thread.Sleep(8000);

                        // Publish 컨베이어 Run
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
        private void AutoRun_Click(object sender, RoutedEventArgs e)  //오토 컨베이어 작동
        {
            ConveyStartAnimation1();
        }
        private void AutoStop_Click(object sender, RoutedEventArgs e)  // 오토 컨베이어 멈춤
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
            Mat src = Cv2.ImRead("../Image/test.jpg"); // test 초록색, test2 빨간색
            Mat image = new Mat();
            Mat dst = src.Clone();

            Mat hsv = new Mat();
            Mat src2 = new Mat();
            Mat src3 = new Mat();
            Mat src4 = new Mat();
            Cv2.CopyTo(src, src2);
            Cv2.CopyTo(src, src3);
            Cv2.CopyTo(src, src4);

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);

            Cv2.InRange(hsv, new Scalar(0, 100, 100), new Scalar(10, 255, 255), src2);
            Cv2.InRange(hsv, new Scalar(170, 100, 100), new Scalar(179, 255, 255), src3);
            Cv2.InRange(hsv, new Scalar(63, 100, 100), new Scalar(75, 255, 255), src4);

            Mat redChannel = src2 | src3 | src4;

            Cv2.CvtColor(redChannel, src2, ColorConversionCodes.GRAY2BGR);
            Cv2.CvtColor(src2, image, ColorConversionCodes.BGR2GRAY); // 회색 

            Cv2.ImShow("image", image);

            Cv2.Dilate(image, image, kernel, new OpenCvSharp.Point(-1, -1), 3); // 이미지 팽창
            Cv2.GaussianBlur(image, image, new OpenCvSharp.Size(13, 13), 3, 3, BorderTypes.Reflect101); // 흐르게
            Cv2.Erode(image, image, kernel, new OpenCvSharp.Point(-1, -1), 3); // 이미지 축소

            // Cv2.HoughCircles(image, HoughModes.Gradient, 1, 30, 62(캐니엣지 변경 필요), 35, 0(최소반지름), 0(최대반지름));
            // 실제 원 구하는 
            CircleSegment[] circles = Cv2.HoughCircles(image, HoughModes.Gradient, 1, 30, 63, 35, 35, 40); 

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

            //#region 색상 분석!
            //Mat gray = new Mat();   // 흑백
            //Mat gray2 = new Mat();   // 흑백
            //Mat gray3 = new Mat();   // 흑백

            //Mat binary = new Mat(); //이진화
            //Mat binary2 = new Mat(); //이진화
            //Mat binary3 = new Mat(); //이진화

            //Mat[] mv = new Mat[4];
            //Mat mask = new Mat();  //빨강색
            //Mat mask0 = new Mat();  //빨강색
            //Mat mask2 = new Mat();  //초록색
            //Mat mask3 = new Mat();  //노랑색

            //Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV); //색공간을 그레이케일 영사을 변환하여 속도와 메로리 줄이기
            //Cv2.CvtColor(src, src, ColorConversionCodes.BGR2HSV); //색공간을 그레이케일 영사을 변환하여 속도와 메로리 줄이기
            //mv = Cv2.Split(src); //채널 분리(창하나더띠우기)
            //Cv2.CvtColor(src, src, ColorConversionCodes.HSV2BGR);

            ////Cv2.ImShow("HSV", hsv);
            ///*Cv2.ImShow("Channel1", mv[0]);
            //Cv2.ImShow("Channel2", mv[1]);
            //Cv2.ImShow("Channel3", mv[2]);*/


            //// mv[0] Red channel, mv[1] Green channel, mv[2] Blue channel
            //// mv[0] + mv[1] = yellow

            //Mat yellowchannel = mv[0] | mv[1];
            //Cv2.InRange(mv[0], new Scalar(0, 0, 0), new Scalar(5, 255, 255), mask0); //빨강색
            //Cv2.InRange(mv[0], new Scalar(178, 0, 0), new Scalar(180, 255, 255), mask); //빨강색
            //Cv2.InRange(hsv, new Scalar(41, 0, 0), new Scalar(70, 255, 255), mask2); //초록색 70 ~75
            //Cv2.InRange(hsv, new Scalar(26,0,0), new Scalar(30,255,255), mask3); //노랑색

            //Mat maskRed = mask0 | mask;

            //Cv2.ImShow("mask1", maskRed);
            //Cv2.ImShow("mask2", mask2);
            //Cv2.ImShow("mask3", mask3);

            //Cv2.BitwiseAnd(src, mask.CvtColor(ColorConversionCodes.GRAY2BGR), src);
            //Cv2.BitwiseAnd(src2, mask2.CvtColor(ColorConversionCodes.GRAY2BGR), src2);
            //Cv2.BitwiseAnd(src3, mask3.CvtColor(ColorConversionCodes.GRAY2BGR), src3);

            //Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); // 흑백
            //Cv2.CvtColor(src2, gray2, ColorConversionCodes.BGR2GRAY); // 흑백
            //Cv2.CvtColor(src3, gray3, ColorConversionCodes.BGR2GRAY); // 흑백

            //Cv2.Threshold(gray, binary, 127, 255, ThresholdTypes.Binary);  //이진화
            //Cv2.Threshold(gray2, binary2, 127, 255, ThresholdTypes.Binary);  //이진화
            //Cv2.Threshold(gray3, binary3, 127, 255, ThresholdTypes.Binary);  //이진화

            //int pixels = Cv2.CountNonZero(binary);
            //int pixels2 = Cv2.CountNonZero(binary2);
            //int pixels3 = Cv2.CountNonZero(binary3);

            //#endregion
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
            Mat hsv = new Mat();
            Mat src = Cv2.ImRead("../Image/test.jpg", ImreadModes.AnyColor);
            Mat src2 = new Mat();
            Mat src3 = new Mat();
            Mat src4 = new Mat();
            Mat src5 = new Mat();


            Cv2.CopyTo(src, src2);
            Cv2.CopyTo(src, src3);
            Cv2.CopyTo(src, src4);
            Cv2.CopyTo(src, src5);


            #region 구멍갯수 분석!
            Mat image2 = new Mat();
            Mat dst = src.Clone();

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);

            Cv2.InRange(hsv, new Scalar(0, 100, 100), new Scalar(10, 255, 255), src2);
            Cv2.InRange(hsv, new Scalar(170, 100, 100), new Scalar(179, 255, 255), src3);
            Cv2.InRange(hsv, new Scalar(55, 100, 100), new Scalar(70, 255, 255), src4);

            Mat redChannel = src2 | src3 | src4;

            Cv2.CvtColor(redChannel, src2, ColorConversionCodes.GRAY2BGR);
            Cv2.CvtColor(src2, image2, ColorConversionCodes.BGR2GRAY); // 회색 

            Cv2.ImShow("image2", image2);


            Cv2.Dilate(image2, image2, kernel, new OpenCvSharp.Point(-1, -1), 3);
            Cv2.GaussianBlur(image2, image2, new OpenCvSharp.Size(13, 13), 3, 3, BorderTypes.Reflect101);
            Cv2.Erode(image2, image2, kernel, new OpenCvSharp.Point(-1, -1), 3);

            // Cv2.HoughCircles(image, HoughModes.Gradient, 1, 30, 62(캐니엣지 변경 필요), 35, 0(최소반지름), 0(최대반지름    ));
            CircleSegment[] circles = Cv2.HoughCircles(image2, HoughModes.Gradient, 1, 30, 63, 35, 35, 40);

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
            Cv2.ImShow("구멍갯수", dst);
            #endregion

            #region 색상 분석!
            Mat gray = new Mat();   // 흑백
            Mat gray2 = new Mat();   // 흑백
           // Mat gray3 = new Mat();   // 흑백

            Mat binary = new Mat(); //이진화
            Mat binary2 = new Mat(); //이진화
            //Mat binary3 = new Mat(); //이진화

            Mat[] mv = new Mat[3];
            Mat mask = new Mat();  //빨강색
            //Mat mask0 = new Mat();  //빨강색
            Mat mask2 = new Mat();  //초록색
            //Mat mask3 = new Mat();  //노랑색

            //Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV); //색공간을 그레이케일 영사을 변환하여 속도와 메로리 줄이기
            Cv2.CvtColor(src, src, ColorConversionCodes.BGR2HSV); //색공간을 그레이케일 영사을 변환하여 속도와 메로리 줄이기
            mv = Cv2.Split(src); //채널 분리(창하나더띠우기)
            Cv2.CvtColor(src, src, ColorConversionCodes.HSV2BGR);

            //Cv2.InRange(mv[0], new Scalar(178), new Scalar(179), mask); //빨강색
            // Cv2.InRange(mv[1], new Scalar(73), new Scalar(74), mask2); //초록색 70 ~75
            //Cv2.InRange(mv[2], new Scalar(25), new Scalar(35), mask3); //노랑색

            //Cv2.InRange(hsv, new Scalar(0, 0, 0), new Scalar(1, 255, 255), mask0); //빨강색
            Cv2.InRange(mv[0], new Scalar(175), new Scalar(180), mask); //빨강색
            //Cv2.InRange(hsv, new Scalar(179, 0, 0), new Scalar(180, 255, 255), mask); //빨강색
            Cv2.InRange(mv[1], new Scalar(65), new Scalar(70), mask2); //초록색 70 ~75
            //Cv2.InRange(hsv, new Scalar(66, 0, 0), new Scalar(67, 255, 255), mask2); //초록색 70 ~75
            //Cv2.InRange(hsv, new Scalar(29, 0, 0), new Scalar(30, 255, 255), mask3); //노랑색

            //Mat maskRed = mask0 | mask;

            Cv2.BitwiseAnd(src, mask.CvtColor(ColorConversionCodes.GRAY2BGR), src);
            Cv2.BitwiseAnd(src5, mask2.CvtColor(ColorConversionCodes.GRAY2BGR), src5);
            //Cv2.BitwiseAnd(src3, mask3.CvtColor(ColorConversionCodes.GRAY2BGR), src3);

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); // 흑백
            Cv2.CvtColor(src5, gray2, ColorConversionCodes.BGR2GRAY); // 흑백
            //Cv2.CvtColor(src3, gray3, ColorConversionCodes.BGR2GRAY); // 흑백

            Cv2.Threshold(gray, binary, 127, 255, ThresholdTypes.Binary);  //이진화
            Cv2.Threshold(gray2, binary2, 127, 255, ThresholdTypes.Binary);  //이진화
            //Cv2.Threshold(gray3, binary3, 127, 255, ThresholdTypes.Binary);  //이진화

            int pixels = Cv2.CountNonZero(binary);
            int pixels2 = Cv2.CountNonZero(binary2);
            // pixels3 = Cv2.CountNonZero(binary3);
            #endregion

            //#region 구멍갯수 식별 후 로봇팔 로직!
            //if (circles.Length > 3)
            //{
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            //    {
            //        LblResult2.Content = "제품 판별 : 양품";
            //    }));

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
            //else
            //{
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => {
            //        LblResult2.Content = "제품 판별 : 불량";
            //    }));
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

            #region 색상 식별 후 로봇팔 로직!
            if (pixels > 50)
            {
                ArmStartAnimation();
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    LblResult2.Content = "제품 판별 : 불량(빨간색)";
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
            else if (pixels2 > 50)
            {
                ArmStartAnimation1();
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    LblResult2.Content = "제품 판별 : 양품(초록색)";
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
            //else if (pixels3 > 10000)
            //{
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            //    {
            //        LblResult2.Content = "제품 판별 : 불량(노랑색)" +
            //        ")";
            //    }));
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
            #endregion
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
        #endregion

        #region 컨베이어,로봇팔 온도 차트 열기 버튼
        private void LoadLineTemp_Click(object sender, RoutedEventArgs e)
        {
            var win = new LineTempView();
            win.Topmost = true;
            win.ShowDialog();
        }
        #endregion
    }
}
