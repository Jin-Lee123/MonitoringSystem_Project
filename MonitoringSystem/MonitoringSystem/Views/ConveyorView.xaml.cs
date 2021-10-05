using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// ConveyorView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConveyorView : UserControl, INotifyPropertyChanged
    {

        private string RtspUrl = "rtsp://192.168.0.6:9000";   //아이피(라즈베리아이피) 바꿔줘야댐 192.168.191.185   192.168.0.14
        private DirectoryInfo libDirectory;

        public ConveyorView()
        {
            InitializeComponent();
            
            //CCTV
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            //화살표
            Value = 160;
            DataContext = this;
        }

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
            Motor1.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor2.RenderTransform = rt;
            Motor2.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor3.RenderTransform = rt;
            Motor3.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor4.RenderTransform = rt;
            Motor4.RenderTransformOrigin = new Point(0.5, 0.5);

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
            Motor1.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor2.RenderTransform = rt;
            Motor2.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor3.RenderTransform = rt;
            Motor3.RenderTransformOrigin = new Point(0.5, 0.5);
            Motor4.RenderTransform = rt;
            Motor4.RenderTransformOrigin = new Point(0.5, 0.5);

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
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;

            RotateTransform rt = new RotateTransform();
            Arm.RenderTransform = rt;
            Arm.RenderTransformOrigin = new Point(0, 0.5);

            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
        private void ArmStartAnimation1()
        {
            // 애니메이션 로봇팔 움직임(불량)
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 40;
            da.Duration = TimeSpan.FromSeconds(2);
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;

            RotateTransform rt = new RotateTransform();
            Arm.RenderTransform = rt;
            Arm.RenderTransformOrigin = new Point(0, 0.5);

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

        #region CCTV
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            image.SourceProvider.CreatePlayer(libDirectory);
            image.SourceProvider.MediaPlayer.Play(new Uri(RtspUrl));
        }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileInfo fi = new FileInfo("C:\\asdf\\test.jpg");
            image.SourceProvider.MediaPlayer.TakeSnapshot(fi);
        }
    }
}
