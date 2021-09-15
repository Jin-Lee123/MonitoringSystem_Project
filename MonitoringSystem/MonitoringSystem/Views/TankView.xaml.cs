using MonitoringSystem.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
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


        public TankView()
        {
            InitializeComponent();
        }
    }
}
