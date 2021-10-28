using Caliburn.Micro;
using MonitoringSystem.Models;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;

namespace MonitoringSystem.ViewModels
{
    class LineTempViewModel : Conductor<object>
    {

        private int dev_addr;
        public int Dev_addr
        {
            get => dev_addr;
            set
            {
                dev_addr = value;
                NotifyOfPropertyChange(() => Dev_addr);
            }
        }
        private int currtime;
        public int CurrTime
        {
            get => currtime;
            set
            {
                currtime = value;
                NotifyOfPropertyChange(() => CurrTime);
            }
        }
        private int code;
        public int Code
        {
            get => code;
            set
            {
                code = value;
                NotifyOfPropertyChange(() => Code);
            }
        }
        private int value;
        public int Value
        {
            get => value;
            set
            {
                value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }

        private int seonsor;
        public int Sensor
        {
            get => seonsor;
            set
            {
                seonsor = value;
                NotifyOfPropertyChange(() => Sensor);
            }
        }
        private int seonsor2;
        public int Sensor2
        {
            get => seonsor2;
            set
            {
                seonsor2 = value;
                NotifyOfPropertyChange(() => Sensor2);
            }
        }

        public BindableCollection<TB_Line> Line;

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
                    var empTmp = new TB_Line
                    {
                        Dev_addr = (string)reader["Dev_addr"],
                        CurrTime = (string)reader["CurrTime"],
                        Code = (string)reader["Code"],
                        Value = (double)reader["Value"],
                        Sensor = (double)reader["Sensor"],
                    };
                    Line.Add(empTmp);

                }
                conn.Close();
            }

            Dev_addr = Line[0].TotalQty;
            CurrTime = Line[0].ProdQty;
            Code = Line[0].BadQty;
            Value = Line[0].GoalQty;
            Sensor = Line[0].GoalQty;
        }
    }
}
