using Caliburn.Micro;
using MonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private double SplantT;

        public double SPlantT
        {
            get => SplantT;
            set
            {
                SplantT = value;
                NotifyOfPropertyChange(() => SPlantT);
            }
        }

        private double SplantH;

        public double SPlantH
        {
            get => SplantH;
            set
            {
                SplantH = value;
                NotifyOfPropertyChange(() => SPlantH);
            }
        }
        private double SrobotArm;

        public double SRobotArm
        {
            get => SrobotArm;
            set
            {
                SrobotArm = value;
                NotifyOfPropertyChange(() => SRobotArm);
            }
        }

        private double Sconveyor;

        public double SConveyor
        {
            get => Sconveyor;
            set
            {
                Sconveyor = value;
                NotifyOfPropertyChange(() => SConveyor);
            }
        }

        private double SpumpT;

        public double SPumpT
        {
            get => SpumpT;
            set
            {
                SpumpT = value;
                NotifyOfPropertyChange(() => SPumpT);
            }
        }

        private double SflowRate;

        public double SFlowRate
        {
            get => SflowRate;
            set
            {
                SflowRate = value;
                NotifyOfPropertyChange(() => SFlowRate);
            }
        }

        private double Sdensity;

        public double SDensity
        {
            get => Sdensity;
            set
            {
                Sdensity = value;
                NotifyOfPropertyChange(() => SDensity);
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
                SqlCommand cmd = new SqlCommand(Models.TB_SET.SELECT_QUERY, conn);
                try
                {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    SPlantT = double.Parse(reader["PlantT"].ToString());
                    SPlantH = double.Parse(reader["PlantH"].ToString());
                    SRobotArm = double.Parse(reader["RobotArm"].ToString());
                    SConveyor = double.Parse(reader["Conveyor"].ToString());
                    SPumpT = double.Parse(reader["PumpT"].ToString());
                    SFlowRate = double.Parse(reader["FlowRate"].ToString());
                    SDensity = double.Parse(reader["Density"].ToString());
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conn.Close();
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

        public void SaveSetting()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
                {
                    int resultRow = 0;
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                        cmd.CommandText = Models.TB_SET.UPDATE_QUERY;

                    SqlParameter PlantTParam = new SqlParameter("@PlantT", SPlantT);
                    cmd.Parameters.Add(PlantTParam);
                    SqlParameter PlantHParam = new SqlParameter("@PlantH", SPlantH);
                    cmd.Parameters.Add(PlantHParam);
                    SqlParameter RobotArmParam = new SqlParameter("@RobotArm", SRobotArm);
                    cmd.Parameters.Add(RobotArmParam);
                    SqlParameter ConveyorParam = new SqlParameter("@Conveyor", SConveyor);
                    cmd.Parameters.Add(ConveyorParam);
                    SqlParameter PumpTParam = new SqlParameter("@PumpT", SPumpT);
                    cmd.Parameters.Add(PumpTParam);
                    SqlParameter FlowRateParam = new SqlParameter("@FlowRate", SFlowRate);
                    cmd.Parameters.Add(FlowRateParam);
                    SqlParameter DensityParam = new SqlParameter("@Density", SDensity);
                    cmd.Parameters.Add(DensityParam);
                    if (Id != 0) // Update일때만 Id 사용 (분기안하면 무조건 에러)
                    {
                        SqlParameter idParam = new SqlParameter("@id", Id);
                        cmd.Parameters.Add(idParam);
                    }

                    resultRow = cmd.ExecuteNonQuery();

                    if (resultRow > 0)
                    {
                        MessageBox.Show("저장되었습니다!");
                        GetSettings();
                    }
                    else
                    {
                        MessageBox.Show("데이터 저장 실패!");
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"예외발생 : {ex.Message}");
                //return;
            }
            finally
            {
            }
        }
    

        #endregion
        #region 창 오픈 시 조회

        public SettingsViewModel()
        {
            // DB연결
            GetEmployees();
            GetSettings();
        }
        #endregion
    }
}
