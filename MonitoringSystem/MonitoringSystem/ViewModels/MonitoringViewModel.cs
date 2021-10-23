using Caliburn.Micro;
using MonitoringSystem.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using DocumentFormat.OpenXml.Spreadsheet;
using MarkerType = OxyPlot.MarkerType;

namespace MonitoringSystem.ViewModels
{
    public class MonitoringViewModel : Conductor<object>
    {

        #region Property 설정

        private BindableCollection<TB_Plant> plant;

        public BindableCollection<TB_Plant> Plant
        {
            get => plant;
            set
            {
                plant = value;
                NotifyOfPropertyChange(() => Plant);

            }
        }
        #region Gas 변수
        private float gas1;
        private float gas2;
        private float gas3;
        private float gas4;
        private float gas5;
        private float gas6;

        public float Gas1
        {
            get => gas1;
            set
            {
                gas1 = value;
                NotifyOfPropertyChange(() => Gas1);
            }
        }
        public float Gas2
        {
            get => gas2;
            set
            {
                gas2 = value;
                NotifyOfPropertyChange(() => Gas2);
            }
        } 
        public float Gas3
        {
            get => gas3;
            set
            {
                gas3 = value;
                NotifyOfPropertyChange(() => Gas3);
            }
        }
        public float Gas4
        {
            get => gas4;
            set
            {
                gas4 = value;
                NotifyOfPropertyChange(() => Gas4);
            }
        }
        public float Gas5
        {
            get => gas5;
            set
            {
                gas5 = value;
                NotifyOfPropertyChange(() => Gas5);
            }
        }
        public float Gas6
        {
            get => gas6;
            set
            {
                gas6 = value;
                NotifyOfPropertyChange(() => Gas6);
            }
        }
        #endregion

        private float plantT;
        public float PlantT
        {
            get => plantT;
            set
            {
                plantT = value;
                NotifyOfPropertyChange(() => PlantT);
            }
        }

        private float plantH;
        public float PlantH
        {
            get => plantH;
            set
            {
                plantH = value;
                NotifyOfPropertyChange(() => PlantH);
            }
        }

        private PlotModel temperature;

        public PlotModel Temperature
        {
            get { return temperature; }
            set
            {
                temperature = value;
                NotifyOfPropertyChange(() => Temperature);
            }
        }

        private PlotModel otherproperty;

        public PlotModel Otherproperty
        {
            get { return otherproperty; }
            set
            {
                otherproperty = value;
                NotifyOfPropertyChange(() => Otherproperty);
            }
        }



        private float duty;
        public float Duty
        {
            get => duty;
            set
            {
                duty = value;
                NotifyOfPropertyChange(() => Duty);
            }
        }
        // RobotTemp
        private float robotTemp;
        public float RobotTemp
        {
            get => robotTemp;
            set
            {
                robotTemp = value;
                NotifyOfPropertyChange(() => RobotTemp);
            }
        }

        // ConveyTemp
        private float conveyTemp;
        public float ConveyTemp
        {
            get => conveyTemp;
            set
            {
                conveyTemp = value;
                NotifyOfPropertyChange(() => ConveyTemp);
            }
        }



        #region Setting값 Property

        #endregion
        #endregion


