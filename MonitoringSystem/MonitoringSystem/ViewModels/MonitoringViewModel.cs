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
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;

namespace MonitoringSystem.ViewModels
{
    public class MonitoringViewModel : Conductor<object>
    {

        #region Property 설정

        private SeriesCollection seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set
            {
                seriesCollection = value;
            }
        }
/*
        private double values;
        public double Values
        {
            get => values;
            set
            {
                values = value;
                NotifyOfPropertyChange(() => Values);
            }
        }
*/

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

        private BindableCollection<TB_Log> TB_log;

        public BindableCollection<TB_Log> Log
        {
            get => TB_log;
            set
            {
                TB_log = value;
                NotifyOfPropertyChange(() => Log);
            }
        }

        private string fan;
        public string Fan
        {
            get => fan;
            set
            {
                fan = value;
                NotifyOfPropertyChange(() => Fan);
            }
        }

        #region Gas 변수
        private double gas1;
        private double gas2;
        private double gas3;
        private double gas4;
        private double gas5;
        private double gas6;

        public double Gas1
        {
            get => gas1;
            set
            {
                gas1 = value;
                NotifyOfPropertyChange(() => Gas1);
            }
        }
        public double Gas2
        {
            get => gas2;
            set
            {
                gas2 = value;
                NotifyOfPropertyChange(() => Gas2);
            }
        } 
        public double Gas3
        {
            get => gas3;
            set
            {
                gas3 = value;
                NotifyOfPropertyChange(() => Gas3);
            }
        }
        public double Gas4
        {
            get => gas4;
            set
            {
                gas4 = value;
                NotifyOfPropertyChange(() => Gas4);
            }
        }
        public double Gas5
        {
            get => gas5;
            set
            {
                gas5 = value;
                NotifyOfPropertyChange(() => Gas5);
            }
        }
        public double Gas6
        {
            get => gas6;
            set
            {
                gas6 = value;
                NotifyOfPropertyChange(() => Gas6);
            }
        }
        #endregion

        private double plantT;
        public double PlantT
        {
            get => plantT;
            set
            {
                plantT = value;
                NotifyOfPropertyChange(() => PlantT);
            }
        }

        private double plantH;
        public double PlantH
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



        private double duty;
        public double Duty
        {
            get => duty;
            set
            {
                duty = value;
                NotifyOfPropertyChange(() => Duty);
            }
        }
        // RobotTemp
        private double robotTemp;
        public double RobotTemp
        {
            get => robotTemp;
            set
            {
                robotTemp = value;
                NotifyOfPropertyChange(() => RobotTemp);
            }
        }

        // ConveyTemp
        private double conveyTemp;

        public double ConveyTemp
        {
            get => conveyTemp;
            set
            {
                conveyTemp = value;
                NotifyOfPropertyChange(() => ConveyTemp);
            }
        }
        private double mainTankValue;
        public double MainTankValue
        {
            get => mainTankValue;
            set
            {
                if (value >= 1000)
                {
                    mainTankValue = 100;
                }
                else if (1000 > value && value >= 600)
                {
                    mainTankValue = Math.Round(value / 1000 * 100, 2);
                }
                else
                {
                    mainTankValue = Math.Round(value / 1200 * 100, 2);
                }
            }
        }

        // SubTankValue 계산
        private double subTankValue;
        public double SubTankValue
        {
            get => subTankValue;
            set
            {
                if (value >= 1000)
                {
                    subTankValue = 100;
                }
                else if (1000 > value && value >= 600)
                {
                    subTankValue = Math.Round(value / 1000 * 100, 2);
                }
                else if (value < 300)
                {
                    subTankValue = 50;
                }
             }
        }

        public string[] GasName { get; set; }
        public Func<double, string> Formatter { get; set; }

        #region ### Fan On/Off ###
        public void FanOn()
        {
            DataConnection.FanOn();
        }
        public void FanOff()
        {
            DataConnection.FanOff();
        }
        #endregion
        #endregion


        #region 차트 설정
        public MonitoringViewModel()
        {
            

            SeriesCollection = new SeriesCollection
            {
                new LiveCharts.Wpf.ColumnSeries
                {
                 Values = new ChartValues<double> {1, 2, 3, 4, 5, 6}
                }
            };

            GasName = new[] { "CO", "Alcohol", "CO2", "Tolueno", "NH4", "Acetona" };
            Formatter = value => value.ToString("N");

            

            DispatcherTimer timer = new DispatcherTimer();

            timer.Tick += new EventHandler(FunctionB);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SeriesCollection[0].Values.Clear();
                        SeriesCollection[0].Values.Add(Gas1);
                        SeriesCollection[0].Values.Add(Gas2);
                        SeriesCollection[0].Values.Add(Gas3);
                        SeriesCollection[0].Values.Add(Gas4);
                        SeriesCollection[0].Values.Add(Gas5);
                        SeriesCollection[0].Values.Add(Gas6);
                    });
                }
            });
        }


        #endregion

        
        private void FunctionB(object sender, EventArgs e)
        {
            PlantT = DataConnection.PlantT;
            PlantH = DataConnection.PlantH;
            Duty = DataConnection.Duty;
            ConveyTemp = DataConnection.ConveyTemp;
            RobotTemp = DataConnection.RobotTemp;
            Duty = DataConnection.Duty;
            Gas1 = DataConnection.Gas1;
            Gas2 = DataConnection.Gas2;
            Gas3 = DataConnection.Gas3;
            Gas4 = DataConnection.Gas4;
            Gas5 = DataConnection.Gas5;
            Gas6 = DataConnection.Gas6;
            Fan = DataConnection.Fan;
            MainTankValue = DataConnection.MainTankValue;
            SubTankValue = DataConnection.SubTankValue;
            GetLogs();


            
        }
        public void GetLogs()
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
               
                conn.Open();


                SqlCommand cmd = new SqlCommand(Models.TB_Log.SELECT_QUERY2, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                Log = new BindableCollection<TB_Log>();

                while (reader.Read())
                {

                    var empTmp = new TB_Log
                    {

                        Level = reader["Level"].ToString(),
                        Message = reader["Message"].ToString(),
                        AdditionalInfo = reader["AdditionalInfo"].ToString(),

                    };
                    Log.Add(empTmp);
                }
            }
        }
    }

}



