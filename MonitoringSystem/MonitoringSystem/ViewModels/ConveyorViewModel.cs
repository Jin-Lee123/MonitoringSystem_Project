using Caliburn.Micro;
using MonitoringSystem.JIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MonitoringSystem.ViewModels
{
    class ConveyorViewModel : Conductor<object>
    {
        #region 애니매이션
        //private UC_Motor motor1;
        //public UC_Motor Motor1
        //{
        //    get => motor1;
        //    set
        //    {
        //        motor1 = value;
        //        NotifyOfPropertyChange(() => Motor1);
        //    }
        //}

        //public ConveyorViewModel()
        //{
        //    Motor1 = new UC_Motor();
        //}

        //public void RunStop()
        //{
        //    if (IsRun == false)
        //    {
        //        StartAnimation();
        //        StartAnimation1();
        //        IsRun = true;
        //    }
        //    else
        //    {
        //        StopAnimation();
        //        IsRun = false;
        //    }
        //}


        //private bool IsRun = false;

        //private void StopAnimation()
        //{
        //    Motor1.RenderTransform = null;
        //    Motor2.RenderTransform = null;
        //    Motor3.RenderTransform = null;
        //    Motor4.RenderTransform = null;

        //    Arm.RenderTransform = null;
        //}
        //public void StartAnimation()
        //{
        //    // 애니메이션 gg
        //    DoubleAnimation da = new DoubleAnimation();
        //    da.From = 0;
        //    da.To = 360;
        //    da.Duration = TimeSpan.FromSeconds(2);
        //    da.RepeatBehavior = RepeatBehavior.Forever;

        //    RotateTransform rt = new RotateTransform();
        //    Motor1.RenderTransform = rt;
        //    Motor1.RenderTransformOrigin = new Point(0.5, 0.5);
        //    Motor2.RenderTransform = rt;
        //    Motor2.RenderTransformOrigin = new Point(0.5, 0.5);
        //    Motor3.RenderTransform = rt;
        //    Motor3.RenderTransformOrigin = new Point(0.5, 0.5);
        //    Motor4.RenderTransform = rt;
        //    Motor4.RenderTransformOrigin = new Point(0.5, 0.5);

        //    rt.BeginAnimation(RotateTransform.AngleProperty, da);
        //}
        //private void StartAnimation1()
        //{
        //    // 애니메이션 gg
        //    DoubleAnimation da = new DoubleAnimation();
        //    da.From = 0;
        //    da.To = -40;
        //    da.Duration = TimeSpan.FromSeconds(2);
        //    da.RepeatBehavior = RepeatBehavior.Forever;
        //    da.AutoReverse = true;

        //    RotateTransform rt = new RotateTransform();
        //    Arm.RenderTransform = rt;
        //    Arm.RenderTransformOrigin = new Point(0, 0.5);

        //    rt.BeginAnimation(RotateTransform.AngleProperty, da);
        //}
        #endregion  // 주석처리중


    }
}
