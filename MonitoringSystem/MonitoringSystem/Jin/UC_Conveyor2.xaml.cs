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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MonitoringSystem.JIN
{
    /// <summary>
    /// UC_Conveyor2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UC_Conveyor2 : UserControl
    {
        public UC_Conveyor2()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartAnimation();
        }

        private void StartAnimation()
        {
            // 애니메이션 gg
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
    }
}
