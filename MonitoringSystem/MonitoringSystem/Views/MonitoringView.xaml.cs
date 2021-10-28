using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// MonitoringView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitoringView : UserControl
    {
        Thread thread;
        ThreadStart ts;
        Dispatcher dispatcher = Application.Current.Dispatcher;

        private string RtspUrl = "rtsp://192.168.0.10:9000";
        private DirectoryInfo libDirectory;

        private bool activeThread;
        public MonitoringView()
        {
            InitializeComponent();

            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
           // image.SourceProvider.CreatePlayer(libDirectory);
           // image.SourceProvider.MediaPlayer.Play(new Uri(RtspUrl));
            MessageBox.Show("Warn Log 입력");
            App.Logger.Fatal(new Exception("MyWarning"), "Warning!!!!");

        }

    }
}
