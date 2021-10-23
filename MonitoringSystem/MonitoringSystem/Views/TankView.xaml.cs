using MonitoringSystem.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// TankView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TankView : UserControl
    {
        private string RtspUrl = "rtsp://192.168.0.16:9000";   //아이피(라즈베리아이피) 바꿔줘야댐 192.168.191.185  
        private DirectoryInfo libDirectory;
        Uri uri;

        public TankView()
        {
            InitializeComponent();
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
        }
        // CCTV
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            image.SourceProvider.CreatePlayer(libDirectory);
            uri = new Uri(RtspUrl);
            image.SourceProvider.MediaPlayer.Play(uri);
            FileInfo file = new FileInfo("C:\\GitRepository\\MonitoringSystem_Project\\MonitoringSystem\\MonitoringSystem\\test.jpg");

            if (image.SourceProvider.MediaPlayer.IsPlaying())
            {
                image.SourceProvider.MediaPlayer.TakeSnapshot(file);
            }
            else
            {
                image.SourceProvider.MediaPlayer.Play(uri);
                Thread.Sleep(2000);
                image.SourceProvider.MediaPlayer.TakeSnapshot(file);
            }
        }

    }
}
