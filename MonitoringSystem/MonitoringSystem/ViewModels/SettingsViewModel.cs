using Caliburn.Micro;
using MonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ViewModels
{
    public class SettingsViewModel : Conductor<object>
    {

        #region TB_SET 설정
        private BindableCollection<TB_SET> TB_set;

        public BindableCollection<TB_SET> TB_SET
        {
            get => TB_set;
            set
            {
                TB_set = value;
                NotifyOfPropertyChange(() => TB_SET);
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
        private float robotArm;

        public float RobotArm
        {
            get => robotArm;
            set
            {
                robotArm = value;
                NotifyOfPropertyChange(() => RobotArm);
            }
        }

        private float conveyor;

        public float Conveyor
        {
            get => conveyor;
            set
            {
                conveyor = value;
                NotifyOfPropertyChange(() => Conveyor);
            }
        }

        private float pumpT;

        public float PumpT
        {
            get => pumpT;
            set
            {
                pumpT = value;
                NotifyOfPropertyChange(() => PumpT);
            }
        }

        private float flowRate;

        public float FlowRate
        {
            get => flowRate;
            set
            {
                flowRate = value;
                NotifyOfPropertyChange(() => FlowRate);
            }
        }

        private float density;

        public float Density
        {
            get => density;
            set
            {
                density = value;
                NotifyOfPropertyChange(() => Density);
            }
        }

        #endregion
        #region TB_Employees 설정

        private BindableCollection<TB_Employees> TB_employees;
        public BindableCollection<TB_Employees> Employees
        {
            get => TB_employees;
            set
            {
                TB_employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }

        private int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        private string empName;
        public string EmpName
        {
            get => empName;
            set
            {
                empName = value;
                NotifyOfPropertyChange(() => EmpName);
            }
        }

        private string deptName;
        public string DeptName
        {
            get => deptName;
            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => DeptName);
            }
        }
        #endregion

        #region 조회 설정
        public void GetSettings()
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.TB_SET.SELECT_QUERY, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                TB_SET = new BindableCollection<TB_SET>();

                while (reader.Read())
                {
                    var empTmp = new TB_SET
                    {
                        PlantT = float.Parse(reader["PlantT"].ToString()),
                        PlantH = float.Parse(reader["PlantH"].ToString()),
                        RobotArm = float.Parse(reader["RobotArm"].ToString()),
                        Conveyor = float.Parse(reader["Conveyor"].ToString()),
                        PumpT = float.Parse(reader["PumpT"].ToString()),
                        FlowRate = float.Parse(reader["FlowRate"].ToString()),
                        Density = float.Parse(reader["Density"].ToString()),
                    };
                    TB_SET.Add(empTmp);
                }
            }
        }
        public void GetEmployees()
        {
            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.TB_Employees.SELECT_QUERY, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                Employees = new BindableCollection<TB_Employees>();

                while (reader.Read())
                {
                    var empTmp = new TB_Employees
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        DeptName = reader["DeptName"].ToString(),
                    };
                    Employees.Add(empTmp);
                }
            }
        }

        #endregion
        #region 창 오픈 시 조회

        public SettingsViewModel()
        {
            // DB연결
            GetEmployees();
        }
        #endregion
    }
}
