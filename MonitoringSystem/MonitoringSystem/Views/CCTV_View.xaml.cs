using System;
using System.Collections.Generic;
using System.Linq;
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
    /// CCTV_View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CCTV_View : Window
    {
        public CCTV_View()
        {
            InitializeComponent();
            Captured_Image.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + @"/../Image/CCTV.jpg", UriKind.RelativeOrAbsolute));
        }

        private void ZOOM_IN(object sender, RoutedEventArgs e)
        {
        }


        private void ZOOM_OUT(object sender, RoutedEventArgs e)
        {

        }

        private void ROTATE(object sender, RoutedEventArgs e)
        {
            myRotateTransform.Angle += 90;
        }
    }
}
