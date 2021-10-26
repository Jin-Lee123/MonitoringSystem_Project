using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Windows;

using OpenCvSharp;
using System.Windows.Media.Imaging;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// TankView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TankView : UserControl
    {
        private string RtspUrl = "rtsp://192.168.0.7:9000";   //아이피(라즈베리아이피) 바꿔줘야댐 192.168.191.185  
        private DirectoryInfo libDirectory;
        private const string windowName = "src";
        Uri uri;

        public TankView()
        {
            InitializeComponent();
            
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            // 동영상 실행
            CCTV.SourceProvider.CreatePlayer(libDirectory);
            CCTV.SourceProvider.MediaPlayer.Play(new Uri(RtspUrl));
        }
        #region CCTV Play Button

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            AutoPlay();
        }

        #endregion

        private void AutoPlay()
        {
            Thread.Sleep(4000);
            FileInfo fi = new FileInfo("C:\\GitRepository\\MonitoringSystem_Project\\MonitoringSystem\\MonitoringSystem\\bin\\Image\\CCTV.jpg");

            // 동영상 멈추면 다시 실행시키고 사진찍게함
            if (CCTV.SourceProvider.MediaPlayer.IsPlaying())
            {
                CCTV.SourceProvider.MediaPlayer.TakeSnapshot(fi);
            }
            else
            {
                //MessageBox.Show("Not play");
                CCTV.SourceProvider.MediaPlayer.Play(uri);
                Thread.Sleep(2000);
                CCTV.SourceProvider.MediaPlayer.TakeSnapshot(fi);
            }
            Mat src = Cv2.ImRead("../Image/CCTV.jpg", ImreadModes.AnyColor);
        }

        #region 사진 캡쳐 로직 구현
        private void Capture_Button_Click(object sender, RoutedEventArgs e)
        {
            // Create Image and set its width and height  
            Image dynamicImage = new Image();
            dynamicImage.Width = 300;
            dynamicImage.Height = 200;

            // Create a BitmapSource  
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(System.Environment.CurrentDirectory + @"/../Image/CCTV.jpg", UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            // Set Image.Source  
            dynamicImage.Source = bitmap;

            // Add Image to Window  
            Captured_Image.Children.Add(dynamicImage);
        }
        #endregion
    }
}
