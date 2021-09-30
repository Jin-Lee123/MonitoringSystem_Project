using System;
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
    public partial class ConveyorView : UserControl
    {

        private string RtspUrl = "rtsp://192.168.0.14:9000";   //아이피(라즈베리아이피) 바꿔줘야댐 192.168.191.185  
        private DirectoryInfo libDirectory;

        public ConveyorView()
        {
            InitializeComponent();
            
            //CCTV
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
        }

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
            // 애니메이션 로봇팔 움직임
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
        private void ArmRun_Click(object sender, RoutedEventArgs e)
        {
            ArmStartAnimation();
        }
        private void ArmStop_Click(object sender, RoutedEventArgs e)
        {
            StopAnimation();
        }

        // CCTV
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            image.SourceProvider.CreatePlayer(libDirectory);
            image.SourceProvider.MediaPlayer.Play(new Uri(RtspUrl));
        }
    }
}
