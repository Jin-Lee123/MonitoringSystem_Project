using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Models
{
    public class TB_Employees
    {
        public int Id { get; set; }
        public string EmpName { get; set; }
        public string DeptName { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT Id
                                                            , EmpName
                                                            , DeptName
                                                         FROM TB_Employees";
    }
}
