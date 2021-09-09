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
    /// UC_Robotic_Arm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UC_Robotic_Arm : UserControl
    {
        public UC_Robotic_Arm()
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
            da.To = -40;


            da.Duration = TimeSpan.FromSeconds(2);
            da.RepeatBehavior = RepeatBehavior.Forever;

            da.AutoReverse = true;

            RotateTransform rt = new RotateTransform();
            Arm.RenderTransform = rt;
            Arm.RenderTransformOrigin = new Point(0, 0.5);
          
            rt.BeginAnimation(RotateTransform.AngleProperty, da);

          
        }
    }
}