        #region 차트 설정
        public MonitoringViewModel()
        {
            //         App.LOGGER.Info($"예외발생, Client_MqttMsgPublishReceived : []");
            DispatcherTimer timer = new DispatcherTimer();


             var plotModel1 = new PlotModel();
             var plotModel2 = new PlotModel();


            plotModel1.Title = "Temperature";
            plotModel2.Title = "Gas";
            
            var linearAxis1 = new DateTimeAxis();
            linearAxis1.Position = AxisPosition.Bottom;
            plotModel1.Axes.Add(linearAxis1);

            var linearAxis2 = new LinearAxis();
         //   linearAxis2.Minimum = 15;
         //   linearAxis2.Maximum = 35;
            plotModel1.Axes.Add(linearAxis2);
            var lineSeries1 = new LineSeries();
            lineSeries1.Title = "LineSeries 1";
            
            
            var categoryAxis = new CategoryAxis();
            categoryAxis.Labels.Add("Gas1");
            categoryAxis.Labels.Add("Gas2");
            categoryAxis.Labels.Add("Gas3");
            categoryAxis.Labels.Add("Gas4");
            categoryAxis.Labels.Add("Gas5");
            categoryAxis.Labels.Add("Gas6");
            categoryAxis.Position = AxisPosition.Bottom;

            plotModel2.Axes.Add(categoryAxis);
            

            var linearAxis4 = new LinearAxis();
            plotModel2.Axes.Add(linearAxis4);
            linearAxis4.Maximum = 30;
            linearAxis4.Minimum = 20;
            var barSeries = new LinearBarSeries();
            barSeries.BarWidth = 40;
            
            plotModel1.Series.Add(lineSeries1);
            Temperature = plotModel1;
            Temperature.InvalidatePlot(true);

            plotModel2.Series.Add(barSeries);
            Otherproperty = plotModel2;
            Otherproperty.InvalidatePlot(true);

            timer.Tick += new EventHandler(FunctionB);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            Thread thread1 = new Thread(new ThreadStart(FunctionA));
            thread1.IsBackground = true;
            thread1.Start();


        }

    

        #endregion

        #region Plot 함수


        public void FunctionA()
        {
            DateTime curtime;
            var lineSeries1 = new LineSeries();
            var barSeries = new LinearBarSeries();
            

            lineSeries1.MarkerType = MarkerType.Circle;
            List<DataPoint> lData = new List<DataPoint>();
            List<DataPoint> TData = new List<DataPoint>();
            // GetPlant();




            while (true)
            {
                Thread.Sleep(100);
                Temperature.Series.Clear();
                lineSeries1.Points.Clear();
                Otherproperty.Series.Clear();
                barSeries.Points.Clear();
                curtime = DateTime.Now;
                double dCurtime = curtime.ToOADate();
                if (lData.Count == 0)
                {
                    
                 //   lData.Add(new DataPoint(dCurtime, Gas1));

                    lData.Add(new DataPoint(dCurtime, PlantT));
                    TData.Add(new DataPoint(dCurtime, PlantH));
                }
                else
                {

                    if (lData.Count <= 300)
                    {
                //        lData.Add(new DataPoint(dCurtime, Gas1));

                          lData.Add(new DataPoint(dCurtime, PlantT));
                        TData.Add(new DataPoint(dCurtime, PlantH));

                    }
                    else
                    {
                        lData.RemoveAt(0);
                        TData.RemoveAt(0);
                 //     lData.Add(new DataPoint(dCurtime, Gas1));
                        lData.Add(new DataPoint(dCurtime, PlantT));
                        TData.Add(new DataPoint(dCurtime, PlantH));

                    }
                }

                foreach (var data in lData)
                {
                    lineSeries1.Points.Add(data);
                }
                barSeries.Points.Add(new DataPoint(Gas1, y : Gas1));
                barSeries.Points.Add(new DataPoint(Gas2, y : Gas2));
                barSeries.Points.Add(new DataPoint(Gas3, y : Gas3));
                barSeries.Points.Add(new DataPoint(Gas4, y : Gas4));
                barSeries.Points.Add(new DataPoint(Gas5, y : Gas5));
                barSeries.Points.Add(new DataPoint(Gas6, y : Gas6));
                Otherproperty.Series.Add(barSeries);
                Otherproperty.InvalidatePlot(true);
                Temperature.Series.Add(lineSeries1);
                Temperature.InvalidatePlot(true);

            }
        }
        #endregion

        private void FunctionB(object sender, EventArgs e)
        {
            PlantT = DataConnection.PlantT;
            ConveyTemp = DataConnection.ConveyTemp;
            RobotTemp = DataConnection.RobotTemp;
            Duty = DataConnection.Duty;
            Gas1 = DataConnection.Gas1;
            Gas2 = DataConnection.Gas2;
            Gas3 = DataConnection.Gas3;
            Gas4 = DataConnection.Gas4;
            Gas5 = DataConnection.Gas5;
            Gas6 = DataConnection.Gas6;
        }

    }
}

