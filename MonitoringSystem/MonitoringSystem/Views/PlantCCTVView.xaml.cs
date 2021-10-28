using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// PlantCCTVView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlantCCTVView : Window
    {
        private string RtspUrl = "rtsp://192.168.0.16:9000";
        private DirectoryInfo libDirectory;
        Uri uri;

        public PlantCCTVView()
        {
            InitializeComponent();
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            image.SourceProvider.CreatePlayer(libDirectory);
            uri = new Uri(RtspUrl);
            image.SourceProvider.MediaPlayer.Play(uri);

        }
    }
}
