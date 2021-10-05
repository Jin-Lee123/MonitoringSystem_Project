using Caliburn.Micro;
using MonitoringSystem.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Windows.Threading;

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
        #region Setting값 Property

        #endregion
        #endregion

        
        #region 차트 설정
        public MonitoringViewModel()
        {
            //         App.LOGGER.Info($"예외발생, Client_MqttMsgPublishReceived : []");
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(FunctionB);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            var plotModel1 = new PlotModel();
            plotModel1.Title = "Temperature";
            var linearAxis1 = new DateTimeAxis();
            linearAxis1.Position = AxisPosition.Bottom;
            plotModel1.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis();
            plotModel1.Axes.Add(linearAxis2);
            var lineSeries1 = new LineSeries();
            lineSeries1.Title = "LineSeries 1";

            plotModel1.Series.Add(lineSeries1);
            Temperature = plotModel1;
            Temperature.InvalidatePlot(true);

            var plotModel2 = new PlotModel();
            plotModel2.Title = "OtherProperty";
            var linearAxis3 = new LinearAxis();
            linearAxis3.Position = AxisPosition.Bottom;
            plotModel2.Axes.Add(linearAxis3);
            var linearAxis4 = new LinearAxis();
            plotModel2.Axes.Add(linearAxis4);
            var lineSeries2 = new LinearBarSeries();
            lineSeries2.Title = "BarSeries 2";
            lineSeries2.Points.Add(new DataPoint(0, 28));
            lineSeries2.Points.Add(new DataPoint(1, 32));
            plotModel2.Series.Add(lineSeries2);
            Otherproperty = plotModel2;
            Otherproperty.InvalidatePlot(true);
            Thread thread1 = new Thread(new ThreadStart(FunctionA));
            thread1.IsBackground = true;
            thread1.Start();

           
        }

        private void FunctionB(object sender, EventArgs e)
        {
            PlantT = DataConnection.PlantT;
        }


        #endregion

        #region Plot 함수


        public void FunctionA()
        {
            DateTime curtime;
            var lineSeries1 = new LineSeries();
            lineSeries1.MarkerType = MarkerType.Circle;
            List<DataPoint> lData = new List<DataPoint>();
           // GetPlant();

            while (true)
            {
                Thread.Sleep(100);
                Temperature.Series.Clear();
                lineSeries1.Points.Clear();
                curtime = DateTime.Now;
                double dCurtime = curtime.ToOADate();
                if (lData.Count == 0)
                {
                    
                    lData.Add(new DataPoint(dCurtime, PlantT));
                //    lData.Add(new DataPoint(dCurtime, PlantT));
                }
                else
                {

                    if (lData.Count <= 300)
                    {
                        lData.Add(new DataPoint(dCurtime, PlantT));
                 //       lData.Add(new DataPoint(dCurtime, PlantT));
                    }
                    else
                    {
                        lData.RemoveAt(0);
                        lData.Add(new DataPoint(dCurtime, PlantT));
                 //       lData.Add(new DataPoint(dCurtime, PlantT));
                    }
                }

                foreach (var data in lData)
                {
                    lineSeries1.Points.Add(data);
                }

                Temperature.Series.Add(lineSeries1);
                Temperature.InvalidatePlot(true);

            }
        }
        #endregion

        
        #region sql 조회 함수



        /*  public void GetPlant()
          {
              using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
              {
                  conn.Open();
                  SqlCommand cmd = new SqlCommand(Models.TB_Plant.SELECT_QUERY, conn);
                  SqlDataReader reader = cmd.ExecuteReader();
                  plant = new BindableCollection<TB_Plant>();

                  while (reader.Read())
                  {
                      var empTmp = new TB_Plant
                      {
                          PlantT = float.Parse(reader["PlantT"].ToString()),
                      };
                      plant.Add(empTmp);
                  }
              }
        }*/

        #endregion

    }
}

