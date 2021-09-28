using Caliburn.Micro;
using MonitoringSystem.Models;
using MonitoringSystem.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Action = System.Action;

namespace MonitoringSystem.ViewModels
{
 
    public class LoginViewModel : Conductor<object>
    {
        public static Action CloseAction { get; set; }
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

        private string username;

        public string UserName
        {
            get => username;
            set
            {
                username = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        private string password;

        public string PassWord
        {
            get => password;
            set
            {
                password = value;
                NotifyOfPropertyChange(() => PassWord);
            }
        }
        private string visible;
        public string Visible
        {
            get => visible;
            set
            {
                visible = value;
                NotifyOfPropertyChange(() => Visible);
            }
        }
        #endregion
        public void LogIn()
        {

            using (SqlConnection conn = new SqlConnection(Common.CONNSTRING))
            {
                conn.Open();

                // 값이 empTmp에서 빠져나오지 않던 기존의 코드

                /*SqlCommand cmd = new SqlCommand(Models.TB_Employees.SELECT_QUERY, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                Employees = new BindableCollection<TB_Employees>();

                while (reader.Read())
                {
                    var empTmp = new TB_Employees
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        DeptName = reader["DeptName"].ToString()

                    };

                    Employees.Add(empTmp);

                }*/
                

                // 로그인 ID/PASSWORD 확인 (하드코딩)

                SqlDataAdapter adapter = new SqlDataAdapter(Models.TB_Employees.SELECT_QUERY, conn);
                DataTable DtTemp = new DataTable();
                adapter.Fill(DtTemp);

                int i = 0;
                while (true)
                { 
                    if (DtTemp.Rows[i]["EmpName"].ToString() == UserName && DtTemp.Rows[i]["DeptName"].ToString() == PassWord)
                    {
                        MessageBox.Show("로그인 되었습니다.");
                        CloseAction();
                        break;
                    }
                    else if (i == 3)
                    {
                        MessageBox.Show("틀렸습니다.");
                        break;

                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }


    }
}
