using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using LiveCharts;
using LiveCharts.Wpf;
using MonitoringSystem.Models;

namespace MonitoringSystem.Views
{
    /// <summary>
    /// LineTempView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LineTempView : Window
    {
        public LineTempView()
        {
            InitializeComponent();

            GetEmployees();
            GetEmployees2();

            ChartValues<float> values = new ChartValues<float>();
            ChartValues<float> values2 = new ChartValues<float>();
            List<string> labels = new List<string>();
            List<string> labels2 = new List<string>();

            foreach (var tmp in Line)
            {
                values.Add(tmp.Sensor);
                labels.Add(tmp.CurrTime.Substring(10, 6));
            }
            foreach (var tmp in Line2)
            {
                values2.Add(tmp.Sensor);
                labels2.Add(tmp.CurrTime.Substring(10, 6));
            }

            var robotTempSeries = new LineSeries
            {
                Title = "Robot Temperature",
                Values = values
            };
            var conveyTempSeries = new LineSeries
            {
                Title = "Convey Temperature",
                Values = values2
            };

            SeriesCollection = new SeriesCollection();
            SeriesCollection.Add(robotTempSeries);
            SeriesCollection.Add(conveyTempSeries);

            Labels = labels.ToArray();

            DataContext = this;
        }

        public BindableCollection<TB_Line> Line;
        public BindableCollection<TB_Line> Line2;

        private void GetEmployees() // 1. SELECT 문
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                conn.Open();
                string selquery = Models.TB_Line.SELECT_QUERY2;

                SqlCommand cmd = new SqlCommand(selquery, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                Line = new BindableCollection<TB_Line>();
                while (reader.Read())
                {
                    Line.Add(new TB_Line
                    {
                        CurrTime = (string)reader["CurrTime"],
                        Code = (string)reader["Code"],
                        Sensor = float.Parse(reader["Sensor"].ToString()),
                    });
                }
            }
        }
        private void GetEmployees2() // 1. SELECT 문
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                conn.Open();
                string selquery2 = Models.TB_Line.SELECT_QUERY3;

                SqlCommand cmd2 = new SqlCommand(selquery2, conn);
                SqlDataReader reader2 = cmd2.ExecuteReader();

                Line2 = new BindableCollection<TB_Line>();
                while (reader2.Read())
                {
                    Line2.Add(new TB_Line
                    {
                        CurrTime = (string)reader2["CurrTime"],
                        Code = (string)reader2["Code"],
                        Sensor = float.Parse(reader2["Sensor"].ToString()),
                    });
                }
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
